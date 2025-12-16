namespace ChroniclesTest;

using PlayerApp.Models;

public class Tests {
    private CharacterClass mageClass;
    [SetUp]
    public void Setup() {
        mageClass = new CharacterClass() {
            Name = "Mage",
            ClassType = "Magic",
            Description = "A master of arcane arts",
            HitDiceId = 2,
            HitDice = new DiceType { Id = 2, Name = "D6", Sides = 6 }
        };
    }

    // Let's do TDD!

    // step 2: Assign stats to the character 
    // step 3: Validate the stats
    // step 4: Ensure invalid stats are caught
    // step 5: Ensure valid stats are accepted
    // step 6: Assign a class to the character
    // step 7: Ensure class features are correct
    // step 8: Ensure Mana and Health are calculated correctly based on stats



    // step 1: Build a character
    [Test]
    public void CreateNewCharacter() {
        Character character = new();

        Assert.That(character, Is.TypeOf<Character>());
        Assert.Multiple(() => {
            Assert.That(character.Name, Is.EqualTo("Tav"));
            Assert.That(character.Level, Is.EqualTo(1));
        });

    }

    [Test]
    public void AddRaceToCharacter() {
        Character character = new();

        Assert.That(character.CharacterRace, Is.Null);

        CharacterRace tiefling = new() {
            Name = "Tiefling",
            Description = "A demonic race spawned from the Hells themselves. " +
            "According to popular playstyles, their skin can be any shade of red, " +
            "purple, or normal skin tones. Some even are blue"
        };

        character.CharacterRace = tiefling;

        Assert.Multiple(() => {
            Assert.That(character.CharacterRace.Name, Is.EqualTo("Tiefling"));
            Assert.That(character.CharacterRace.Description, Is.EqualTo(tiefling.Description));
        });
    }

    [Test]
    public void AddClassToCharacter() {
        Character character = new();

        Assert.That(character.CharacterClass, Is.Null);

        CharacterClass fighter = new() {
            Name = "Fighter",
            ClassType = "Combat",
            Description = "They pick up sword and go swingy swingy fast. Eventually fast enough to hit someone twice in a row :O",
            HitDiceId = 5,
            HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 }
        };

        character.AssignCharacterClass(fighter);

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(character.CharacterClass.Name, Is.EqualTo("Fighter"));
            Assert.That(character.CharacterClass.Description, Is.EqualTo(fighter.Description));
        });
    }

    [Test]
    public void AddStatsToCharacter() {
        Character character = new();

        Assert.That(character.Stats, Is.Not.Null);

        CharacterStats stats = new() {
            Strength = 15,
            Dexterity = 12,
            Constitution = 14,
            Intelligence = 10,
            Wisdom = 8,
            Charisma = 13
        };

        character.Stats = stats;

        Assert.Multiple(() => {
            Assert.That(character.Stats.Strength, Is.EqualTo(15));
            Assert.That(character.Stats.Dexterity, Is.EqualTo(12));
            Assert.That(character.Stats.Constitution, Is.EqualTo(14));
            Assert.That(character.Stats.Intelligence, Is.EqualTo(10));
            Assert.That(character.Stats.Wisdom, Is.EqualTo(8));
            Assert.That(character.Stats.Charisma, Is.EqualTo(13));
        });
    }


    [Test]
    public void CharacterStartsWithDefaultStatsIfNoneProvided() {
        Character character = new();

        Assert.That(character.Stats, Is.Not.Null);

        Assert.Multiple(() => {
            Assert.That(character.Stats.Strength, Is.EqualTo(10));
            Assert.That(character.Stats.Dexterity, Is.EqualTo(10));
            Assert.That(character.Stats.Constitution, Is.EqualTo(10));
            Assert.That(character.Stats.Intelligence, Is.EqualTo(10));
            Assert.That(character.Stats.Wisdom, Is.EqualTo(10));
            Assert.That(character.Stats.Charisma, Is.EqualTo(10));
        });
    }

    [Test]
    public void CharacterWithNoClassHasNoHitDie() {
        Character character = new();

        DiceType? hitDie = character.GetHitDice();

        Assert.That(hitDie, Is.Null);
    }

    [Test]
    public void CharacterWithNoClassHasNoManaDie() {
        Character character = new();

        DiceType? manaDie = character.GetManaDice();

        Assert.That(manaDie, Is.Null);
    }

    [Test]
    public void CharacterGetsCorrectBonusFromStats() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Strength = 18,
            Dexterity = 14,
            Constitution = 12,
            Intelligence = 10,
            Wisdom = 8,
            Charisma = 16
        };

        Assert.Multiple(() => {
            Assert.That(character.GetBonus("Strength"), Is.EqualTo(4));
            Assert.That(character.GetBonus("Dexterity"), Is.EqualTo(2));
            Assert.That(character.GetBonus("Constitution"), Is.EqualTo(1));
            Assert.That(character.GetBonus("Intelligence"), Is.EqualTo(0));
            Assert.That(character.GetBonus("Wisdom"), Is.EqualTo(-1));
            Assert.That(character.GetBonus("Charisma"), Is.EqualTo(3));
        });
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOne() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };

        character.AssignCharacterClass(mageClass);

        int expectedHP = 31; // (2 * 6) + 12 + 3
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOneForCombatClass() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };

        CharacterClass fighterClass = new() {
            Name = "Barbarian",
            ClassType = "Combat",
            Description = "A strong melee fighter",
            HitDiceId = 5,
            HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 }
        };

        character.AssignCharacterClass(fighterClass);

        int expectedHP = 47; // 2 * 16 + 12 + 3
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }
}

