namespace ChroniclesTest;

using PlayerApp.Models;

public class Tests {
    [SetUp]
    public void Setup() {

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



}

