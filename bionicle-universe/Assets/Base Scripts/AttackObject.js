#pragma strict

//enum ElementType {Water, Physical}

var collisionEffect : GameObject;
var targetObject : GameObject;

var target : Vector3;
var path : Ray;

var velocity : float = 1;
var basePower : float;
var longevity : int;
var lifetime : int;

var enlargeSize : boolean;
var reduceSize : boolean;
var movementEnabled : boolean;
var homingMovementEnabled : boolean;
var collisionEnabled : boolean;
var collided : boolean;
var lifeTimerEnabled : boolean;

var Objcollision : Collision;

var pathsphere : GameObject;

function  OnCollisionEnter(collision : Collision) {
	Objcollision = collision;
	collided = true;
}

function ObjectUpdate () {
	//object physics update code section
	
	if (movementEnabled) {

		if( Vector3.Distance(transform.position, target) > 0.2f ) {
			transform.position = Vector3.MoveTowards(transform.position, target, velocity);
			//Only un-comment below line if testing speed/path of projectiles.
			//Instantiate(pathsphere, transform.position, transform.rotation);
		}
		
	}
	
	if (homingMovementEnabled && targetObject != null) {

		if( Vector3.Distance(transform.position, target) > 0.2f ) {
			target = targetObject.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, target, velocity);
			//Only un-comment below line if testing speed/path of projectiles.
			//Instantiate(pathsphere, transform.position, transform.rotation);
		}
		
	}
	
	if (collisionEnabled && collided) {
		var contact : ContactPoint = Objcollision.contacts[0];
		var rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
		if (collisionEffect != null) {
			Instantiate (collisionEffect, contact.point, rotation);
		}
		Destroy (gameObject);
	}
	
	//additional functions update section
	
	if (enlargeSize) {
		transform.localScale.x = transform.localScale.x + basePower;
		transform.localScale.y = transform.localScale.y + basePower;
		transform.localScale.z = transform.localScale.z + basePower;
	}
	
	if (reduceSize) {
		transform.localScale.x = transform.localScale.x - basePower;
		transform.localScale.y = transform.localScale.y - basePower;
		transform.localScale.z = transform.localScale.z - basePower;
	}
	
	if (lifetime == longevity) {
		Instantiate (collisionEffect, transform.position, transform.rotation);
		Destroy (gameObject);
	}
	
}

function Update () {
	ObjectUpdate();
}

function Awake () {	
	TimerTick();
}

function TimerTick () {
	while(lifetime < longevity)
    {
        yield WaitForSeconds(1);
 
        lifetime++;
    }
}