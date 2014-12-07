import System.IO;

var abilityFileName = "AbilityData.budf";

var tempstats : EntityStats;

private var stats : EntityStats;

var damp = 5.0;

var rotateSpeed : float = 3.0;
var alive = true;

var canMove = 0;

var target : GameObject;

private var isAttacked = false;

var controller : CharacterController; 
var forward : Vector3; 

var dist;

function Start () {

	stats = tempstats;
	
	tempstats = null;
	
	controller = GetComponent(CharacterController);
	
	if (stats.aiState == 0) {
		alive = false;
	}
	
	if (stats.aiState == 2) {
		RandGo();
	}
	
}

function Update () {
	
	if (stats.aiState == 1) {
		animation.CrossFade("Attack");
	}
	
	if (stats.aiState == 2 && canMove == 1) {
		transform.Rotate(0, Random.Range (-10, 10), 0);
    
    	forward = transform.TransformDirection(Vector3.forward);
    	controller.SimpleMove(forward * stats.speed);
    	animation.CrossFade("Walkcycle");
	}
	
	if (stats.aiState == 3) {
		dist = Vector3.Distance(target.transform.position, transform.position);
		
		if (dist >= 3) {
			
			var rotate = Quaternion.LookRotation(Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * damp);
	
			forward = transform.TransformDirection(Vector3.forward);
    		controller.SimpleMove(forward * stats.speed);
    		animation.CrossFade("Walkcycle");
		}

	}
	
	if (isAttacked) {
		stats.aiState = 3;
	}
}

function RandGo () {
	while (alive) {
		yield WaitForSeconds (Random.Range (3, 8));
		canMove = 0;
		yield WaitForSeconds (Random.Range (3, 8));
    	canMove = 1;
	}
}

function Attack () {
	
	
	var hit : RaycastHit;
	if (Physics.Raycast (transform.position, Vector3.forward, hit, 15)) {
        var distanceToGround = hit.distance;
    }
}

function ReadAbilityData () {
	var abilitySr = new StreamReader( "/Users/toshaantokhin/Dropbox/BNG RPG folder/BUData/" + abilityFileName);
    var abilityFileContents = abilitySr.ReadToEnd();
    abilitySr.Close();
    
    var usableAbilities;
    
    var abilityLines = abilityFileContents.Split("\n"[0]);
    
    for (i = 0; i < abilityLines.Length; i++) {
    	
    }
}
@script RequireComponent(CharacterController)
