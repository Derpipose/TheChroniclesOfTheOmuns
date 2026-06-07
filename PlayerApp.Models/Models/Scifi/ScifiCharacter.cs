using System;

namespace PlayerApp.Models;

public class ScifiCharacter {
    
    public int Id { get; private set; } = 0;
    public int CharacterId { get; private set; } = 0;
    public Character Character { get; private set; } 

    public ScifiCharacter(Character character) {
        if (character.Id == 0)
            throw new InvalidOperationException(
                "Character must be persisted before creating a ScifiCharacter.");
        CharacterId = character.Id;
        Character = character;
    }

    

}