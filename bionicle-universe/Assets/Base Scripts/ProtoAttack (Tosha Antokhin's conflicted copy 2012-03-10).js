var prototype : GameObject;
var playerHand : GameObject;
var spawnedObject : GameObject;
var target : GameObject;
var attackObjectScript : AttackObject;

function Update () {
	
	var ray : Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	var hit : RaycastHit;
	
	if (Input.GetMouseButtonDown(0)) {
		if (Physics.Raycast (ray, hit)) {
			spawnedObject = Instantiate(prototype, transform.position, transform.rotation);
			attackObjectScript = spawnedObject.GetComponent(AttackObject);
			attackObjectScript.target = hit.point;
			attackObjectScript.path = ray;
			attackObjectScript.enlargeSize = true;
			}
			
			//Uncomment the following line to test the target of the script
			//Instantiate(target, attackObjectScript.target, transform.rotation);
	}
	
	if (Input.GetMouseButtonUp(0)) {
		attackObjectScript.enlargeSize = false;
		attackObjectScript.movementEnabled = true;
		attackObjectScript.collisionEnabled = true;
	}
}