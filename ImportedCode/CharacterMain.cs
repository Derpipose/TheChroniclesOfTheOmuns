using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DnDCharacterBuilder {
    public class CharacterMain {

        public CharacterMain() {
            LoadCharacters();
        }

        private List<Character> characterlist = new List<Character>();

        public Character SetCharacter() {
            Console.Clear();
            int input = 32;
            string read = "initialized";
            while(read != "") {

                Console.WriteLine("This is all of the available Characters. Please select one. Or just hit enter to go back.");
                for (int i = 0; i < characterlist.Count; i++) {
                    string name = characterlist[i].Name;
                    Console.WriteLine(i + ": " + name);
                }
                read = Console.ReadLine();
                try {
                    input = Convert.ToInt32(read);
                } catch {
                    Console.WriteLine("Please enter a valid input.");
                }

                if (input < characterlist.Count) { 
                    return characterlist[input];
                }
            }
            return null;

        }

        public Character NewCharacter() {
            Console.Clear();
            Console.WriteLine("What would you like to name your new character? (Don't worry, you can change the name later) \n");
            string name = Console.ReadLine();
            if ( name != "") {
                Character character = new Character();
                character.SetName(name);
                characterlist.Add(character);
                return character;
            }
            return null;
        }

        public void EditStats(Character character) {
            string input = "initialize";
            int num;
            while (input != "") {
                Console.Clear();
                Console.WriteLine("Press enter to return to the previous page.");
                Console.WriteLine(character.Name + "'s current stats are:");
                if (character.CharClass.StatFavor1 == "Str") {
                    Console.WriteLine("Str: " + character.CharStats.BaseStrength + "  Class preferred highest");
                } else if (character.CharClass.StatFavor2 == "Str") {
                    Console.WriteLine("Str: " + character.CharStats.BaseStrength + "  Class preferred next highest");
                } else {
                    Console.WriteLine("Str: " + character.CharStats.BaseStrength);
                }

                if (character.CharClass.StatFavor1 == "Con") {
                    Console.WriteLine("Con: " + character.CharStats.BaseConstitution + "  Class preferred highest");
                } else if (character.CharClass.StatFavor2 == "Con") {
                    Console.WriteLine("Con: " + character.CharStats.BaseConstitution + "  Class preferred next highest");
                } else {
                    Console.WriteLine("Con: " + character.CharStats.BaseConstitution);
                }
                
                if (character.CharClass.StatFavor1 == "Dex") {
                    Console.WriteLine("Dex: " + character.CharStats.BaseDexterity + "  Class preferred highest");
                } else if (character.CharClass.StatFavor2 == "Dex") {

                    Console.WriteLine("Dex: " + character.CharStats.BaseDexterity + "  Class preferred next highest");
                } else {
                    Console.WriteLine("Dex: " + character.CharStats.BaseDexterity);
                }

                if (character.CharClass.StatFavor1 == "Int") {
                    Console.WriteLine("Int: " + character.CharStats.BaseIntelligence + "  Class preferred highest");
                } else if (character.CharClass.StatFavor2 == "Int") {

                    Console.WriteLine("Int: " + character.CharStats.BaseIntelligence + "  Class preferred next highest");
                } else {
                    Console.WriteLine("Int: " + character.CharStats.BaseIntelligence);
                }

                if (character.CharClass.StatFavor1 == "Wis") {
                    Console.WriteLine("Wis: " + character.CharStats.BaseWisdom + "  Class preferred highest");
                } else if (character.CharClass.StatFavor2 == "Wis") {

                    Console.WriteLine("Wis: " + character.CharStats.BaseWisdom + "  Class preferred next highest");
                } else {
                    Console.WriteLine("Wis: " + character.CharStats.BaseWisdom);
                }

                if (character.CharClass.StatFavor1 == "Cha") {
                    Console.WriteLine("Cha: " + character.CharStats.BaseCharisma + "  Class preferred highest");
                } else if (character.CharClass.StatFavor2 == "Cha") {

                    Console.WriteLine("Cha: " + character.CharStats.BaseCharisma + "  Class preferred next highest");
                } else {
                    Console.WriteLine("Cha: " + character.CharStats.BaseCharisma);
                }

                if (character.CharRace.RaceStrength != 0) { Console.WriteLine("Racial bonus to Strength of + " + character.CharRace.RaceStrength); }
                if (character.CharRace.RaceConstitution != 0) { Console.WriteLine("Racial bonus to Constitution of + " + character.CharRace.RaceConstitution); }
                if (character.CharRace.RaceDexterity != 0) { Console.WriteLine("Racial bonus to Dexterity of + " + character.CharRace.RaceDexterity); }
                if (character.CharRace.RaceIntelligence != 0) { Console.WriteLine("Racial bonus to Intelligence of + " + character.CharRace.RaceIntelligence); }
                if (character.CharRace.RaceWisdom != 0) { Console.WriteLine("Racial bonus to Wisdom of + " + character.CharRace.RaceWisdom); }
                if (character.CharRace.RaceCharisma != 0) { Console.WriteLine("Racial bonus to Charisma of + " + character.CharRace.RaceCharisma); }

                if (character.CharRace.Picks.Count > 0) { 
                    Console.WriteLine("You have " + character.CharRace.Picks.Count + " stats you can modify from your race. If you want to place your stat, please type 'RACE'");
                }

                Console.WriteLine("\n Please enter a stat you would like to edit");
                input = Console.ReadLine();
                if(input != "") {
                    if (input == "RACE" || input == "Race" || input == "race") { 
                        for(int i = 0; i  < character.CharRace.Picks.Count; i++) {
                            Console.WriteLine("Where would you like the + " + character.CharRace.Picks[i] + "?");
                            input = Console.ReadLine();
                            character.CharRace.SetRaceStat(character.CharRace.Picks[i], input);
                            Console.ReadLine();
                        }
                    } else { 
                        Console.WriteLine("\n What would you like the " + input + " stat to be?");
                        try {
                            num = Int32.Parse(Console.ReadLine());
                        } catch {
                            num = 0;
                        }

                        character.SetStats(num, input);
                    }
                }
            }
        }

        public void EditRace(Character character) {
            string input = "initialize";
            string race;
            while (input != "") {
            if (character.Race != "") {
                race = character.Race;
            } else { 
                race = "unchosen"; }
                Console.Clear();
                Console.WriteLine("Please enter a race from the website. Ensure spelling and capitalization is correct. Or hit enter to go back.");
                Console.WriteLine(character.Name + "'s race is currently: " + race + " " + character.CharRace.Variant);
                input = Console.ReadLine();
                if (input != "") { 
                    character.SetRace(input);
                    input = Console.ReadLine();
                }
            }

        }

        public void EditClass(Character character) {
            string input = "initialize";
            string charclass;
            while (input != "") {
                if (character.ClassName != "") {
                    charclass = character.ClassName;
                } else {
                    charclass = "unchosen";
            }
                Console.Clear();
                Console.WriteLine("Please enter a class from the website. Ensure spelling and capitalization is correct. Or hit enter to go back. ");
                Console.WriteLine(character.Name + "'s class is currently: " + charclass);
                input = Console.ReadLine();
                if(input != "") {

                character.SetClass(input);
                input = Console.ReadLine();
                }
            }

        }

        public void EditName(Character character) {
            string input = "initialized";
            while (input != "") {
                Console.Clear();
                Console.WriteLine("What would you like " + character.Name + "'s name to be instead?");
                input = Console.ReadLine();
                if (input != "") {
                    character.SetName(input);
                }
            }
        }

        public void DisplayCharacter(Character character) {
            Console.Clear();
            string input = "initialized";
            Console.WriteLine("Hit enter to go back");
            while(input != "") {

                Console.WriteLine(character.Name + "'s character sheet would look like this!");
                if(character.ClassName == "") {
                    Console.WriteLine("Class not set!");
                } else {
                    Console.WriteLine("Class: " + character.ClassName + "   Magic Books: " + character.CharClass.MagicBooks + 
                        " \nHit Die: D" + character.CharClass.HitDie + "   Mana Die: D" + character.CharClass.ManaDie + "    Skills: " + character.CharClass.Skills);
                }
                if(character.Race == "") {
                    Console.WriteLine("Race not set!");
                }else {
                    Console.WriteLine("Race: " + character.Race + "    Speed: " + character.CharRace.Speed + "    Given Language: "+  character.CharRace.GivenLanguage);
                }
                if(character.CharStats.Strength == 0 || character.CharStats.Constitution == 0 || character.CharStats.Dexterity == 0 ||
                    character.CharStats.Intelligence == 0 || character.CharStats.Wisdom == 0 || character.CharStats.Charisma == 0) {
                    Console.WriteLine(character.Name + "'s stats aren't done yet!");
                } else {
                    Console.WriteLine("Strength:  " + character.CharStats.Strength + "(+" + character.CharStats.GetBonus("Strength") + ")" + "   Constitution: " + character.CharStats.Constitution + "(+" + character.CharStats.GetBonus("Constitution") + ") \n" + 
                        "Dexterity: " + character.CharStats.Dexterity + "(+" + character.CharStats.GetBonus("Dexterity") + ")" + "   Intelligence:  " + character.CharStats.Intelligence + "(+" + character.CharStats.GetBonus("Intelligence") + ")\n" + 
                        "Wisdom:    " + character.CharStats.Wisdom + "(+" + character.CharStats.GetBonus("Wisdom") + ")" + "   Charisma:     " + character.CharStats.Charisma + "(+" + character.CharStats.GetBonus("Charisma") + ")");
                    Console.WriteLine("Initive:   +" + character.CharStats.GetBonus("Dexterity") + "       AC:   " + (10 + character.CharStats.GetBonus("Dexterity")));
                }
                if(character.ClassName == "" || character.Race == "" || (character.CharStats.Strength == 0 || character.CharStats.Constitution == 0 || character.CharStats.Dexterity == 0 ||
                    character.CharStats.Intelligence == 0 || character.CharStats.Wisdom == 0 || character.CharStats.Charisma == 0)) {
                    Console.WriteLine("Your character isn't complete yet, so your health and mana can't be generated yet!");
                } else {
                    Console.WriteLine("Health:    " + character.CharStats.Health + "       Mana: " + character.CharStats.Mana);
                    if(character.CharClass.ClassType == "Magic") {
                        Console.WriteLine(character.Name + " would start with either a Wand, an Orb or a Staff implement for spell casting.");
                    }
                    if (character.CharClass.ClassType == "Combat") {
                        Console.WriteLine(character.Name + " would start with a trophy trinket along with the standard trinket.");
                    }
                    if (character.CharClass.ClassType == "Utility") {
                        Console.WriteLine(character.Name + " would start with an extra standard trinket.");
                    }
                }

                Console.ReadLine();
                input = "";

            }
        }

        public Character DeleteCharacter(Character character) {
            Console.WriteLine("Are you sure you want to do this? There is NO way to undo this action! Type \"DELETE\" if you are SURE you want to do this. If you have changed your mind, hit enter to go back.");
            string input = Console.ReadLine();
            if (input == "DELETE") {
                for (int i = 0; i < characterlist.Count; i++) {
                    if (characterlist[i] == character) {
                        characterlist.RemoveAt(i);
                        return null;
                    }
                }
            }
            return character;
        }   

        public async void SaveCharacters() {
            string json = "[";
            for(int i =0; i < characterlist.Count(); i++) {

                string characterstring = "{";
                characterstring += "\"Name\": \"" + characterlist[i].Name + "\",\n";
                characterstring += "\"Setting\": \"" + characterlist[i].CharRace.Campaign + "\",\n";
                characterstring += "\"Class\": \"" + characterlist[i].ClassName + "\",\n";
                characterstring += "\"Race\": \"" + characterlist[i].Race + "\",\n";
                characterstring += "\"RaceVariant\": \"" + characterlist[i].CharRace.Variant + "\",\n";
                characterstring += "\"BaseStr\": " + characterlist[i].CharStats.BaseStrength + ",\n";
                characterstring += "\"BaseCon\": " + characterlist[i].CharStats.BaseConstitution + ",\n";
                characterstring += "\"BaseDex\": " + characterlist[i].CharStats.BaseDexterity + ",\n";
                characterstring += "\"BaseInt\": " + characterlist[i].CharStats.BaseIntelligence + ",\n";
                characterstring += "\"BaseWis\": " + characterlist[i].CharStats.BaseWisdom + ",\n";
                characterstring += "\"BaseCha\": " + characterlist[i].CharStats.BaseCharisma + ",\n";
                characterstring += "\"RaceStr\": " + characterlist[i].CharRace.RaceStrength + ",\n";
                characterstring += "\"RaceCon\": " + characterlist[i].CharRace.RaceConstitution + ",\n";
                characterstring += "\"RaceDex\": " + characterlist[i].CharRace.RaceDexterity + ",\n";
                characterstring += "\"RaceInt\": " + characterlist[i].CharRace.RaceIntelligence + ",\n";
                characterstring += "\"RaceWis\": " + characterlist[i].CharRace.RaceWisdom + ",\n";
                characterstring += "\"RaceCha\": " + characterlist[i].CharRace.RaceCharisma + ",\n";
                if (characterlist[i].CharRace.Picks.Count() == 0) {
                    characterstring += "\"Picks1\": 0,\n";
                    characterstring += "\"Picks2\": 0\n";
                } else if (characterlist[i].CharRace.Picks.Count() == 1) {
                    characterstring += "\"Picks1\": "  + characterlist[i].CharRace.Picks[0] + ",\n";
                    characterstring += "\"Picks2\": 0\n";
                } else {
                    characterstring += "\"Picks1\": " + characterlist[i].CharRace.Picks[0] + ",\n";
                    characterstring += "\"Picks2\": " + characterlist[i].CharRace.Picks[1] + "\n";
                }

                    characterstring += "},";
                json += characterstring;
            }
            json += "]";
            await File.WriteAllTextAsync("C:\\Users\\thefl\\source\\repos\\DnDCharacterBuilder\\DnDCharacterBuilder\\Characters.json", json);
        }

        public void LoadCharacters() {
            dynamic characterarray;
            using (StreamReader r = new StreamReader("C:\\Users\\thefl\\source\\repos\\DnDCharacterBuilder\\DnDCharacterBuilder\\Characters.json")) {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);
                characterarray = array;
            }
            if (characterarray != null) {
                foreach (var character in characterarray) {
                    Character newchar = new();
                    string name = character.Name;
                    newchar.SetName(name);
                    string setting = character.Setting;
                    newchar.CharRace.Campaign = setting;
                    string loadclass = character.Class;
                    newchar.CharClass.LoadClass(loadclass);
                    newchar.LoadClass(loadclass);
                    string race = character.Race;
                    string racevar = character.RaceVariant;
                    newchar.LoadRace(race);
                    newchar.CharRace.LoadRace(race, racevar);
                    int str = character.BaseStr;
                    int dex = character.BaseDex;
                    int con = character.BaseCon;
                    int inte = character.BaseInt;
                    int wis = character.BaseWis;
                    int cha = character.BaseCha;
                    newchar.CharStats.LoadStats(str, dex, con, inte, wis, cha);
                    
                    str = character.RaceStr;
                    dex = character.RaceDex;
                    con = character.RaceCon;
                    inte =character.RaceInt;
                    wis = character.RaceWis;
                    cha = character.RaceCha;
                    int pick1 = character.Picks1;
                    int pick2 = character.Picks2;
                    newchar.CharRace.LoadStats(str, dex, con, inte, wis, cha, pick1, pick2);
                    newchar.CharStats.UpdateStats(newchar.CharRace, newchar.CharClass);
                    characterlist.Add(newchar);
                }
            }
        }

        public void EditSetting(Character character) {
            string input = "initialize";
            string setting;
            while (input != "") {
                
                Console.Clear();
                Console.WriteLine("Please enter a setting to form your character. Ensure spelling is correct. Or hit enter to go back.");
                Console.WriteLine(character.Name + "'s setting is currently: " + character.CharRace.Campaign );
                input = Console.ReadLine();
                if (input != "") {
                    character.CharRace.EditSetting(input);
                    input  = Console.ReadLine();
                }
            }
        }

    }
}
