namespace PlayerApp.Models.Enums;

public enum ManaRuleEnum {
    Fixed = 0,
    Area = 1, //start at a set size and grow by a+1 x a+1 
    Volume = 2,  //start at a set size and grow by a+1 x a+1 x a+1 
    Unit = 3, //create or summon increases by a set amount per mana from the set start amount 
    Object = 4, //depends on the object being cast on 
    Mass = 5, //depends on the size and quantity of the object being cast on 
    Channeling = 6, //from base, increase power based on other spell level 
    SummonType = 7, //Mana varies on summon type 
    Content = 8 //Mana varies based on the contents of the spell (Like a bottle full of varying liquids)
}