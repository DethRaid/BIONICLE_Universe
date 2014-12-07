import System.IO;
var fileName = "MetaData.budf";
private var internalKey = "nUw3WVnAfF66BgUGXa9U";
var statusMessage = "";

var mainSkin : GUISkin;
var showStatus : boolean = true;
var showButtons : boolean = false;
var showOptions : boolean = false;
var showOnCorrupted : boolean = false;

function Start () {
	var sr = new StreamReader( "/Users/toshaantokhin/Dropbox/BNG RPG folder/BUData/" + fileName);
    var fileContents = sr.ReadToEnd();
    sr.Close();

    var lines = fileContents.Split("\n"[0]);
    
    for (i = 0; i < lines.Length; i++) {
		
		var lineContents = lines[i].Split("="[0]);
    	
    	if (lineContents[0] == ("KEY"))
    	{
    		statusMessage = ("Key has been found! Checking key...");
			yield WaitForSeconds (2);
			if (lineContents[1] == (internalKey)) {
				statusMessage = ("Key validated!");
				yield WaitForSeconds (2);
			}
			else {
				statusMessage = ("Key not validated! :( Please check for file corruption or misplacement.");
				yield WaitForSeconds (2);
				showStatus = false;
				showOnCorrupted = true;
			}
    	}
    	
    	if (lineContents[0] == ("VERSION"))
    	{
    		statusMessage = ("Game version is " + lineContents[1]);
    		yield WaitForSeconds (2);
    	}
    	
    	if (lineContents[0] == ("VERAUTHOR"))
    	{
    		statusMessage = ("Version authors are " + lineContents[1]);
    		yield WaitForSeconds (2);
    	}
		
		if (lineContents[0] == ("INDEV"))
    	{
    		if (lineContents[1] == "true") {
				statusMessage = ("Indev controls activated.");
				yield WaitForSeconds (2);
			}
    	}
	}
		statusMessage = ("All data loaded successfully. MetaData reader script finished.");	
		yield WaitForSeconds (2);
		if (showOnCorrupted != true) {
			showStatus = false;
			showButtons = true;
		}
}

function OnGUI () {

	GUI.skin = mainSkin;
    
    GUI.BeginGroup (Rect (Screen.width / 2 - 120, Screen.height / 2 - 100, 500, 500));
                
    if (showStatus == true) {
    	GUI.Label (Rect (0, 100, 380, 50), statusMessage);
    }
        
    if (showButtons == true) {
    	if (GUI.Button (Rect (0, 20, 250, 40), "Play!")) {
    		Application.LoadLevel ("Ga-Koro");
    	}
    	
    	if (GUI.Button (Rect (0, 70, 250, 40), "Options")) {
    		showButtons = false;
    		showOptions = true;
    	}
    	
    	if (GUI.Button (Rect (0, 120, 250, 40), "Credits")) {
    		Application.LoadLevel ("Credits");
    	}
    	
    	if (GUI.Button (Rect (0, 170, 250, 40), "Quit")) {
    		Application.Quit();
    	}
    }
	
	if (showOnCorrupted == true) {
		if (GUI.Button (Rect (0, 120, 250, 40), "Restore Data")) {
    		Application.Quit();
    	}
		
		if (GUI.Button (Rect (0, 170, 250, 40), "Quit")) {
    		Application.Quit();
    	}
	}
    
    if (showOptions == true) {
    	GUI.Label (Rect (0, 40, 340, 50), "Nothing here yet :( ");
    	
    	if (GUI.Button (Rect (0, 70, 250, 40), "Back")) {
    		showButtons = true;
    		showOptions = false;
    	}
    }
    
    GUI.EndGroup ();
}