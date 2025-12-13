using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDCharacterBuilder {
    public class CharacterClass {

        public string ClassType { private set; get; }
        public string ClassName { private set; get; }
        public string SpellCastingMod { private set; get; }
        public int HitDie { private set; get; } 
        public int ManaDie { private set; get; }
        public int MagicBooks { private set; get; }
        public int Cantrips { private set; get; }   
        public int Chances { private set; get; }
        public string StatFavor1 { private set; get; }
        public string StatFavor2 { private set; get; }
        public int Skills { private set; get; }
        private dynamic ClassArray;

        public CharacterClass() {
            HitDie = 0;
            ManaDie = 0;
            MagicBooks = 0;
            Cantrips = 0;
            Chances = 0;
            LoadClasses();
        }

        public void LoadClasses() {
            using (StreamReader r = new StreamReader("C:\\Users\\thefl\\source\\repos\\DnDCharacterBuilder\\DnDCharacterBuilder\\Classes.json")) {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);
                ClassArray = array;
            }
        }

        public string SetClass(string name) {
            foreach (var item in ClassArray) {
                if (item.ClassName == name) {
                    Console.WriteLine("Class set as " + name);
                    ClassName = name;
                    SpellCastingMod = item.SpellCastingModifier;
                    MagicBooks = item.MagicBooks;
                    Cantrips = item.Cantrips;
                    Chances = item.Chances;
                    HitDie = item.HitDie;
                    ManaDie = item.ManaDie;
                    ClassType = item.Classification;
                    StatFavor1 = item.StatFavor1;
                    StatFavor2 = item.StatFavor2;
                    Skills = item.ProficiencyCount;
                    return name;
                }
            }
            Console.WriteLine("Class not found. Please ensure proper spelling and capitalization and try again.");
            return "";
        }

        public void LoadClass(string name) {
            foreach (var item in ClassArray) {
                if (item.ClassName == name) {
                    ClassName = name;
                    SpellCastingMod = item.SpellCastingModifier;
                    MagicBooks = item.MagicBooks;
                    Cantrips = item.Cantrips;
                    Chances = item.Chances;
                    HitDie = item.HitDie;
                    ManaDie = item.ManaDie;
                    ClassType = item.Classification;
                    StatFavor1 = item.StatFavor1;
                    StatFavor2 = item.StatFavor2;
                    Skills = item.ProficiencyCount;
                    return;
                }
            }

        }


    }
}
