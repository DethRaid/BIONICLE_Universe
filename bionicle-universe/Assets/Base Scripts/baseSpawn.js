import System.IO;

var spawnFileName = "SpawnData.budf";
var entityFileName = "EntityData.budf";

var spawnLength;

var Scene = "GAKORO";

var currentEntity;

function Start () {

	currentEntity = EntityStats();
    //this is the class that i made to contain all the entity data, you can find it in the classDefs.js file
		
	ReadSpawnData();    	
    
}

function ReadSpawnData () {
	var spawnSr = new StreamReader( "/Users/toshaantokhin/Dropbox/BNG RPG folder/BUData/" + spawnFileName);
    var spawnFileContents = spawnSr.ReadToEnd();
    //  ^^ that line takes all the contents of a file, and puts it into a string.
    spawnSr.Close();

    var spawnLines = spawnFileContents.Split("\n"[0]);
    //This is the split function, it basically splits a string into an array of strings based on a character you give it. Here, i gave it the newline character.
        
    for (i = 0; i < spawnLines.Length && spawnLines[i].Contains(Scene); i++) {
    //Thats the .contains function, it allows you to basically search a string to see if it contains a character or another string.
    	
    		var spawnLineContents = spawnLines[i].Split("="[0]);
    		print ("Spawning " + spawnLineContents[0]);
    		var spawnInfo = spawnLineContents[1].Split("|"[0]);
    		
    		currentEntity.entityID = spawnInfo[0];
    		currentEntity.spawnVector.x = float.Parse(spawnInfo[1]);
    		currentEntity.spawnVector.y = float.Parse(spawnInfo[2]);
    		currentEntity.spawnVector.z = float.Parse(spawnInfo[3]);
    		ReadEntityData();
    		
            //all i basically did above is take the array I had, and split it into a bunch of different, smaller arrays, and assigned them to values. Floatparse basically just turns a string into a float.
            
    		var spawnedObject : GameObject;
    		spawnedObject = Instantiate(Resources.Load(currentEntity.basePrefab), currentEntity.spawnVector, transform.rotation);
    		
   			var statsScript : BaseAI;
			spawnedObject.AddComponent(BaseAI);		
			
			spawnedObject.GetComponent(BaseAI).tempstats = currentEntity;
    	
	}
}

function ReadEntityData () {
	
    //this whole function basically does what the first one does, but on a larger scale, with more arrays and variables. It also calls on the RandomParse function i made alot.
    
	var entitySr = new StreamReader( "/Users/toshaantokhin/Dropbox/BNG RPG folder/BUData/" + entityFileName);
    var entityFileContents = entitySr.ReadToEnd();
    entitySr.Close();

    var entityLines = entityFileContents.Split("\n"[0]);
    
    for (i = 0; i < entityLines.Length && entityLines[i].Contains(currentEntity.entityID); i++) {
    	
    		var entityLineContents = entityLines[i].Split("="[0]);
    		var statSections = entityLineContents[1].Split(":"[0]);
    		
    		var strengthStats = statSections[0].Split("|"[0]);
    		
    		currentEntity.healthPoints = RandomParse(strengthStats[0]);
    		currentEntity.physicalStrength = RandomParse(strengthStats[1]);
    		currentEntity.physicalResistance = RandomParse(strengthStats[2]);
    		
    		var essenceStats = statSections[1].Split("|"[0]);
    		
    		currentEntity.essencePoints = RandomParse(essenceStats[0]);
    		currentEntity.essenceStrength = RandomParse(essenceStats[1]);
    		currentEntity.essenceResistance = RandomParse(essenceStats[2]);
    		
    		var agilityStats = statSections[2].Split("|"[0]);
    		
    		currentEntity.speed = RandomParse(agilityStats[0]);
    		currentEntity.hitChance = RandomParse(agilityStats[1]);
    		currentEntity.dodgeChance = RandomParse(agilityStats[2]);
    		
    		var miscStats = statSections[3].Split("|"[0]);
    		
    		currentEntity.basePrefab = miscStats[0];
    		currentEntity.entityType = RandomParse(miscStats[1]);
    		currentEntity.aiState = RandomParse(miscStats[2]);
	
	}

}


function RandomParse(inputText) {
    //this one just basically takes any values that have a / inbetween them and gets a random between them. I just use this for making enemies with differing stats.
	if (inputText.Contains("/")) {
    		var inputRand = inputText.Split("/"[0]);
    		var randOne = int.Parse(inputRand[0]);
    		var randTwo = int.Parse(inputRand[1]);
    		var randStat = Random.Range(randOne, randTwo);
    		return randStat;
    	}
    else {
    		return int.Parse(inputText);
	}
}