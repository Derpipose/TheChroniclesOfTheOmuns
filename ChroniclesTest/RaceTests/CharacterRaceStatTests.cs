using PlayerApp.Models;
using PlayerApp.Models.Enums;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ChroniclesTest;

public class CharacterRaceBonusTests {

    CharacterService characterService = new CharacterService();

    [SetUp]
    public void Setup() {

    }

    [Test]
    public void ApplyRaceBonuses_WithFixedBonus_AppliesStatBonus() {
        // Arrange
        var race = new CharacterRace {
            Id = 1,
            Name = "Dwarf",
            RaceType = "Stout",
            Description = "Short and sturdy",
            RaceStatBonuses = new List<RaceStatBonus> {
                new() {
                    Id = 1,
                    RaceId = 1,
                    BonusValue = 2,
                    StatId = (int)StatsType.Constitution,
                    IsSelectable = false
                }
            }
        };

        var character = new Character {
            Name = "Gimli",
            CharacterStatBonuses = new List<CharacterStatBonus>()
        };

        //Assign Race to Character, applying bonuses will take place in the method
        characterService.UpdateCharacterRace(character, race);

        // Assert
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));

        var bonus = character.CharacterStatBonuses.First();

        Assert.Multiple(() => {
            Assert.That(bonus.StatId, Is.EqualTo((int)StatsType.Constitution));
            Assert.That(bonus.BonusValue, Is.EqualTo(2));
        });
    }

    // need test for remove race that removes racial bonuses
    [Test]
    public void RemoveRace_RemovesAppliedBonuses() {
        // Arrange
        var race = new CharacterRace {
            Id = 1,
            Name = "Dwarf",
            RaceType = "Stout",
            Description = "Short and sturdy",
            RaceStatBonuses = new List<RaceStatBonus> {
                new() {
                    Id = 1,
                    RaceId = 1,
                    BonusValue = 2,
                    StatId = (int)StatsType.Constitution,
                    IsSelectable = false
                }
            }
        };
        var character = new Character {
            Name = "Gimli",
            CharacterStatBonuses = new List<CharacterStatBonus>()
        };

        //Assign Race
        characterService.UpdateCharacterRace(character, race);
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));

        // Act
        characterService.RemoveCharacterRace(character);
        // Assert
        Assert.That(character.CharacterStatBonuses, Is.Empty);
    }

    // Need test for assigning race and then assigning a different race 
    // that removes old bonuses and applies new ones

    [Test]
    public void ChangeRace_UpdatesAppliedBonuses() {
        // Arrange
        var dwarfRace = new CharacterRace {
            Id = 1,
            Name = "Dwarf",
            RaceType = "Stout",
            Description = "Short and sturdy",
            RaceStatBonuses = new List<RaceStatBonus> {
                new() {
                    Id = 1,
                    RaceId = 1,
                    BonusValue = 2,
                    StatId = (int)StatsType.Constitution,
                    IsSelectable = false
                }
            }
        };

        var elfRace = new CharacterRace {
            Id = 2,
            Name = "Elf",
            RaceType = "Graceful",
            Description = "Tall and agile",
            RaceStatBonuses = new List<RaceStatBonus> {
                new() {
                    Id = 2,
                    RaceId = 2,
                    BonusValue = 2,
                    StatId = (int)StatsType.Dexterity,
                    IsSelectable = false
                }
            }
        };

        var character = new Character {
            Name = "Legolas",
            CharacterStatBonuses = new List<CharacterStatBonus>()
        };

        //Assign Dwarf Race
        characterService.UpdateCharacterRace(character, dwarfRace);
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));
        Assert.That(character.CharacterStatBonuses.First().StatId, Is.EqualTo((int)StatsType.Constitution));

        // Act - Change to Elf Race
        characterService.UpdateCharacterRace(character, elfRace);
        // Assert
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));
        var bonus = character.CharacterStatBonuses.First();
        Assert.Multiple(() => {
            Assert.That(bonus.StatId, Is.EqualTo((int)StatsType.Dexterity));
            Assert.That(bonus.BonusValue, Is.EqualTo(2));
        });
    }

    // MultiBonus Tests
    [Test]
    public void ApplyRaceBonuses_WithMultipleFixedBonuses_AppliesAllBonuses() {
        // Arrange
        var race = new CharacterRace {
            Id = 1,
            Name = "Orc",
            RaceType = "Brutish",
            Description = "Strong and fierce",
            RaceStatBonuses = new List<RaceStatBonus> {
                new() {
                    Id = 1,
                    RaceId = 1,
                    BonusValue = 2,
                    StatId = (int)StatsType.Strength,
                    IsSelectable = false
                },
                new() {
                    Id = 2,
                    RaceId = 1,
                    BonusValue = 1,
                    StatId = (int)StatsType.Constitution,
                    IsSelectable = false
                }
            }
        };
        var character = new Character {
            Name = "Gorg",
            CharacterStatBonuses = new List<CharacterStatBonus>()
        };
        //Assign
        characterService.UpdateCharacterRace(character, race);
        // Assert
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(2));
    }

    // [Test]
    // public void ApplyRaceBonuses_WithSelectableBonus_AppliesSelectedStat() {
    //     // Arrange
    //     var race = new CharacterRace {
    //         Id = 1,
    //         RaceStatBonuses = new List<RaceStatBonus> {
    //             new() {
    //                 Id = 2,
    //                 RaceId = 1,
    //                 BonusValue = 1,
    //                 StatId = null,
    //                 IsSelectable = true
    //             }
    //         }
    //     };

    //     var character = new Character {
    //         CharacterStatBonuses = new List<CharacterStatBonus>()
    //     };

    //     var selectedStats = new[] { StatsType.Intelligence };

    //     // Act
    //     character.ApplyRaceBonuses(race.RaceStatBonuses, selectedStats);

    //     // Assert
    //     Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));

    //     var bonus = character.CharacterStatBonuses.First();

    //     Assert.Multiple(() => {
    //         Assert.That(bonus.StatId, Is.EqualTo((int)StatsType.Intelligence));
    //         Assert.That(bonus.BonusValue, Is.EqualTo(1));
    //     });
    // }

    // [Test]
    // public void ApplyRaceBonuses_WithFixedAndSelectable_AppliesBoth() {
    //     // Arrange
    //     var race = new CharacterRace {
    //         Id = 1,
    //         RaceStatBonuses = new List<RaceStatBonus> {
    //             new() {
    //                 Id = 1,
    //                 RaceId = 1,
    //                 BonusValue = 2,
    //                 StatId = (int)StatsType.Dexterity,
    //                 IsSelectable = false
    //             },
    //             new() {
    //                 Id = 2,
    //                 RaceId = 1,
    //                 BonusValue = 1,
    //                 StatId = null,
    //                 IsSelectable = true
    //             }
    //         }
    //     };

    //     var character = new Character {
    //         CharacterStatBonuses = new List<CharacterStatBonus>()
    //     };

    //     var selectedStats = new[] { StatsType.Wisdom };

    //     // Act
    //     character.ApplyRaceBonuses(race.RaceStatBonuses, selectedStats);

    //     // Assert
    //     Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(2));

    //     var fixedBonus = character.CharacterStatBonuses.FirstOrDefault(b => b.StatId == (int)StatsType.Dexterity);
    //     var selectableBonus = character.CharacterStatBonuses.FirstOrDefault(b => b.StatId == (int)StatsType.Wisdom);

    //     Assert.Multiple(() => {
    //         Assert.That(fixedBonus, Is.Not.Null);
    //         Assert.That(fixedBonus!.BonusValue, Is.EqualTo(2));
    //         Assert.That(selectableBonus, Is.Not.Null);
    //         Assert.That(selectableBonus!.BonusValue, Is.EqualTo(1));
    //     });
    // }

    // [Test]
    // public void ApplyRaceBonuses_WithoutSelection_DoesNotApplySelectable() {
    //     // Arrange
    //     var race = new CharacterRace {
    //         Id = 1,
    //         RaceStatBonuses = new List<RaceStatBonus> {
    //             new() {
    //                 Id = 1,
    //                 RaceId = 1,
    //                 BonusValue = 1,
    //                 StatId = null,
    //                 IsSelectable = true
    //             }
    //         }
    //     };

    //     var character = new Character {
    //         CharacterStatBonuses = new List<CharacterStatBonus>()
    //     };

    //     // Act
    //     character.ApplyRaceBonuses(race.RaceStatBonuses, Enumerable.Empty<StatsType>());

    //     // Assert
    //     Assert.That(character.CharacterStatBonuses, Is.Empty);
    // }

    // [Test]
    // public void ApplyRaceBonuses_WithMultipleSelectable_ConsumesMultipleSelections() {
    //     // Arrange
    //     var race = new CharacterRace {
    //         Id = 1,
    //         RaceStatBonuses = new List<RaceStatBonus> {
    //             new() {
    //                 Id = 1,
    //                 RaceId = 1,
    //                 BonusValue = 1,
    //                 StatId = null,
    //                 IsSelectable = true
    //             },
    //             new() {
    //                 Id = 2,
    //                 RaceId = 1,
    //                 BonusValue = 1,
    //                 StatId = null,
    //                 IsSelectable = true
    //             }
    //         }
    //     };

    //     var character = new Character {
    //         CharacterStatBonuses = new List<CharacterStatBonus>()
    //     };

    //     var selectedStats = new[] {
    //         StatsType.Strength,
    //         StatsType.Charisma
    //     };

    //     var characterService = new CharacterService();

    //     // Act
    //     characterService.ApplyRaceBonuses(character, race.RaceStatBonuses, selectedStats);

    //     // Assert
    //     Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(2));

    //     var distinctStats = character.CharacterStatBonuses.Select(b => b.StatId).Distinct().Count();
    //     Assert.That(distinctStats, Is.EqualTo(2));
    // }
}
