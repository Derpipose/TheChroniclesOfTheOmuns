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

public class ScifiTalentTests {
    [Fact]
    public void Constructor_SetsNameDescriptionAndAbility() {
        var talent = new ScifiTalent("Resourcefulness", "Ability to make do with what's available.", "An ability");

        Assert.Equal("Resourcefulness", talent.Name);
        Assert.Equal("Ability to make do with what's available.", talent.Description);
        Assert.Equal("An ability", talent.Ability);
    }

    [Fact]
    public void Constructor_PerkAndPerkAmount_DefaultToEmpty() {
        var talent = new ScifiTalent("Resourcefulness", "Description", "An ability");

        Assert.Equal(string.Empty, talent.Perk);
        Assert.Equal(string.Empty, talent.PerkAmount);
    }

    [Fact]
    public void Constructor_SetsPerkAndPerkAmount_WhenProvided() {
        var talent = new ScifiTalent("Tough", "Description", "An ability", "Damage Resist", "2");

        Assert.Equal("Damage Resist", talent.Perk);
        Assert.Equal("2", talent.PerkAmount);
    }

    [Fact]
    public void Id_DefaultsToZero() {
        var talent = new ScifiTalent("Resourcefulness", "Description", "An ability");

        Assert.Equal(0, talent.Id);
    }

    [Fact]
    public void Setters_UpdateEachField() {
        var talent = new ScifiTalent("Old", "Old description", "Old ability", "Old perk", "1");

        talent.SetName("New");
        talent.SetDescription("New description");
        talent.SetAbility("New ability");
        talent.SetPerk("New perk");
        talent.SetPerkAmount("5");

        Assert.Equal("New", talent.Name);
        Assert.Equal("New description", talent.Description);
        Assert.Equal("New ability", talent.Ability);
        Assert.Equal("New perk", talent.Perk);
        Assert.Equal("5", talent.PerkAmount);
    }

    [Fact]
    public void Setter_OnlyChangesTargetField() {
        var talent = new ScifiTalent("Name", "Description", "Ability", "Perk", "1");

        talent.SetName("Renamed");

        Assert.Equal("Description", talent.Description);
        Assert.Equal("Ability", talent.Ability);
        Assert.Equal("Perk", talent.Perk);
        Assert.Equal("1", talent.PerkAmount);
    }

#if DEBUG
    [Fact]
    public void SimulatePersist_SetsId() {
        var talent = new ScifiTalent("Resourcefulness", "Description", "An ability");
        talent.SimulatePersist(1);

        Assert.Equal(1, talent.Id);
    }
#endif

    // ScifiCharacterTalent join entity

    [Theory]
    [InlineData(TalentSourceEnum.Class)]
    [InlineData(TalentSourceEnum.Background)]
    [InlineData(TalentSourceEnum.LevelUp)]
    public void CharacterTalent_Constructor_SetsIdsAndSource(TalentSourceEnum source) {
        var characterTalent = new ScifiCharacterTalent(3, 7, source);

        Assert.Equal(3, characterTalent.ScifiCharacterId);
        Assert.Equal(7, characterTalent.ScifiTalentId);
        Assert.Equal(source, characterTalent.SourceType);
    }

    // ScifiCharacter talent rules

    private static async Task<(AppDbContext db, ScifiCharacter sc, ScifiTalent talent)> SeedCharacterAndTalent(bool active = false) {
        var db = TestDb.Create();
        var character = new Character(1, "Rook");
        db.Characters.Add(character);
        var talent = new ScifiTalent("Tough", "Description", "An ability");
        db.ScifiTalents.Add(talent);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sc = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(sc);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        sc = await db.ScifiCharacters.Include(s => s.Character).FirstAsync(TestContext.Current.CancellationToken);
        if (active) sc.Character!.SetActive();
        return (db, sc, talent);
    }

    [Fact]
    public async Task AddTalent_AddsTalentWithSource() {
        var (db, sc, talent) = await SeedCharacterAndTalent();
        await using var _ = db;

        sc.AddTalent(talent, TalentSourceEnum.Background);

        var added = Assert.Single(sc.Talents);
        Assert.Equal(talent.Id, added.ScifiTalentId);
        Assert.Equal(TalentSourceEnum.Background, added.SourceType);
    }

    [Fact]
    public async Task AddTalent_Throws_WhenSameTalentAddedTwice() {
        var (db, sc, talent) = await SeedCharacterAndTalent();
        await using var _ = db;
        sc.AddTalent(talent, TalentSourceEnum.Background);

        Assert.Throws<InvalidOperationException>(() => sc.AddTalent(talent, TalentSourceEnum.Class));
    }

    [Fact]
    public async Task AddTalent_Throws_WhenTalentNotPersisted() {
        var (db, sc, _) = await SeedCharacterAndTalent();
        await using var _ = db;

        Assert.Throws<InvalidOperationException>(() =>
            sc.AddTalent(new ScifiTalent("Unsaved", "Description", "Ability"), TalentSourceEnum.Class));
    }

    [Fact]
    public async Task AddTalent_Throws_WhenCharacterNotLoaded() {
        // Persisted character and talent, so only the missing Character can trigger the throw
        var (db, _, talent) = await SeedCharacterAndTalent();
        await using var _ = db;
        db.ChangeTracker.Clear();
        var sc = await db.ScifiCharacters.FirstAsync(TestContext.Current.CancellationToken);
        Assert.Null(sc.Character);

        var ex = Assert.Throws<InvalidOperationException>(() => sc.AddTalent(talent, TalentSourceEnum.Class));
        Assert.Contains("associated Character", ex.Message);
    }

    [Theory]
    [InlineData(TalentSourceEnum.Class)]
    [InlineData(TalentSourceEnum.Background)]
    public async Task AddTalent_Throws_ForClassOrBackground_WhenCharacterActive(TalentSourceEnum source) {
        var (db, sc, talent) = await SeedCharacterAndTalent(active: true);
        await using var _ = db;

        Assert.Throws<InvalidOperationException>(() => sc.AddTalent(talent, source));
    }

    [Fact]
    public async Task AddTalent_AllowsLevelUp_WhenCharacterActive() {
        var (db, sc, talent) = await SeedCharacterAndTalent(active: true);
        await using var _ = db;

        sc.AddTalent(talent, TalentSourceEnum.LevelUp);

        Assert.Single(sc.Talents);
    }

    [Fact]
    public async Task ClearTalentsBySource_RemovesOnlyThatSource() {
        var (db, sc, talent) = await SeedCharacterAndTalent();
        await using var _ = db;
        var other = new ScifiTalent("Quick", "Description", "Ability");
        db.ScifiTalents.Add(other);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        sc.AddTalent(talent, TalentSourceEnum.Background);
        sc.AddTalent(other, TalentSourceEnum.Class);

        sc.ClearTalentsBySource(TalentSourceEnum.Background);

        var remaining = Assert.Single(sc.Talents);
        Assert.Equal(TalentSourceEnum.Class, remaining.SourceType);
    }

    [Theory]
    [InlineData(TalentSourceEnum.Class)]
    [InlineData(TalentSourceEnum.Background)]
    [InlineData(TalentSourceEnum.LevelUp)]
    public async Task ClearTalentsBySource_Throws_WhenCharacterActive(TalentSourceEnum source) {
        var (db, sc, _) = await SeedCharacterAndTalent(active: true);
        await using var _ = db;

        Assert.Throws<InvalidOperationException>(() => sc.ClearTalentsBySource(source));
    }
}

public class ScifiTalentControllerTests {
    private const int PlayerId = 1;

    private static ScifiTalentController CreateController(AppDbContext db, int playerId = PlayerId) {
        var context = new DefaultHttpContext {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, playerId.ToString())], "test"))
        };
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        return new ScifiTalentController(db, new PlayerContext(accessor), new ScifiTalentService(db));
    }

    private static async Task<ScifiTalent> AddTalent(AppDbContext db, string name, string perk = "", string perkAmount = "") {
        var talent = new ScifiTalent(name, $"{name} description", $"{name} ability", perk, perkAmount);
        db.ScifiTalents.Add(talent);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return talent;
    }

    private static async Task<ScifiCharacter> AddScifiCharacter(AppDbContext db, int playerId = PlayerId) {
        var character = new Character(playerId, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sc = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(sc);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return sc;
    }

    private static async Task SetClassAndBackground(AppDbContext db, ScifiCharacter sc, ScifiClass scifiClass, ScifiBackground background) {
        sc.SetScifiClass(scifiClass.Id);
        sc.SetScifiBackground(background.Id);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static List<ScifiTalentController.ScifiTalentPicksResponseDto> Picks(IActionResult result) {
        var ok = Assert.IsType<OkObjectResult>(result);
        return Assert.IsType<List<ScifiTalentController.ScifiTalentPicksResponseDto>>(ok.Value);
    }

    // GetAll / GetById

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoTalents() {
        await using var db = TestDb.Create();

        var result = await CreateController(db).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Empty(Assert.IsAssignableFrom<IEnumerable<ScifiTalentController.ScifiTalentResponseDto>>(ok.Value));
    }

    [Fact]
    public async Task GetAll_ReturnsTalentsOrderedByName() {
        await using var db = TestDb.Create();
        await AddTalent(db, "Zeal");
        await AddTalent(db, "Aim");
        await AddTalent(db, "Meditation");

        var result = await CreateController(db).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var talents = Assert.IsAssignableFrom<IEnumerable<ScifiTalentController.ScifiTalentResponseDto>>(ok.Value).ToList();
        Assert.Equal(["Aim", "Meditation", "Zeal"], talents.Select(t => t.Name));
    }

    [Fact]
    public async Task GetById_ReturnsTalentWithPerk() {
        await using var db = TestDb.Create();
        var talent = await AddTalent(db, "Tough", "Damage Resist", "2");

        var result = await CreateController(db).GetById(talent.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ScifiTalentController.ScifiTalentResponseDto>(ok.Value);
        Assert.Equal("Tough", dto.Name);
        Assert.Equal("Tough ability", dto.Ability);
        Assert.Equal("Damage Resist", dto.Perk);
        Assert.Equal("2", dto.PerkAmount);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing() {
        await using var db = TestDb.Create();

        var result = await CreateController(db).GetById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // GetTalentPicks

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetTalentPicks_ReturnsBadRequest_ForInvalidCharacterId(int characterId) {
        await using var db = TestDb.Create();

        var result = await CreateController(db).GetTalentPicks(characterId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetTalentPicks_ReturnsNotFound_WhenCharacterMissing() {
        await using var db = TestDb.Create();

        var result = await CreateController(db).GetTalentPicks(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetTalentPicks_ReturnsNotFound_WhenCharacterBelongsToAnotherPlayer() {
        await using var db = TestDb.Create();
        var sc = await AddScifiCharacter(db, playerId: 2);

        var result = await CreateController(db, playerId: PlayerId).GetTalentPicks(sc.CharacterId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetTalentPicks_ReturnsBadRequest_WhenNoClassOrBackground() {
        await using var db = TestDb.Create();
        var sc = await AddScifiCharacter(db);

        var result = await CreateController(db).GetTalentPicks(sc.CharacterId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetTalentPicks_ReturnsBadRequest_WhenOnlyClassSet() {
        await using var db = TestDb.Create();
        var sc = await AddScifiCharacter(db);
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);
        db.ScifiClasses.Add(scifiClass);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        sc.SetScifiClass(scifiClass.Id);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await CreateController(db).GetTalentPicks(sc.CharacterId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetTalentPicks_NoOverlap_ReturnsClassTalentsGrantedAndBackgroundTalentsPickable() {
        await using var db = TestDb.Create();
        var classTalent = await AddTalent(db, "Gunnery");
        var bgTalent = await AddTalent(db, "Haggler");
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Merchant", "Sells things", hasClassOverlap: false);
        db.ScifiClasses.Add(scifiClass);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.Add(new ScifiClassTalent(scifiClass.Id, classTalent.Id));
        db.ScifiBackgroundTalents.Add(new ScifiBackgroundTalent(background.Id, bgTalent.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, scifiClass, background);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        Assert.Equal(2, picks.Count);
        var granted = picks.Single(p => p.Id == classTalent.Id);
        Assert.False(granted.IsPickable);
        Assert.False(granted.IsSelected);
        var pickable = picks.Single(p => p.Id == bgTalent.Id);
        Assert.True(pickable.IsPickable);
        Assert.False(pickable.IsSelected);
        Assert.Equal(classTalent.Id, picks[0].Id); // granted talents come first
    }

    [Fact]
    public async Task GetTalentPicks_MarksSavedBackgroundTalentAsSelected() {
        await using var db = TestDb.Create();
        var bgPicked = await AddTalent(db, "Haggler");
        var bgOther = await AddTalent(db, "Appraiser");
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Merchant", "Sells things");
        db.ScifiClasses.Add(scifiClass);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiBackgroundTalents.AddRange(
            new ScifiBackgroundTalent(background.Id, bgPicked.Id),
            new ScifiBackgroundTalent(background.Id, bgOther.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, scifiClass, background);

        var loaded = await db.ScifiCharacters.Include(s => s.Character).FirstAsync(TestContext.Current.CancellationToken);
        loaded.AddTalent(bgPicked, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        Assert.True(picks.Single(p => p.Id == bgPicked.Id).IsSelected);
        Assert.False(picks.Single(p => p.Id == bgOther.Id).IsSelected);
    }

    [Fact]
    public async Task GetTalentPicks_OverlapMismatch_PoolsFromClassMatchingBackgroundName() {
        await using var db = TestDb.Create();
        var ownClassTalent = await AddTalent(db, "Gunnery");
        var matchingClassTalent = await AddTalent(db, "Piloting");
        var ignoredBgTalent = await AddTalent(db, "Haggler");
        var soldier = new ScifiClass("Soldier", DiceType.D12);
        var pilot = new ScifiClass("Pilot", DiceType.D8);
        var background = new ScifiBackground("Pilot", "Flew ships", hasClassOverlap: true);
        db.ScifiClasses.AddRange(soldier, pilot);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.AddRange(
            new ScifiClassTalent(soldier.Id, ownClassTalent.Id),
            new ScifiClassTalent(pilot.Id, matchingClassTalent.Id));
        db.ScifiBackgroundTalents.Add(new ScifiBackgroundTalent(background.Id, ignoredBgTalent.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, soldier, background);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        Assert.Equal(2, picks.Count);
        Assert.False(picks.Single(p => p.Id == ownClassTalent.Id).IsPickable);
        Assert.True(picks.Single(p => p.Id == matchingClassTalent.Id).IsPickable);
        Assert.DoesNotContain(picks, p => p.Id == ignoredBgTalent.Id);
    }

    [Fact]
    public async Task GetTalentPicks_OverlapMismatch_MarksSavedTalentAsSelected() {
        await using var db = TestDb.Create();
        var ownClassTalent = await AddTalent(db, "Gunnery");
        var pilotPicked = await AddTalent(db, "Piloting");
        var pilotOther = await AddTalent(db, "Dogfighter");
        var soldier = new ScifiClass("Soldier", DiceType.D12);
        var pilot = new ScifiClass("Pilot", DiceType.D8);
        var background = new ScifiBackground("Pilot", "Flew ships", hasClassOverlap: true);
        db.ScifiClasses.AddRange(soldier, pilot);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.AddRange(
            new ScifiClassTalent(soldier.Id, ownClassTalent.Id),
            new ScifiClassTalent(pilot.Id, pilotPicked.Id),
            new ScifiClassTalent(pilot.Id, pilotOther.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, soldier, background);

        var loaded = await db.ScifiCharacters.Include(s => s.Character).FirstAsync(TestContext.Current.CancellationToken);
        loaded.AddTalent(pilotPicked, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        Assert.True(picks.Single(p => p.Id == pilotPicked.Id).IsSelected);
        Assert.False(picks.Single(p => p.Id == pilotOther.Id).IsSelected);
        Assert.False(picks.Single(p => p.Id == ownClassTalent.Id).IsSelected);
    }

    [Fact]
    public async Task GetTalentPicks_OverlapButClassMatchesBackground_PoolsFromBackgroundTalents() {
        await using var db = TestDb.Create();
        var classTalent = await AddTalent(db, "Piloting");
        var bgTalent = await AddTalent(db, "Dogfighter");
        var pilot = new ScifiClass("Pilot", DiceType.D8);
        var background = new ScifiBackground("Pilot", "Flew ships", hasClassOverlap: true);
        db.ScifiClasses.Add(pilot);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.Add(new ScifiClassTalent(pilot.Id, classTalent.Id));
        db.ScifiBackgroundTalents.Add(new ScifiBackgroundTalent(background.Id, bgTalent.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, pilot, background);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        Assert.Equal(2, picks.Count);
        Assert.False(picks.Single(p => p.Id == classTalent.Id).IsPickable);
        Assert.True(picks.Single(p => p.Id == bgTalent.Id).IsPickable);
    }

    [Fact]
    public async Task GetTalentPicks_OverlapMismatch_ReturnsOnlyGranted_WhenNoClassMatchesBackground() {
        await using var db = TestDb.Create();
        var soldier = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Hermit", "Lives alone", hasClassOverlap: true);
        db.ScifiClasses.Add(soldier);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, soldier, background);

        var result = await CreateController(db).GetTalentPicks(sc.CharacterId);

        Assert.DoesNotContain(Picks(result), p => p.IsPickable);
    }

    [Fact]
    public async Task GetTalentPicks_DoesNotDuplicateTalentSharedByClassAndPool() {
        await using var db = TestDb.Create();
        var shared = await AddTalent(db, "Shared");
        var soldier = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Merchant", "Sells things");
        db.ScifiClasses.Add(soldier);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.Add(new ScifiClassTalent(soldier.Id, shared.Id));
        db.ScifiBackgroundTalents.Add(new ScifiBackgroundTalent(background.Id, shared.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, soldier, background);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        var only = Assert.Single(picks);
        Assert.False(only.IsPickable); // the granted entry wins
    }

    [Fact]
    public async Task GetTalentPicks_ReturnsEmpty_WhenClassAndBackgroundHaveNoTalents() {
        await using var db = TestDb.Create();
        var soldier = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Merchant", "Sells things");
        db.ScifiClasses.Add(soldier);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, soldier, background);

        var picks = Picks(await CreateController(db).GetTalentPicks(sc.CharacterId));

        Assert.Empty(picks);
    }

    // SelectTalentPick

    private static async Task<(ScifiCharacter sc, ScifiTalent classTalent, ScifiTalent bgA, ScifiTalent bgB)> SeedPickScenario(AppDbContext db) {
        var classTalent = await AddTalent(db, "Gunnery");
        var bgA = await AddTalent(db, "Haggler");
        var bgB = await AddTalent(db, "Appraiser");
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Merchant", "Sells things");
        db.ScifiClasses.Add(scifiClass);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ScifiClassTalents.Add(new ScifiClassTalent(scifiClass.Id, classTalent.Id));
        db.ScifiBackgroundTalents.AddRange(
            new ScifiBackgroundTalent(background.Id, bgA.Id),
            new ScifiBackgroundTalent(background.Id, bgB.Id));
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, scifiClass, background);
        return (sc, classTalent, bgA, bgB);
    }

    private static async Task<List<ScifiCharacterTalent>> BackgroundTalents(AppDbContext db, int characterId) {
        var loaded = await db.ScifiCharacters.Include(s => s.Talents).FirstAsync(s => s.CharacterId == characterId, TestContext.Current.CancellationToken);
        return loaded.Talents.Where(t => t.SourceType == TalentSourceEnum.Background).ToList();
    }

    [Fact]
    public async Task SelectTalentPick_SavesValidPick() {
        await using var db = TestDb.Create();
        var (sc, _, bgA, _) = await SeedPickScenario(db);

        var result = await CreateController(db).SelectTalentPick(sc.CharacterId, bgA.Id);

        Assert.IsType<OkResult>(result);
        var saved = Assert.Single(await BackgroundTalents(db, sc.CharacterId));
        Assert.Equal(bgA.Id, saved.ScifiTalentId);
    }

    [Fact]
    public async Task SelectTalentPick_ReplacesPreviousPick() {
        await using var db = TestDb.Create();
        var (sc, _, bgA, bgB) = await SeedPickScenario(db);
        var controller = CreateController(db);
        await controller.SelectTalentPick(sc.CharacterId, bgA.Id);

        var result = await controller.SelectTalentPick(sc.CharacterId, bgB.Id);

        Assert.IsType<OkResult>(result);
        var saved = Assert.Single(await BackgroundTalents(db, sc.CharacterId));
        Assert.Equal(bgB.Id, saved.ScifiTalentId);
    }

    [Fact]
    public async Task SelectTalentPick_ReselectingSamePick_IsNoOp() {
        await using var db = TestDb.Create();
        var (sc, _, bgA, _) = await SeedPickScenario(db);
        var controller = CreateController(db);
        await controller.SelectTalentPick(sc.CharacterId, bgA.Id);

        var result = await controller.SelectTalentPick(sc.CharacterId, bgA.Id);

        Assert.IsType<OkResult>(result);
        var saved = Assert.Single(await BackgroundTalents(db, sc.CharacterId));
        Assert.Equal(bgA.Id, saved.ScifiTalentId);
    }

    [Fact]
    public async Task SelectTalentPick_ReturnsBadRequest_WhenTalentNotInPool() {
        await using var db = TestDb.Create();
        var (sc, _, _, _) = await SeedPickScenario(db);
        var outsider = await AddTalent(db, "Outsider");

        var result = await CreateController(db).SelectTalentPick(sc.CharacterId, outsider.Id);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(await BackgroundTalents(db, sc.CharacterId));
    }

    [Fact]
    public async Task SelectTalentPick_ReturnsBadRequest_WhenTalentIsClassGranted() {
        await using var db = TestDb.Create();
        var (sc, classTalent, _, _) = await SeedPickScenario(db);

        var result = await CreateController(db).SelectTalentPick(sc.CharacterId, classTalent.Id);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(await BackgroundTalents(db, sc.CharacterId));
    }

    [Fact]
    public async Task SelectTalentPick_ReturnsConflict_WhenCharacterActive() {
        await using var db = TestDb.Create();
        var (sc, _, bgA, _) = await SeedPickScenario(db);
        var loaded = await db.ScifiCharacters.Include(s => s.Character).FirstAsync(TestContext.Current.CancellationToken);
        loaded.Character!.SetActive();
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await CreateController(db).SelectTalentPick(sc.CharacterId, bgA.Id);

        Assert.IsType<ConflictObjectResult>(result);
        Assert.Empty(await BackgroundTalents(db, sc.CharacterId));
    }
}

public class ScifiTalentSyncTests {
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

    private static async Task<ScifiCharacter> AddScifiCharacter(AppDbContext db) {
        var character = new Character(1, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sc = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(sc);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return await Load(db, sc.Id);
    }

    // Reloads with everything the sync needs, picking up FK changes the way the controller does
    private static async Task<ScifiCharacter> Load(AppDbContext db, int id) {
        db.ChangeTracker.DetectChanges();
        return await db.ScifiCharacters
            .Include(s => s.Character)
            .Include(s => s.ScifiClass)
            .Include(s => s.ScifiBackground)
            .Include(s => s.Talents)
            .FirstAsync(s => s.Id == id, TestContext.Current.CancellationToken);
    }

    private static async Task Sync(AppDbContext db, ScifiCharacter sc) {
        db.ChangeTracker.DetectChanges();
        await new ScifiTalentService(db).SyncTalentsAsync(sc);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task<List<(int TalentId, TalentSourceEnum Source)>> Stored(AppDbContext db, int scId) =>
        (await db.Set<ScifiCharacterTalent>().Where(t => t.ScifiCharacterId == scId).ToListAsync(TestContext.Current.CancellationToken))
            .Select(t => (t.ScifiTalentId, t.SourceType)).OrderBy(t => t.ScifiTalentId).ToList();

    [Fact]
    public async Task Sync_BothSet_AddsClassTalents() {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var s2 = await AddTalent(db, "Armor");
        var b1 = await AddTalent(db, "Haggler");
        var soldier = await AddClass(db, "Soldier", s1, s2);
        var merchant = await AddBackground(db, "Merchant", false, b1);
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(soldier.Id);
        sc.SetScifiBackground(merchant.Id);
        sc = await Load(db, sc.Id);

        await Sync(db, sc);

        Assert.Equal(
            [(s1.Id, TalentSourceEnum.Class), (s2.Id, TalentSourceEnum.Class)],
            await Stored(db, sc.Id));
    }

    [Fact]
    public async Task Sync_ClassChanges_RemovesOldClassTalentsAndAddsNew() {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var m1 = await AddTalent(db, "Triage");
        var b1 = await AddTalent(db, "Haggler");
        var soldier = await AddClass(db, "Soldier", s1);
        var medic = await AddClass(db, "Medic", m1);
        var merchant = await AddBackground(db, "Merchant", false, b1);
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(soldier.Id);
        sc.SetScifiBackground(merchant.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);

        sc.SetScifiClass(medic.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);

        Assert.Equal([(m1.Id, TalentSourceEnum.Class)], await Stored(db, sc.Id));
    }

    [Fact]
    public async Task Sync_ClassChangesToMismatch_ClearsPickNoLongerInPool() {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var m1 = await AddTalent(db, "Triage");
        var b1 = await AddTalent(db, "Field Medicine");
        var soldier = await AddClass(db, "Soldier", s1);
        var medic = await AddClass(db, "Medic", m1);
        var medicBg = await AddBackground(db, "Medic", true, b1); // overlap
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(medic.Id); // matches background: pick from background's own talents
        sc.SetScifiBackground(medicBg.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);
        sc.AddTalent(b1, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        sc.SetScifiClass(soldier.Id); // mismatch: pool is now the Medic class's talents
        sc = await Load(db, sc.Id);
        await Sync(db, sc);

        Assert.Equal([(s1.Id, TalentSourceEnum.Class)], await Stored(db, sc.Id));
    }

    [Fact]
    public async Task Sync_ClassChanges_KeepsValidPickWhenBackgroundHasNoOverlap() {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var m1 = await AddTalent(db, "Triage");
        var b1 = await AddTalent(db, "Haggler");
        var soldier = await AddClass(db, "Soldier", s1);
        var medic = await AddClass(db, "Medic", m1);
        var merchant = await AddBackground(db, "Merchant", false, b1);
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(soldier.Id);
        sc.SetScifiBackground(merchant.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);
        sc.AddTalent(b1, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        sc.SetScifiClass(medic.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);

        Assert.Equal(
            [(m1.Id, TalentSourceEnum.Class), (b1.Id, TalentSourceEnum.Background)],
            await Stored(db, sc.Id));
    }

    [Fact]
    public async Task Sync_NewClassGrantsCurrentPick_ClearsPickWithoutDuplicateError() {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var shared = await AddTalent(db, "Haggler");
        var soldier = await AddClass(db, "Soldier", s1);
        var trader = await AddClass(db, "Trader", shared);
        var merchant = await AddBackground(db, "Merchant", false, shared);
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(soldier.Id);
        sc.SetScifiBackground(merchant.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);
        sc.AddTalent(shared, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        sc.SetScifiClass(trader.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);

        Assert.Equal([(shared.Id, TalentSourceEnum.Class)], await Stored(db, sc.Id));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Sync_ClassOrBackgroundCleared_ClearsBothSources(bool clearClass) {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var b1 = await AddTalent(db, "Haggler");
        var soldier = await AddClass(db, "Soldier", s1);
        var merchant = await AddBackground(db, "Merchant", false, b1);
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(soldier.Id);
        sc.SetScifiBackground(merchant.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);
        sc.AddTalent(b1, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        Assert.Equal(2, (await Stored(db, sc.Id)).Count);

        if (clearClass) sc.ClearScifiClass(); else sc.ClearScifiBackground();
        sc = await Load(db, sc.Id);
        await Sync(db, sc);

        Assert.Empty(await Stored(db, sc.Id));
    }

    [Fact]
    public async Task Sync_RunTwice_MakesNoChangesTheSecondTime() {
        await using var db = TestDb.Create();
        var s1 = await AddTalent(db, "Gunnery");
        var b1 = await AddTalent(db, "Haggler");
        var soldier = await AddClass(db, "Soldier", s1);
        var merchant = await AddBackground(db, "Merchant", false, b1);
        var sc = await AddScifiCharacter(db);
        sc.SetScifiClass(soldier.Id);
        sc.SetScifiBackground(merchant.Id);
        sc = await Load(db, sc.Id);
        await Sync(db, sc);
        sc.AddTalent(b1, TalentSourceEnum.Background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var before = await Stored(db, sc.Id);

        db.ChangeTracker.DetectChanges();
        await Sync(db, sc);
        db.ChangeTracker.DetectChanges();

        Assert.False(db.ChangeTracker.HasChanges());
        Assert.Equal(before, await Stored(db, sc.Id));
    }
}
