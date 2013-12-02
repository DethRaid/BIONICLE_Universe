public class AttackObj {
	
	enum ElementType {Water, Physical}
	
	var collisionEffect : GameObject;
	
	var target : RaycastHit;
	var path : Ray;
	
	var velocity : float;
	var basePower : float;
	var longevity : float;
	
	var startPos : Vector3;
	var endPos : Vector3;
	var rotationDegrees : Vector3;
	
	var enlargeSize : boolean;
	var reduceSize : boolean;
	var movementEnabled : boolean;
	var collisionEnabled : boolean;
	var collided : boolean;
	
	var collision : Collision;
	
	function ObjectUpdate () {
		//object physics update code section
		
		if (movementEnabled) {
			//later imma put some realistic physics stuff here.
			
			for (t = 0; t <10; t += Time.deltaTime * velocity)
			{
				var startRotation = this.Transform.rotation;
				var endRotation = this.Transform.rotation * Quaternion.Euler(rotationDegrees);
				
				startPos = this.Transform.position;
				endPos = target.point;
				
				this.Transform.position = Vector3.Lerp(startPos, endPos, t);
				this.Transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
				yield; 
			}
		}
		
		if (collisionEnabled && collided) {
			var contact : ContactPoint = collision.contacts[0];
			var rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
			if (collisionEffect != null) {
				Object.Instantiate (collisionEffect, contact.point, rotation);
			}
			Object.Destroy(this.GameObject);
		}
		
		//additional functions update section
		
		if (enlargeSize) {
			this.Transform.localScale.x = this.Transform.localScale.x + basePower;
			this.Transform.localScale.y = this.Transform.localScale.y + basePower;
			this.Transform.localScale.z = this.Transform.localScale.z + basePower;
		}
		
		if (reduceSize) {
			this.Transform.localScale.x = this.Transform.localScale.x - basePower;
			this.Transform.localScale.y = this.Transform.localScale.y - basePower;
			this.Transform.localScale.z = this.Transform.localScale.z - basePower;
		}
		
	}
	
}
