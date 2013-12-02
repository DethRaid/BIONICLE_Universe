var explosion : GameObject;
var getBigger : boolean = false;
var unlockedToggle : boolean = false;
var enlargedToggle : boolean = false;

static var collisionIsEnabled : boolean = false;

var power : float = 0.0;
var timer : float = 0.0;
var updateTimer : boolean = false;

function Start () 
{
	this.gameObject.particleEmitter.emit = false;
}

function Update () {

	if (updateTimer) 
		timer += Time.deltaTime;
	
	if (getBigger) {
		if (power <= 5) {
			power = power * timer / 4;
		}
		transform.localScale.x = transform.localScale.x + power / 2;
		transform.localScale.y = transform.localScale.y + power / 2;
		transform.localScale.z = transform.localScale.z + power / 2;
	}
}


function OnCollisionEnter (collision : Collision) 
{
	if (collisionIsEnabled) {
		var contact : ContactPoint = collision.contacts[0];
		var rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
    	Instantiate (explosion, contact.point, rotation);
		Destroy(gameObject);  
	} 
}

function EnlargeObjectToggle() {
	if (enlargedToggle == false) {
		updateTimer = true;
		getBigger = true;
		//enlargedToggle = true;
	}
	
	if (enlargedToggle == true) {
		updateTimer = false;
		getBigger = false;
		power = 0;
		timer = 0;
		enlargedToggle = false;
	}
}

function UnlockObjectToggle() {
	if (!unlockedToggle) {
		collisionIsEnabled = true;
		this.gameObject.particleEmitter.emit = true;
		unlockedToggle = true;
	}
	
	else if (unlockedToggle) {
		collisionIsEnabled = false;
		this.gameObject.particleEmitter.emit = false;
		unlockedToggle = false;
	}
}