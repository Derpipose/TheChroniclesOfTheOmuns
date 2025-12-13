using static System.Formats.Asn1.AsnWriter;

namespace DnDCharacterBuilder
{
    public class CharacterStats {

        public int Strength { private set; get; }
        public int Constitution { private set; get; }
        public int Dexterity { private set; get; }
        public int Intelligence { private set; get; }
        public int Wisdom { private set; get; }
        public int Charisma { private set; get; }
        public int BaseStrength { private set; get; }
        public int BaseConstitution { private set; get; }
        public int BaseDexterity { private set; get; }
        public int BaseIntelligence { private set; get; }
        public int BaseWisdom { private set; get; }
        public int BaseCharisma { private set; get; }

        public int Health { private set; get; }
        public int Mana { private set; get; }


        public CharacterStats() {
            Strength = 0;
            Intelligence = 0;
            Constitution = 0;
            Dexterity = 0;
            Wisdom = 0;
            Charisma = 0;

            BaseStrength = 0;
            BaseIntelligence = 0;
            BaseConstitution = 0;
            BaseDexterity = 0;
            BaseWisdom = 0;
            BaseCharisma = 0;
        }

        public void SetStat(int score, string stat, CharacterRace race) {
            if (stat == "Strength" || stat == "Str" || stat == "str") {
                BaseStrength = score;
                Strength = BaseStrength + race.RaceStrength;
            } else
            if (stat == "Intelligence" || stat == "Int" || stat == "int") {
                BaseIntelligence = score;
                Intelligence = BaseIntelligence + race.RaceIntelligence;
            } else
            if (stat == "Wisdom" || stat == "Wis" || stat == "wis") {
                BaseWisdom = score;
                Wisdom = BaseWisdom + race.RaceWisdom;
            } else
            if (stat == "Dexterity" || stat == "Dex" || stat == "dex") {
                BaseDexterity = score;
                Dexterity = BaseDexterity + race.RaceDexterity;
            } else
            if (stat == "Constitution" || stat == "Con" || stat == "con") {
                BaseConstitution = score;
                Constitution = BaseConstitution + race.RaceConstitution;
            } else
            if (stat == "Charisma" || stat == "Cha" || stat == "cha") {
                BaseCharisma = score;
                Charisma = BaseCharisma + race.RaceCharisma;
            } else { Console.WriteLine("Please ensure proper spelling or abreviation and try again. Press enter to continue");
                Console.ReadLine();
            }
        }

        public void UpdateStats(CharacterRace race, CharacterClass charClass) {
            Strength = BaseStrength + race.RaceStrength;
            Constitution = BaseConstitution + race.RaceConstitution;
            Dexterity = BaseDexterity + race.RaceDexterity;
            Intelligence = BaseIntelligence + race.RaceIntelligence;
            Wisdom = BaseWisdom + race.RaceWisdom;
            Charisma = BaseCharisma + race.RaceCharisma;
            if (charClass.ClassType == "Combat") {
                Health = (2 * Constitution + charClass.HitDie + GetBonus("Constitution"));
            } 
            else {
                Health = ((2 * charClass.HitDie) + Constitution + GetBonus("Constitution"));
            }
            if(charClass.ClassType == "Magic"){
                if (race.AddOrMultMana != "Mult") {
                    if (Intelligence <= Wisdom) {
                        Mana = Intelligence + Wisdom + GetBonus("Wisdom") + race.BonusMana + charClass.ManaDie;
                    } else {
                        Mana = Intelligence + Wisdom + GetBonus("Intelligence") + race.BonusMana + charClass.ManaDie;
                    }
                } else {
                    if (Intelligence <= Wisdom) {
                        Mana = (Wisdom * race.BonusMana) + Intelligence + charClass.ManaDie + GetBonus("Wisdom");
                    } else {
                        Mana = (Intelligence * race.BonusMana) + Wisdom + charClass.ManaDie + GetBonus("Intelligence");
                    }
                }
            } else {
                if (race.AddOrMultMana == "Mult") {
                    if (Intelligence <= Wisdom) {
                        Mana = (Wisdom * race.BonusMana + (2 * charClass.ManaDie) + GetBonus("Wisdom"));
                    } else {
                        Mana = (Intelligence * race.BonusMana + (2 * charClass.ManaDie) + GetBonus("Intelligence"));
                    }
                } else {
                    if (Intelligence <= Wisdom) {
                        Mana = (Wisdom + (2 * charClass.ManaDie) + GetBonus("Wisdom") + race.BonusMana);
                    } else {
                        Mana = (Intelligence + (2 * charClass.ManaDie) + GetBonus("Intelligence") + race.BonusMana);
                    }
                }
            }
        }

        public int GetBonus(string stat) {
            int checker = 0;
            int bonus = 0;
            if (stat == "Strength") {
                checker = Strength;
            } else if (stat == "Intelligence") {
                checker = Intelligence;
            } else if (stat == "Wisdom") {
                checker = Wisdom;
            } else if (stat == "Dexterity") {
                checker = Dexterity;
            } else if (stat == "Constitution") {
                checker = Constitution;
            } else if (stat == "Charisma") {
                checker = Charisma;
            }


            if (checker <= 3) {
                bonus = -4;
            } else if (checker == 4 || checker == 5) {
                bonus = -3;
            } else if (checker == 6 || checker == 7) {
                bonus = -2;
            } else if (checker <= 8 || checker == 9) {
                bonus = -1;
            } else if (checker == 10 || checker == 11) {
                bonus = 0;
            } else if (checker == 12 || checker == 13) {
                bonus = 1;
            } else if (checker == 14 || checker == 15) {
                bonus = 2;
            } else if (checker == 16 || checker == 17) {
                bonus = 3;
            } else if (checker == 18 || checker == 19) {
                bonus = 4;
            } else if (checker == 20 || checker == 21) {
                bonus = 5;
            }
            return bonus;
        }

        public void LoadStats(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma) {
            BaseStrength = strength;
            BaseDexterity = dexterity;
            BaseConstitution = constitution;
            BaseIntelligence = intelligence;
            BaseWisdom = wisdom;
            BaseCharisma = charisma;

        }
    }
}