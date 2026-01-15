window.onload = function (){
    initializeEventListeners();
}

function initializeEventListeners() {
    const campaignSelect = document.getElementById("Campaign");
    const radioButtons = document.getElementsByName("select");
    const racetypes = document.getElementById("RaceType");
    const raceinfo = document.getElementById("RaceSelect");
    const classinfo = document.getElementById("Class");
    const createbutton = document.getElementById("CreateCharacter");
    const backbutton = document.getElementById("BackToBuild");
    const str = document.getElementById("Str");
    const dex = document.getElementById("Dex");
    const con = document.getElementById("Con");
    const wis = document.getElementById("Wis");
    const int = document.getElementById("Int");
    const cha = document.getElementById("Cha");
    
    
    campaignSelect.addEventListener("change", checkConditions);
    radioButtons.forEach(radio => {
        radio.addEventListener("change", checkConditions);
    });
    racetypes.addEventListener("change", RaceSpecific);
    raceinfo.addEventListener("change", raceInfo);
    classinfo.addEventListener("change", classInfo);
    createbutton.addEventListener("click", checkBuildConditions);
    backbutton.addEventListener("click", showAllBuilder);
    str.addEventListener("change", () => regulateStat("Str"));
    dex.addEventListener("change", () => regulateStat("Dex"));
    con.addEventListener("change", () => regulateStat("Con"));
    wis.addEventListener("change", () => regulateStat("Wis"));
    int.addEventListener("change", () => regulateStat("Int"));
    cha.addEventListener("change", () => regulateStat("Cha"));

    // Statcheck to ssee if they are at or above max, or below min either. 
    
    console.log("Event Listeners Ready to go!");
}

function checkConditions() {
    const campaignSelect = document.getElementById("Campaign");
    const selectedCampaign = campaignSelect.value;
    console.log(selectedCampaign);
    const isRadioSelected = document.querySelector('input[name="select"]:checked') !== null;
    console.log(isRadioSelected);
    if (selectedCampaign != "NOTCAMPAIGN" && isRadioSelected) {
        const selectedRadio = document.querySelector('input[name="select"]:checked');
        const playerStatus = selectedRadio.value;
        LoadRaces(selectedCampaign, playerStatus);
        LoadClasses(selectedCampaign, playerStatus);
        console.log("Conditions met, allowing access to main builder!");
        showLowerBuilder();
        hideRaces();
    }
}

function LoadRaces(Campaign, playerStatus){
    
    fetch(`https://derpipose.github.io/JsonFiles/RacesExpounded.json`)
    .then((result) => result.json() 
        .then((sheet) => {
            const RaceTypes = [];
            var myDiv = document.getElementById("RaceType");
            var racediv = document.getElementById("RaceSelect");
            var raceinfodiv = document.getElementById("RaceInfo");
            var classinfodiv = document.getElementById("ClassInfo");
            
            if(playerStatus == "Vet"){
                sheet.forEach(element=> {if(RaceTypes.includes(element.SubType)){}else if(element.Campaign == Campaign){RaceTypes.push(element.SubType);}});
                console.log("Added Veteran Capable races");
            }else{
                sheet.forEach(element=> {if(element.Starter == "Yes"){if(RaceTypes.includes(element.SubType)){}else if(element.Campaign == Campaign){RaceTypes.push(element.SubType);}}});
                console.log("Added Newby Capable races");
            }
            

            wipeDiv(myDiv);
            wipeDiv(racediv);
            clearDiv(raceinfodiv);
            wipeDiv(classinfodiv);
            clearStatSpans();
            
            

            RaceTypes.forEach(element =>{
                let option = document.createElement("option");                
                option.textContent = element;
                myDiv.appendChild(option);

            });

            

        })
    );

}

function RaceSpecific(){
    const raceTypeChosen = document.getElementById("RaceType");
    const SubRace = raceTypeChosen.value;
    const campaignSelect = document.getElementById("Campaign");
    const Campaign = campaignSelect.value;
    const selectedRadio = document.querySelector('input[name="select"]:checked');
    const playerStatus = selectedRadio.value;
    
    var raceDiv = document.getElementById("RaceInfo");
    clearDiv(raceDiv);
    
    fetch(`https://derpipose.github.io/JsonFiles/RacesExpounded.json`)
    .then((result) => result.json() 
        .then((sheet) => {
            const Races = [];
            var myDiv = document.getElementById("RaceSelect");
            
            if(playerStatus == "Vet"){
                sheet.forEach(element=> {if(Races.includes(element.Name)){}else if(element.Campaign == Campaign && element.SubType == SubRace){Races.push(element.Name);}});
                // console.log("Added " + SubRace + " races");
            }else{
                sheet.forEach(element=> {if(Races.includes(element.Name)){}else if(element.Campaign == Campaign && element.SubType == SubRace && element.Starter == "Yes"){Races.push(element.Name);}});
                // console.log("Added " + SubRace + " races");
            }
            

            console.log(Races);
            wipeDiv(myDiv);
            clearStatSpans();
            
            if(Races.length > 0){
                showRaces();
            }else{
                hideRaces();
            }

            Races.forEach(element =>{
                let option = document.createElement("option");                
                option.textContent = element;
                myDiv.appendChild(option);

            });

        })
    );

    
}

function raceInfo(){

    hideRadios();
    const raceChosen = document.getElementById("RaceSelect");
    const Race = raceChosen.value;
    const campaignChosen = document.getElementById("Campaign");
    const Campaign = campaignChosen.value;
    var AgeSpan = document.getElementById("age-range");
    var raceNameForAge = document.getElementById("chosenRace");
    var myDiv = document.getElementById("RaceInfo");
    var pick1div = document.getElementById("pick1-container");
    var pick2div = document.getElementById("pick2-container");
    var pick1 = "";
    var pick2 = "";
    

    
    fetch(`https://derpipose.github.io/JsonFiles/RacesExpounded.json`)
    .then((result) => result.json() 
        .then((sheet) => {
            sheet.forEach(element=> {if(element.Campaign == Campaign && element.Name == Race){
            
                clearDiv(myDiv);
                let Description = document.createElement("p");
                Description.textContent = "Description: " + element.Description;
                myDiv.appendChild(Description);

                if(element.Special != ""){
                    let Special = document.createElement("p");
                    Special.textContent = "Special: " + element.Special;
                    myDiv.appendChild(Special);
                }
                console.log("RaceInfo Clearing Statspan and radio");
                clearStatSpans();
                clearRadioButtons();
                
                var basestatchoices = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];
                
                var statchoices = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];
                basestatchoices.forEach(playerstat => {

                    statchoices = SetStat(playerstat, element[playerstat], statchoices);
                    if(element[playerstat] == 1){
                        pick1 = playerstat;
                        console.log("I set pick1");
                    }
                    if(element[playerstat] == 2){
                        pick2 = playerstat;
                        console.log("I set pick2");
                    }
                    if(element[playerstat] != 0){

                        let Picking = document.createElement("p");
                        if(element[playerstat] > 0){

                            Picking.textContent = "You get a +" + element[playerstat] +" stat bonus to: " + playerstat;
                        }else{
                            Picking.textContent = "You get a " + element[playerstat] +" stat bonus to: " + playerstat;

                        }
                        myDiv.appendChild(Picking);
                    }
                });
                
                
                if(element.Pick == "Both" || element.Pick == "Race"){
                    pick1 = "Player can choose";
                    pick2 = "Player can choose";
                    statchoices.forEach(element => {
                        let label = document.createElement("label");
                        label.innerText = element;
                        label.htmlFor = "pick1-" + element;

                        let input = document.createElement("input");
                        input.type = "radio";
                        input.name = "pick1";
                        input.id = "pick1-" + element;
                        input.value = element;

                        input.addEventListener("click", updatePicks);
                        pick1div.appendChild(input);
                        pick1div.appendChild(label);

                        var element = document.getElementById("pick1-container");
                        element.style.display = "block";
                        console.log("show pick 1");

                    });
                    statchoices.forEach(element => {
                        let label = document.createElement("label");
                        label.innerText = element;
                        label.htmlFor = "pick2-" + element;
                        let input = document.createElement("input");
                        input.type = "radio";
                        input.name = "pick2";
                        input.id = "pick2-" + element;
                        input.value = element;
                        input.addEventListener("click",updatePicks);
                        pick2div.appendChild(input);
                        pick2div.appendChild(label);

                        var element = document.getElementById("pick2-container");
                        element.style.display = "block";
                        console.log("show pick 2");

                    });

                }else if(element.Pick == 1){
                    pick1 = "Player can choose";
                    statchoices.forEach(element => {
                        let label = document.createElement("label");
                        label.innerText = element;
                        label.htmlFor = "pick1-" + element;

                        let input = document.createElement("input");
                        input.type = "radio";
                        input.name = "pick1";
                        input.id = "pick1-" + element;
                        input.value = element;

                        input.addEventListener("click", updatePicks);
                        pick1div.appendChild(input);
                        pick1div.appendChild(label);

                        var element = document.getElementById("pick1-container");
                        element.style.display = "block";
                        console.log("show pick 1");
                    });

                }else if(element.Pick == 2){
                    pick2 = "Player can choose";
                    statchoices.forEach(element => {
                        let label = document.createElement("label");
                        label.innerText = element;
                        label.htmlFor = "pick2-" + element;
                        let input = document.createElement("input");
                        input.type = "radio";
                        input.name = "pick2";
                        input.id = "pick2-" + element;
                        input.value = element;
                        input.addEventListener("click",updatePicks);
                        pick2div.appendChild(input);
                        pick2div.appendChild(label);

                        var element = document.getElementById("pick2-container");
                        element.style.display = "block";
                        console.log("show pick 2");
                    });
                }
                
                AgeSpan.textContent = element.AdventuringAgeStart + " - " + element.AgeMax;
                raceNameForAge.textContent = element.Name;

            }});

        })
    );

    

}

function LoadClasses(Campaign, playerStatus){

    myDiv = document.getElementById("Class");
    
    fetch(`https://derpipose.github.io/JsonFiles/ClassesExpounded.json`)
    .then((result) => result.json() 
        .then((sheet) => {
            const ClassList = [];
            if(Campaign == "Oriental"){
                sheet.forEach(element=> {if(ClassList.includes(element.ClassName)){}else if(element.Classification == "Eastern"){
                    ClassList.push(element.ClassName);
                }});
            } else{
                sheet.forEach(element=> {if(ClassList.includes(element.ClassName)){}else if(element.Classification != "Eastern" && element.Classification != "Scifi"){
                    if(playerStatus != "Vet" && element.Classification == "Veteran"){

                    }else{

                        ClassList.push(element.ClassName);
                    }
                }});
            }  


            wipeDiv(myDiv);
            var classtype = document.getElementById("ClassType");
            classtype.textContent = "";
            


            ClassList.forEach(element =>{
                let option = document.createElement("option");                
                option.textContent = element;
                myDiv.appendChild(option);

            });
        
            console.log("Got the " + Campaign + " classes!");

        })
    );
}

function classInfo(){
    const classChosen = document.getElementById("Class");
    const className = classChosen.value;
    var myDiv = document.getElementById("ClassInfo");

    
    fetch(`https://derpipose.github.io/JsonFiles/ClassesExpounded.json`)
    .then((result) => result.json() 
        .then((sheet) => {
            sheet.forEach(element=> {if(element.ClassName == className){
                
                wipeDiv(myDiv);

                let type = document.getElementById("ClassType");
                if(element.Classification == "Eastern"){
                    type.textContent = element.EasternClassType;
                    // myDiv.appendChild(type);
                }else if(element.Classification == "Veteran"){
                    // let type = document.createElement("p");
                    type.textContent = element.VeteranTag;
                    // myDiv.appendChild(type);
                }else{
                    // let type = document.createElement("p");
                    type.textContent = element.Classification;
                }
                // myDiv.appendChild(type);
                
                let HitDie = document.createElement("p");
                HitDie.textContent = "Hit Die: 1D" + element.HitDie;
                myDiv.appendChild(HitDie);
                let ManaDie = document.createElement("p");
                ManaDie.textContent = "Mana Die: 1D" + element.ManaDie;
                myDiv.appendChild(ManaDie);

                let Description = document.createElement("p");
                Description.textContent = "Description: " + element.Description;
                myDiv.appendChild(Description);

                console.log("Got a description and special if it has it");
            }});

        })
    );
}

function wipeDiv(div){
    while (div.children.length > 1) {
        div.removeChild(div.lastChild);
    }
}

function clearDiv(div){
    while(div.children.length > 0){
        div.removeChild(div.lastChild);
    }
}

function clearStatSpans(){
    var stats = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];
    stats.forEach(stat =>{
        var statadd = document.getElementById(stat + "-add");
        statadd.textContent = "";
        var statfinal = document.getElementById(stat + "-final")
        statfinal.textContent = "";
        var statprefer = document.getElementById(stat + "-prefer");
        statprefer.textContent = "";
        var statmax = document.getElementById(stat);
        statmax.max = 8;
    });

    var age = document.getElementById("age-range");
    var race = document.getElementById("chosenRace");

    
    age.textContent = "";
    race.textContent = "";
    console.log("StatSpan wiped");
}

function clearOnlyStats(){
    const raceChosen = document.getElementById("RaceSelect");
    const Race = raceChosen.value;
    const campaignChosen = document.getElementById("Campaign");
    const Campaign = campaignChosen.value;
    var StatsAvaliable = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];
    const StatsComplete = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];
    
    fetch(`https://derpipose.github.io/JsonFiles/RacesExpounded.json`)
    .then((result) => result.json() 
        .then((sheet) => {
            sheet.forEach(element=> {if(element.Campaign == Campaign && element.Name == Race){
                StatsComplete.forEach(stat => {
                    if(element[stat] != 0){
                        StatsAvaliable = SetStat(stat, element[stat], StatsAvaliable);
                    }
                });
            }})}));
    
    StatsAvaliable.forEach(stat => {
        var docstat = document.getElementById(stat + "-add");
        docstat.textContent = "";
        var docstatmax = document.getElementById(stat);
        docstatmax.max = 8;
    });

    console.log("StatOnlySpan wiped");
}

function clearRadioButtons(){
    hideRadios();
    var radio1 = document.getElementById("pick1-container");
    var radio2 = document.getElementById("pick2-container");
    wipeDiv(radio1);
    wipeDiv(radio2);
}

function updatePicks() {
    console.log("I have been picked!");
    clearOnlyStats();

    let pick1 = document.querySelector('input[name="pick1"]:checked') ? document.querySelector('input[name="pick1"]:checked').value : null;
    let pick2 = document.querySelector('input[name="pick2"]:checked') ? document.querySelector('input[name="pick2"]:checked').value : null;

    // Enable all options first
    let pick1Options = document.querySelectorAll('#pick1-container input[type="radio"]');
    let pick2Options = document.querySelectorAll('#pick2-container input[type="radio"]');

    pick1Options.forEach(option => option.disabled = false);
    pick2Options.forEach(option => option.disabled = false);

    

    // Disable the selected option in the opposite set
    if (pick1) {
        
        let pick2Element = document.querySelector(`#pick2-${pick1}`);
        if (pick2Element) {
            pick2Element.disabled = true;
        }
    }
    if (pick2) {
        
        let pick1Element = document.querySelector(`#pick1-${pick2}`);
        if (pick1Element) {
            pick1Element.disabled = true;
        }
    }
    
    var basestatchoices = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];


    if(pick1 !== null){
        
        basestatchoices = SetStat(pick1, 1, basestatchoices);
        regulateStat(pick1);
    }
    if(pick2 !== null){
        
        basestatchoices = SetStat(pick2, 2, basestatchoices);
        regulateStat(pick2);
    }
    var statsleft = basestatchoices;
    basestatchoices.forEach(element => {
        statsleft = SetStat(element, 0, statsleft);
        updateStatCalculations(element);

    });

    // updateStatCalculations();
}

function checkBuildConditions(){
    // console.log("I have been pushed.");
    const campaign = document.getElementById("Campaign");
    const race = document.getElementById("RaceSelect");
    const playerclass = document.getElementById("Class");
    const str = document.getElementById("Str");
    const dex = document.getElementById("Dex");
    const con = document.getElementById("Con");
    const int = document.getElementById("Int");
    const wis = document.getElementById("Wis");
    const cha = document.getElementById("Cha");
    var stats = false;

    if(str.value == "" || dex.value == "" || con.value == "" || int.value == "" || wis.value == "" || cha.value == "" || str.value == 0 || dex.value == 0 || con.value == 0 || int.value == 0 || wis.value == 0 || cha.value == 0){
        stats = false;
        console.log("There were blanks in the stats");
        showStatWarn();
    }else {
        console.log("Stats look good cap!");
        stats = true;
        hideStatWarn();
    }

    if(race.value != "NOTRACETYPE" && playerclass.value != "NOTCLASS" && stats){
        console.log("Character ready to build captain!!!");
        hideRaceWarn();
        hideClassWarn();
        BuildTheCharacter();
    }else{ 
        if(race.value == "NOTRACETYPE"){
            showRaceWarn();
            console.log("Race Missing.");

        }
        if(playerclass.value == "NOTCLASS"){
            showClassWarn();
            console.log("Class Missing.");

        }
        console.log("Something is missing. Will find out what later.");
    }
}

function SetStat(stat, num, statchoices){

    var StatInQuestion = document.getElementById(stat+"-add");
    var maxstat = document.getElementById(stat);
    if (num < 0){
        StatInQuestion.textContent = "- "+ Math.abs(num);
        maxstat.max = 8-num;
    }else if( num > 0){
        StatInQuestion.textContent = "+ " + num;
        maxstat.max = 8-num;
    } else {
        StatInQuestion.textContent = "+ " + num;
    }
    if(num != 0){

        var index = statchoices.indexOf(stat);
        if (index !== -1) {
            statchoices.splice(index, 1);
        }
    }
    updateStatCalculations(stat);
    return statchoices;
}

function updateStatCalculations(Stat){
    if(document.getElementById(Stat)){

        const statValue = parseInt(document.getElementById(Stat).value, 10) || 0;
        
        // Get the stat-add value and convert to an integer
        const statAddText = document.getElementById(Stat + '-add').textContent.trim();
        const statAddValue = parseInt(statAddText.replace(/[^\d-]/g, ''), 10) || 0;

        // Calculate the final value
        const finalValue = statValue + statAddValue;
        
        // Update the stat-final span
        document.getElementById(Stat + '-final').textContent = ` = ${finalValue}`;
    }
    
}

function regulateStat(Stat){
    var workingstat = document.getElementById(Stat);
    var max = parseFloat(workingstat.max); // Convert max to a number
    var min = parseFloat(workingstat.min); // Convert min to a number
    var value = parseFloat(workingstat.value); // Convert value to a number
    
    if(value > max){
        workingstat.value = max;
        console.log("The number was too big. Sorry. Gotta nerf you just a bit. There, now you are good to go.");
    }else if(value < min){
        workingstat.value = min; 
        console.log("Chaotic stupid isn't an alignment.");
    }
    updateStatCalculations(Stat);
    
    // Check if all stats are filled in
    const str = document.getElementById("Str");
    const dex = document.getElementById("Dex");
    const con = document.getElementById("Con");
    const int = document.getElementById("Int");
    const wis = document.getElementById("Wis");
    const cha = document.getElementById("Cha");
    
    if(str.value != "" && str.value != 0 && dex.value != "" && dex.value != 0 && con.value != "" && con.value != 0 && int.value != "" && int.value != 0 && wis.value != "" && wis.value != 0 && cha.value != "" && cha.value != 0){
        hideStatWarn();
    } else {
        showStatWarn();
    }
}

function wipeCharacter(span){
    document.getElementById(span).textContent = '';
}

function BuildTheCharacter(){
    const spanIDs = [
        "Charname-show", "PlayerName-show", "CharClass-show", "Race-show", "Books-show", "Speed-show", "Languages-show", "Hit-Die-show", "Mana-Die-show", "Skills-show", "Str-show", "Con-show", "Dex-show", "Int-show", "Wis-show", "Cha-show", "Initive-show", "AC-show", "Health-show", "Mana-show"
    ];
    spanIDs.forEach(element => {
        wipeCharacter(element);
    });

    document.getElementById("Charname-show").textContent = document.getElementById("CharacterName").value;
    document.getElementById("PlayerName-show").textContent = document.getElementById("PlayerName").value;
    document.getElementById("CharClass-show").textContent = document.getElementById("Class").value;

    var classtype = document.getElementById("ClassType").value;
    var race = document.getElementById("RaceSelect").value;

    // var intelegence = document.getElementById("Int-final").value;

    var intelegencetext = document.getElementById('Int-final').textContent.trim();
    var intelegence = parseInt(intelegencetext.replace(/[^\d-]/g, ''), 10) || 0;
    intelegence = intelegence + 10;

    var Constitutiontext = document.getElementById('Con-final').textContent.trim();
    var constitution = parseInt(Constitutiontext.replace(/[^\d-]/g, ''), 10) || 0;
    constitution = constitution + 10;

    var WisdomText = document.getElementById('Wis-final').textContent.trim();
    var wisdom = parseInt(WisdomText.replace(/[^\d-]/g, ''), 10) || 0;
    wisdom = wisdom + 10;

    var health = 0;
    var mana = 0;
    var manadie = 0;
    var hitdie = 0;
    var manabonus = 0;
    var manamod = "Add";

    //getting class stuff
    fetch(`https://derpipose.github.io/JsonFiles/ClassesExpounded.json`)
        .then((result) => result.json())
            .then((sheet) => {
                sheet.forEach(element => {
                    if (element.ClassName == document.getElementById("Class").value) {
                        console.log("You are a(n) " + element.ClassName);
                        document.getElementById("Books-show").textContent = element.MagicBooks;
                        document.getElementById("Languages-show").textContent = element.LanguageCount;
                        document.getElementById("Hit-Die-show").textContent = element.HitDie;
                        document.getElementById("Mana-Die-show").textContent = element.ManaDie;
                        document.getElementById("Skills-show").textContent = element.ProficiencyCount;

                        manadie = element.ManaDie;
                        hitdie = element.HitDie;
                    }
                });

        return fetch(`https://derpipose.github.io/JsonFiles/RacesExpounded.json`);
    })
    .then((result) => result.json())
        .then((sheet) => {
            sheet.forEach(element => {
                if (element.Name == race && element.Campaign == document.getElementById("Campaign").value) {
                    console.log("You are a(n) " + element.Name);
                    document.getElementById("Race-show").textContent = element.Name;
                    document.getElementById("Speed-show").textContent = element.Speed;

                    if (element.AddOrMultMana != "") {
                        manamod = element.AddOrMultMana;
                        manabonus = element.BonusMana;
                    }
                }
            });

        console.log("Manadie = " + manadie);
        console.log("Intelegece = " + intelegence);
        console.log("wisdom = " + wisdom);
        console.log("manabonus = " + manabonus);
        console.log("manamod = " + manamod);
        
        // Calculate health
        if (classtype == "Combat") {
            health = 2 * constitution + 10 + hitdie + Math.floor((constitution - 10) / 2);
        } else {
            health = 2 * hitdie + constitution + 10 + Math.floor((constitution - 10) / 2);
        }

        // Calculate mana
        if (classtype == "Magic") {
            console.log ("Magic class selected");
            if (manamod != "Mult") {
                if (intelegence <= wisdom) {
                    mana = intelegence + wisdom + Math.floor((wisdom - 10) / 2) + manabonus + manadie;
                } else {
                    mana = intelegence + wisdom + Math.floor((intelegence - 10) / 2) + manabonus + manadie;
                }
            } else {
                if (intelegence <= wisdom) {
                    mana = intelegence + (wisdom * manabonus) + Math.floor((wisdom - 10) / 2) + manadie;
                } else {
                    mana = (intelegence * manabonus) + wisdom + Math.floor((intelegence - 10) / 2) + manadie;
                }
            }
        } else {
            console.log ("Not Magic class selected");

            if (manamod == "Mult") {
                if (intelegence <= wisdom) {
                    mana = (wisdom * manabonus) + (2 * manadie) + Math.floor((wisdom - 10) / 2);
                } else {
                    mana = (intelegence * manabonus) + (2 * manadie) + Math.floor((intelegence - 10) / 2);
                }
            } else {
                if (intelegence <= wisdom) {
                    mana = wisdom + 2 * manadie + Math.floor((wisdom - 10) / 2) + manabonus;
                } else {
                    mana = intelegence + 2 * manadie + Math.floor((intelegence - 10) / 2) + manabonus;
                }
            }
        }

        document.getElementById("Health-show").textContent = health;
        document.getElementById("Mana-show").textContent = mana;
        
        // Get the final Dex value (including racial bonuses)
        var dexText = document.getElementById('Dex-final').textContent.trim();
        var finalDex = parseInt(dexText.replace(/[^\d-]/g, ''), 10) || 0;
        finalDex = finalDex + 10; // Convert to D&D stat format
        var dexModifier = Math.floor((finalDex - 10) / 2);
        
        document.getElementById("Initive-show").textContent = "+ " + dexModifier;
        document.getElementById("AC-show").textContent = (dexModifier + 10);

        var basestatchoices = ["Str", "Dex", "Con", "Int", "Wis", "Cha"];
        basestatchoices.forEach(Stat => {
            var statText = document.getElementById(Stat + '-final').textContent.trim();
            var statfinalValue = parseInt(statText.replace(/[^\d-]/g, ''), 10) || 0;
            var bonus = Math.floor(statfinalValue / 2);
            statfinalValue = statfinalValue + 10;
            document.getElementById(Stat + '-show').textContent = statfinalValue + "(+" + bonus + ")";
        });

        hideAllBuilder();
    })
    .catch(error => {
        console.error('Error:', error);
    });
}

function showLowerBuilder(){

    var element = document.getElementById("CampaignSelected");
    element.style.display = "block";
}

function hideAllBuilder(){
    // console.log("Attempting to hide the builder.");
    var element = document.getElementById("BuildCharacter");
    element.style.display = "none";
    var element = document.getElementById("FinishedCharacter");
    element.style.display = "block";
}

function showAllBuilder(){
    var element = document.getElementById("BuildCharacter");
    element.style.display = "block";
    var element = document.getElementById("FinishedCharacter");
    element.style.display = "none";
}

function showRaces(){
    var element = document.getElementById("RaceSelector");
    element.style.display = "block";
}

function hideRaces(){
    var element = document.getElementById("RaceSelector");
    element.style.display = "none";
}

function showStatWarn(){
    var element = document.getElementById("WarnStats");
    element.style.display = "block";
}

function hideStatWarn(){
    var element = document.getElementById("WarnStats");
    element.style.display = "none";
}


function showRaceWarn(){
    var element = document.getElementById("WarnRace");
    element.style.display = "block";
}

function hideRaceWarn(){
    var element = document.getElementById("WarnRace");
    element.style.display = "none";
}
function showClassWarn(){
    var element = document.getElementById("WarnClass");
    element.style.display = "block";
}

function hideClassWarn(){
    var element = document.getElementById("WarnClass");
    element.style.display = "none";
}

function hideRadios(){
    var element = document.getElementById("pick1-container");
    element.style.display = "none";
    var element = document.getElementById("pick2-container");
    element.style.display = "none";
}