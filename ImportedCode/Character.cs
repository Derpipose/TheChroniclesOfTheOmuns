using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDCharacterBuilder
{
    public class Character
    {
        public Character()
        {
            Name = "";
            Race = "";
            ClassName = "";
        }
        public string Name { get; private set; }
        public string Race { get; private set; }
        public string ClassName { get; private set; }

        public CharacterStats CharStats = new();
        public CharacterRace CharRace = new();
        public CharacterClass CharClass = new();

        public void SetName(string name)
        {
            Name = name;
        }
        public void SetRace(string race)
        {
            Race = CharRace.SetRace(race);
            CharStats.UpdateStats(CharRace, CharClass);
        }
        public void SetClass(string classinfo)
        {
            ClassName = CharClass.SetClass(classinfo);
            CharStats.UpdateStats(CharRace, CharClass);
        }

        public void SetStats(int score, string stat) {
            CharStats.SetStat(score, stat, CharRace);
            CharStats.UpdateStats(CharRace, CharClass);
        }

        public void LoadRace(string race) {
            Race = race;
        }

        public void LoadClass(string classname) {
            ClassName = classname;
        }
    }
}
