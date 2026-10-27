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
    public void AddTalent_Throws_WhenCharacterNotLoaded() {
        var sc = new ScifiCharacter(1);
        var talent = new ScifiTalent("Tough", "Description", "Ability");

        Assert.Throws<InvalidOperationException>(() => sc.AddTalent(talent, TalentSourceEnum.Class));
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

    [Fact]
    public async Task ClearTalentsBySource_Throws_ForBackground_WhenCharacterActive() {
        var (db, sc, _) = await SeedCharacterAndTalent(active: true);
        await using var _ = db;

        Assert.Throws<InvalidOperationException>(() => sc.ClearTalentsBySource(TalentSourceEnum.Background));
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
        return new ScifiTalentController(db, new PlayerContext(accessor));
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
    public async Task GetTalentPicks_OverlapMismatch_ReturnsBadRequest_WhenNoClassMatchesBackground() {
        await using var db = TestDb.Create();
        var soldier = new ScifiClass("Soldier", DiceType.D12);
        var background = new ScifiBackground("Hermit", "Lives alone", hasClassOverlap: true);
        db.ScifiClasses.Add(soldier);
        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sc = await AddScifiCharacter(db);
        await SetClassAndBackground(db, sc, soldier, background);

        var result = await CreateController(db).GetTalentPicks(sc.CharacterId);

        Assert.IsType<BadRequestObjectResult>(result);
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
}
