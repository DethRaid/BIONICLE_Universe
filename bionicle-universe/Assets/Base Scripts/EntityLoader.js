import System.IO;
var fileName = "EntityData.budf";
var entityID = "002";

function Start () {
	var sr = new StreamReader( "/Users/toshaantokhin/Desktop/" + fileName);
    var fileContents = sr.ReadToEnd();
    sr.Close();

    var lines = fileContents.Split("\n"[0]);
    
    for (i = 0; i < lines.Length; i++) {
    	
    	if (lines[i].Contains(entityID))
    	{
    		var lineContents = lines[i].Split("="[0]);
    		var statSections = lineContents[1].Split(":"[0]);
    		var strengthStats = statSections[0].Split("|"[0]);
    		var essenceStats = statSections[1].Split("|"[0]);
    		var agilityStats = statSections[2].Split("|"[0]);
    		var baseModel = statSections[3];
    	}
    	
	}

}