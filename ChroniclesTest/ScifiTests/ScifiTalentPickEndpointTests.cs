namespace ChroniclesTest;

using System.Security.Claims;
using ChroniclesAPI.Controllers;
using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Dice;
using PlayerApp.Models.Enums;

public class ScifiTalentPickEndpointTests {
    private const int PlayerId = 1;

    private static PlayerContext CreatePlayerContext() {
        var context = new DefaultHttpContext {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, PlayerId.ToString())], "test"))
        };
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        return new PlayerContext(accessor);
    }

    private static ScifiCharacterController CreateCharacterController(AppDbContext db) =>
        new(db, CreatePlayerContext(), new ScifiTalentService(db));

    private static ScifiTalentController CreateTalentController(AppDbContext db) =>
        new(db, CreatePlayerContext(), new ScifiTalentService(db));

    private static async Task<ScifiTalent> AddTalent(AppDbContext db, string name) {
        var talent = new ScifiTalent(name, $"{name} description", $"{name} ability");
        db.ScifiTalents.Add(talent);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return talent;
    }

    private static async Task<ScifiClass> AddClass(AppDbContext db, string name, params ScifiTalent[] granted) {
        var scifiClass = new ScifiClass(name, DiceType.D12);
        db.ScifiClasses.Add(scifiClass);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.AddRange(granted.Select(t => new ScifiClassTalent(scifiClass.Id, t.Id)));
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return scifiClass;
    }

    private static async Task<ScifiBackground> AddBackground(AppDbContext db, string name, bool overlap, params ScifiTalent[] talents) {
        var background = new ScifiBackground(name, $"{name} description", hasClassOverlap: overlap);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiBackgroundTalents.AddRange(talents.Select(t => new ScifiBackgroundTalent(background.Id, t.Id)));
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return background;
    }

    private static async Task<Character> AddCharacter(AppDbContext db) {
        var character = new Character(PlayerId, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return character;
    }

    private static async Task<ScifiCharacter> AddScifiCharacter(AppDbContext db, ScifiClass scifiClass, ScifiBackground background) {
        var character = await AddCharacter(db);
        var sc = new ScifiCharacter(character.Id);
        sc.SetScifiClass(scifiClass.Id);
        sc.SetScifiBackground(background.Id);
        db.ScifiCharacters.Add(sc);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return sc;
    }

    private static async Task<List<(int TalentId, TalentSourceEnum Source)>> Stored(AppDbContext db, int scId) =>
        (await db.Set<ScifiCharacterTalent>().AsNoTracking().Where(t => t.ScifiCharacterId == scId).ToListAsync(TestContext.Current.CancellationToken))
            .Select(t => (t.ScifiTalentId, t.SourceType)).OrderBy(t => t.ScifiTalentId).ToList();

    private static async Task<int?> StoredClassId(AppDbContext db, int scId) =>
        (await db.ScifiCharacters.AsNoTracking().FirstAsync(s => s.Id == scId, TestContext.Current.CancellationToken)).ScifiClassId;

    // Update

    [Fact]
    public async Task Update_ValidPick_SavesIt() {
        await using var db = TestDb.Create();
        var granted = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Scavenger");
        var bgB = await AddTalent(db, "Haggler");
        var scifiClass = await AddClass(db, "Soldier", granted);
        var background = await AddBackground(db, "Drifter", false, bgA, bgB);
        var sc = await AddScifiCharacter(db, scifiClass, background);

        var result = await CreateCharacterController(db).Update(sc.Id, new(null, null, null, null, null, bgA.Id));

        Assert.IsType<OkObjectResult>(result);
        db.ChangeTracker.Clear();
        Assert.Contains((bgA.Id, TalentSourceEnum.Background), await Stored(db, sc.Id));
    }

    [Fact]
    public async Task Update_InvalidPick_Returns400_AndSavesNothing_EvenAClassChange() {
        await using var db = TestDb.Create();
        var granted = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Scavenger");
        var outsider = await AddTalent(db, "Outsider");
        var classA = await AddClass(db, "Soldier", granted);
        var classB = await AddClass(db, "Medic");
        var background = await AddBackground(db, "Drifter", false, bgA);
        var sc = await AddScifiCharacter(db, classA, background);
        var controller = CreateCharacterController(db);

        // Class change in the same request as a pick that isn't in the pool
        var result = await controller.Update(sc.Id, new(null, null, classB.Id, null, null, outsider.Id));

        Assert.IsType<BadRequestObjectResult>(result);
        db.ChangeTracker.Clear();
        Assert.Equal(classA.Id, await StoredClassId(db, sc.Id));
        Assert.DoesNotContain(await Stored(db, sc.Id), t => t.Source == TalentSourceEnum.Background);
    }

    [Fact]
    public async Task Update_ClassChangeAndPick_ValidatesPickAgainstNewClass() {
        await using var db = TestDb.Create();
        var soldierTalent = await AddTalent(db, "Gunnery");
        var medicTalent = await AddTalent(db, "Triage");
        var bgSoldier = await AddTalent(db, "Marksman");
        var bgMedic = await AddTalent(db, "Bedside Manner");
        var soldier = await AddClass(db, "Soldier", soldierTalent);
        var medic = await AddClass(db, "Medic", medicTalent);
        // Overlap background named for Soldier: Soldier picks from the background's talents,
        // any other class picks from the Soldier class's talents instead
        var background = await AddBackground(db, "Soldier", true, bgSoldier);
        var sc = await AddScifiCharacter(db, soldier, background);
        var controller = CreateCharacterController(db);

        // bgSoldier is pickable for Soldier, but not once the class becomes Medic
        var rejected = await controller.Update(sc.Id, new(null, null, medic.Id, null, null, bgSoldier.Id));
        Assert.IsType<BadRequestObjectResult>(rejected);

        db.ChangeTracker.Clear();
        // soldierTalent is only pickable under Medic (namesake class talents), so this one is valid
        var accepted = await CreateCharacterController(db).Update(sc.Id, new(null, null, medic.Id, null, null, soldierTalent.Id));

        Assert.IsType<OkObjectResult>(accepted);
        db.ChangeTracker.Clear();
        Assert.Equal(medic.Id, await StoredClassId(db, sc.Id));
        var stored = await Stored(db, sc.Id);
        Assert.Contains((soldierTalent.Id, TalentSourceEnum.Background), stored);
        Assert.Contains((medicTalent.Id, TalentSourceEnum.Class), stored);
    }

    // Create

    [Fact]
    public async Task Create_ValidPick_SavesCharacterWithClassTalentsAndPick() {
        await using var db = TestDb.Create();
        var granted = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Scavenger");
        var scifiClass = await AddClass(db, "Soldier", granted);
        var background = await AddBackground(db, "Drifter", false, bgA);
        var character = await AddCharacter(db);

        var result = await CreateCharacterController(db)
            .Create(new(character.Id, null, null, scifiClass.Id, background.Id, null, bgA.Id));

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ScifiCharacterController.ScifiCharacterResponseDto>(ok.Value);
        db.ChangeTracker.Clear();
        var stored = await Stored(db, dto.Id);
        Assert.Equal(2, stored.Count);
        Assert.Contains((granted.Id, TalentSourceEnum.Class), stored);
        Assert.Contains((bgA.Id, TalentSourceEnum.Background), stored);
    }

    [Fact]
    public async Task Create_InvalidPick_Returns400_AndCreatesNoCharacter() {
        await using var db = TestDb.Create();
        var granted = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Scavenger");
        var outsider = await AddTalent(db, "Outsider");
        var scifiClass = await AddClass(db, "Soldier", granted);
        var background = await AddBackground(db, "Drifter", false, bgA);
        var character = await AddCharacter(db);

        var result = await CreateCharacterController(db)
            .Create(new(character.Id, null, null, scifiClass.Id, background.Id, null, outsider.Id));

        Assert.IsType<BadRequestObjectResult>(result);
        db.ChangeTracker.Clear();
        Assert.Empty(await db.ScifiCharacters.ToListAsync(TestContext.Current.CancellationToken));
        Assert.Empty(await db.Set<ScifiCharacterTalent>().ToListAsync(TestContext.Current.CancellationToken));
    }

    // PreviewTalentPool: one test per pool branch

    private static List<ScifiTalentController.ScifiTalentPickPreviewDto> Preview(IActionResult result) {
        var ok = Assert.IsType<OkObjectResult>(result);
        return Assert.IsType<List<ScifiTalentController.ScifiTalentPickPreviewDto>>(ok.Value);
    }

    [Fact]
    public async Task Preview_NoOverlap_GrantsClassTalents_AndOffersBackgroundTalents() {
        await using var db = TestDb.Create();
        var granted = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Scavenger");
        var scifiClass = await AddClass(db, "Soldier", granted);
        var background = await AddBackground(db, "Drifter", false, bgA);

        var picks = Preview(await CreateTalentController(db).PreviewTalentPool(scifiClass.Id, background.Id));

        Assert.Equal(2, picks.Count);
        Assert.False(picks.Single(p => p.Id == granted.Id).IsPickable);
        Assert.True(picks.Single(p => p.Id == bgA.Id).IsPickable);
    }

    [Fact]
    public async Task Preview_OverlapAndClassMatchesBackground_OffersBackgroundTalents() {
        await using var db = TestDb.Create();
        var granted = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Marksman");
        var scifiClass = await AddClass(db, "Soldier", granted);
        var background = await AddBackground(db, "Soldier", true, bgA);

        var picks = Preview(await CreateTalentController(db).PreviewTalentPool(scifiClass.Id, background.Id));

        Assert.False(picks.Single(p => p.Id == granted.Id).IsPickable);
        Assert.True(picks.Single(p => p.Id == bgA.Id).IsPickable);
    }

    [Fact]
    public async Task Preview_OverlapMismatch_OffersNamesakeClassTalents() {
        await using var db = TestDb.Create();
        var medicTalent = await AddTalent(db, "Triage");
        var soldierTalent = await AddTalent(db, "Gunnery");
        var bgOnly = await AddTalent(db, "Marksman");
        var medic = await AddClass(db, "Medic", medicTalent);
        await AddClass(db, "Soldier", soldierTalent);
        var background = await AddBackground(db, "Soldier", true, bgOnly);

        var picks = Preview(await CreateTalentController(db).PreviewTalentPool(medic.Id, background.Id));

        Assert.Equal(2, picks.Count);
        Assert.False(picks.Single(p => p.Id == medicTalent.Id).IsPickable);
        Assert.True(picks.Single(p => p.Id == soldierTalent.Id).IsPickable);
        Assert.DoesNotContain(picks, p => p.Id == bgOnly.Id);
    }

    [Fact]
    public async Task Preview_OverlapMismatch_NoNamesakeClass_OffersNothingPickable() {
        await using var db = TestDb.Create();
        var medicTalent = await AddTalent(db, "Triage");
        var medic = await AddClass(db, "Medic", medicTalent);
        var background = await AddBackground(db, "Ghost", true);

        var picks = Preview(await CreateTalentController(db).PreviewTalentPool(medic.Id, background.Id));

        var only = Assert.Single(picks);
        Assert.Equal(medicTalent.Id, only.Id);
        Assert.False(only.IsPickable);
    }

    [Fact]
    public async Task Preview_UnknownClassOrBackground_ReturnsNotFound() {
        await using var db = TestDb.Create();
        var scifiClass = await AddClass(db, "Soldier");
        var background = await AddBackground(db, "Drifter", false);
        var controller = CreateTalentController(db);

        Assert.IsType<NotFoundObjectResult>(await controller.PreviewTalentPool(999, background.Id));
        Assert.IsType<NotFoundObjectResult>(await controller.PreviewTalentPool(scifiClass.Id, 999));
    }
}
