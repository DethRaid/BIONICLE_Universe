var mode : int = 1;
var power : int;
var targetMode : int = 1;

var playerHand : GameObject;
var waterBall : GameObject;
var rocketScript : rocket;

var getBigger : boolean = false;

var timer : float = 0.0;

var updateTimer : boolean = false;

var spawnedProjectile : GameObject;

enum MoveType {Time, Speed}

function Update () {
	//this code changes between the interact modes.
	
	if (Input.GetButtonDown("InteractMode")) {
			mode = 1;
	}
	
	if (Input.GetButtonDown("ElementalMode")) {
			mode = 2;
	}
	
	if (Input.GetButtonDown("CombatMode")) {
			mode = 3;
	}
	
	if (Input.GetButtonDown("MultiDown")) {
		targetMode = 2;
	}
	
	if (Input.GetButtonDown("AOFDown")) {
		targetMode = 3;
	}
	
	if (Input.GetButtonDown("SelfDown")) {
		targetMode = 4;
	}
	
	//most of the code below this line does the attacks or interacts 
	
	var ray : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	var hit : RaycastHit;
	
	switch (mode) {
		
		case (1):
		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast (ray, hit)) {
				if (Vector3.Distance(hit.point, transform.position) <= 5) {
				
					if (hit.collider.gameObject.GetComponent(BaseAI) != null) {
						print ("I have a ObjectAI component");
						//This is for stuff to interact with.
					}
				}
			}
		}
		break;
		
		case (2):
			switch (targetMode) {
			
			//start of projectile waterball attack
			
			case (1):
			if (Input.GetMouseButtonDown(0)) {
						
				spawnedProjectile = Instantiate(waterBall, playerHand.transform.position, transform.rotation);
				
				rocketScript = spawnedProjectile.GetComponent(rocket);
				rocketScript.EnlargeObjectToggle();
				//rocketScript.UnlockObjectToggle();
			}
			
			if (Input.GetMouseButtonUp (0)) {
			
				if (Physics.Raycast (ray, hit)) {
					rocketScript.UnlockObjectToggle();
					rocketScript.EnlargeObjectToggle();
					Translation(spawnedProjectile.transform, spawnedProjectile.transform.position, hit.point, 6, MoveType.Speed);
				}	
			}
			
			//end of projectile waterball attack
			
			break;
			
			/*case (2):
			
			//start of multi waterball attack
			
			if (Input.GetMouseButtonDown(0)) {
						
				spawnedProjectile = Instantiate(waterBall, playerHand.transform.position, transform.rotation);
				
				rocketScript.EnlargeObjectToggle();
				rocketScript.UnlockObjectToggle();
			}
			
			if (Input.GetMouseButtonUp (0)) {
			
				if (Physics.Raycast (ray, hit)) {
					if (Vector3.Distance(hit.point, transform.position) <= 50) {
						rocketScript.EnlargeObjectToggle();
						
						Translation(spawnedProjectile.transform, spawnedProjectile.transform.position, hit.point, 10, MoveType.Speed);
						spawnedProjectile.particleEmitter.emit = true;
						
						var effectedEntities : BaseAI[] = FindObjectsOfType (BaseAI);
						if (!Physics.Linecast (playerHand.transform.position, hit.point)) {
							if (hit.collider.gameObject.GetComponent(ObjectAI) != null) {
								print ("I have a ObjectAI component");
								//Do the damage to first target
							
								if(effectedEntities.length > 0) {
        							var closestEnemy = effectedEntities[0];
									var dist = Vector3.Distance(hit.point, effectedEntities[0].transform.position);

      								for(var i=0;i<effectedEntities.Length;i++) {
            							var tempDist = Vector3.Distance(hit.point, effectedEntities[i].transform.position);
            							if(tempDist < dist) {
                							closestEnemy = effectedEntities[i];
            							}
        							}

        									if (Vector3.Distance(spawnedProjectile.transform.position, closestEnemy.transform.position) <= 20) {
 												if (!Physics.Linecast (spawnedProjectile.transform.position, closestEnemy.transform.position)) {
        											Translation(spawnedProjectile.transform, spawnedProjectile.transform.position, closestEnemy.transform.position, 10, MoveType.Speed);
        											rocketScript.UnlockObjectToggle();
        										}
        									}
    							}
					
							}
							
							else {
								rocketScript.UnlockObjectToggle();
							}
						}
					}	
				}
			}
			
			//end of multi waterball attack
				
			break;
			
			case (3):
				
			//start of AOF waterball attack
				
			if (Input.GetMouseButtonDown(0)) {
						
				spawnedProjectile = Instantiate(waterBall, playerHand.transform.position, transform.rotation);
				
				rocketScript.EnlargeObjectToggle();
				rocketScript.UnlockObjectToggle();
			}
			
			if (Input.GetMouseButtonUp (0)) {
			
				if (Physics.Raycast (ray, hit)) {
					if (Vector3.Distance(hit.point, transform.position) <= 50) {
						rocketScript.EnlargeObjectToggle();
						rocketScript.UnlockObjectToggle();
						
						Translation(spawnedProjectile.transform, spawnedProjectile.transform.position, hit.point, 6, MoveType.Speed);
						if (!Physics.Linecast (playerHand.transform.position, hit.point)) {
							effectedEntities = FindObjectsOfType (BaseAI);
						
							for (i = 0; i < effectedEntities.Length; i++) {
								if (Vector3.Distance(effectedEntities[i].transform.position, hit.point) <= 5){
									//here is where I will deal the damage in the area, based on elemental strength.
								}
							}
						}
					}
				}	
			}
			
			//end of AOF waterball attack
			
			break;
			
			case (4):
				//self target
			break;*/
		}
	
	break;
	//end mode switch
	}
}

function Translation (thisTransform : Transform, endPos : Vector3, value : float, moveType : MoveType) {
    yield Translation (thisTransform, thisTransform.position, thisTransform.position + endPos, value, moveType);
}

function Translation (thisTransform : Transform, startPos : Vector3, endPos : Vector3, value : float, moveType : MoveType) {
    if (thisTransform != null) {
    	var rate = (moveType == MoveType.Time)? 1.0/value : 1.0/Vector3.Distance(startPos, endPos) * value;
    	var t = 0.0;
    	while (t < 1.0) {
       		t += Time.deltaTime * rate;
       		thisTransform.position = Vector3.Lerp(startPos, endPos, t);
        	yield; 
    	}
    }
}

function Rotation (thisTransform : Transform, degrees : Vector3, time : float) {
    var startRotation = thisTransform.rotation;
    var endRotation = thisTransform.rotation * Quaternion.Euler(degrees);
    var rate = 1.0/time;
    var t = 0.0;
    while (t < 1.0) {
        t += Time.deltaTime * rate;
        thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
        yield;
    }
}