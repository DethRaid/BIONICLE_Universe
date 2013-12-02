#pragma strict
// Require a character controller to be attached to the same game object
@script RequireComponent(CharacterController)

public var idleAnimation : AnimationClip;
public var walkAnimation : AnimationClip;
public var runAnimation : AnimationClip;
public var jumpPoseAnimation : AnimationClip;
public var swimCycle : AnimationClip;

public var idleAnimationSpeed : float = 0.5;
public var walkMaxAnimationSpeed : float = 0.75;
public var trotMaxAnimationSpeed : float = 1.0;
public var runMaxAnimationSpeed : float = 1.0;
public var jumpAnimationSpeed : float = 1.15;
public var landAnimationSpeed : float = 1.0;
var swimming : GameObject;
var canWalk : boolean = true;

private var _animation : Animation;

enum State {
	Idle = 0,
	Walking = 1,
	Trotting = 2,
	Running = 3,
	Swimming = 4,
	Boosting = 5,
	Jumping = 6,
}

private var _characterState : State;

// The speed when walking
var walkSpeed = 2.0;
// after trotAfterSeconds of walking we trot with trotSpeed
var trotSpeed = 4.0;
// when pressing "Fire3" button (cmd) we start running
var runSpeed = 6.0;
var bouyancy = 0.01;
var water : Transform;
var swimmingParticles : ParticleSystem;
var inAirControlAcceleration = 3.0;
// How high do we jump when pressing jump and letting go immediately
var jumpHeight = 0.5;
// The gravity for the character
var gravity = 20.0;
// The gravity in controlled descent mode
var speedSmoothing = 10.0;
var rotateSpeed = 500.0;
var trotAfterSeconds = 3.0;
var canJump = true;
var underwater = false;

private var jumpRepeatTime = 0.5;
private var jumpTimeout = 0.15;
private var groundedTimeout = 0.25;
// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
private var lockCameraTimer = 0.0;
// The current move direction in x-z
private var moveDirection = Vector3.zero;
// The current vertical speed
private var verticalSpeed = 0.0;
// The current x-z move speed
private var moveSpeed = 0.0;
// The last collision flags returned from controller.Move
private var collisionFlags : CollisionFlags; 
// Are we jumping? (Initiated with jump button and not grounded yet)
private var jumping = false;
private var jumpingReachedApex = false;
// Are we moving backwards (This locks the camera to not do a 180 degree spin)
private var movingBack = false;
// Is the user pressing any keys?
private var isMoving = false;
// When did the user start walking (Used for going into trot after a while)
private var walkTimeStart = 0.0;
// Last time the jump button was clicked down
private var lastJumpButtonTime = -10.0;
// Last time we performed a jump
private var lastJumpTime = -1.0;
// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
private var lastJumpStartHeight = 0.0;
private var inAirVelocity = Vector3.zero;
private var lastGroundedTime = 0.0;
public var isControllable = true;

private var movement : Vector3;

var controller : CharacterController;

var cameraTransform : Transform;

function Awake ()
{
	Screen.showCursor = false;
	moveDirection = transform.TransformDirection(Vector3.forward);
	_animation = GetComponent(Animation);
	if(!_animation)
		Debug.Log("The character you would like to control doesn't have animations. Moving it might look weird.");

	if(!idleAnimation) {
		_animation = null;
		Debug.Log("No idle animation found. Turning off animations.");
	}
	if(!walkAnimation) {
		_animation = null;
		Debug.Log("No walk animation found. Turning off animations.");
	}
	if(!runAnimation) {
		_animation = null;
		Debug.Log("No run animation found. Turning off animations.");
	}
	if(!jumpPoseAnimation && canJump) {
		_animation = null;
		Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
	}
	controller = GetComponent( CharacterController );
	cameraTransform = Camera.main.transform;
}

function UpdateSmoothedMovementDirection ()
{
	var grounded;
	grounded = IsGrounded();
	if( underwater )
		grounded = false;
		
	var forward = cameraTransform.TransformDirection( Vector3.forward );
	if( !underwater )
		forward.y = 0;
	forward = forward.normalized;

	var right = Vector3(forward.z, 0, -forward.x);

	var v = Input.GetAxisRaw("Vertical");
	var h = Input.GetAxisRaw("Horizontal");

	if( v < -0.2 )
		movingBack = true;
	else
		movingBack = false;
	var wasMoving = isMoving;
	isMoving = Mathf.Abs( h ) > 0.1 || Mathf.Abs( v ) > 0.1;
	// Target direction relative to the camera
	var targetDirection = h * right + v * forward;
	// Grounded controls
	if( grounded && !underwater )
	{
		// Lock camera for short period when transitioning moving & standing still
		lockCameraTimer += Time.deltaTime;
		if( isMoving != wasMoving )
			lockCameraTimer = 0.0;
		if( targetDirection != Vector3.zero )
		{
			// If we are really slow, just snap to the target direction
			if( moveSpeed < walkSpeed * 0.9 && grounded )
				moveDirection = targetDirection.normalized;
			// Otherwise smoothly turn towards it
			else
			{
				moveDirection = Vector3.RotateTowards( moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000 );
				moveDirection = moveDirection.normalized;
			}
		}
		var curSmooth = speedSmoothing * Time.deltaTime;
		var targetSpeed = Mathf.Min( targetDirection.magnitude, 1.0 );
		_characterState = State.Idle;
		if( Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) )
		{
			targetSpeed *= runSpeed;
			_characterState = State.Running;
		}
		else if( Time.time - trotAfterSeconds > walkTimeStart )
		{
			targetSpeed *= trotSpeed;
			_characterState = State.Trotting;
		}
		else
		{
			targetSpeed *= walkSpeed;
			_characterState = State.Walking;
		}
		moveSpeed = Mathf.Lerp( moveSpeed, targetSpeed, curSmooth );
		if( moveSpeed < walkSpeed * 0.3 ) 
			walkTimeStart = Time.time;
	}
	else if( underwater )
	{
		lockCameraTimer += Time.deltaTime;
		if( isMoving != wasMoving )
			lockCameraTimer = 0.0;
		moveDirection = Vector3.RotateTowards( moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000 );
		moveDirection = moveDirection.normalized;
		var curSmuth = speedSmoothing * Time.deltaTime;
		var wantedSpeed = targetDirection.magnitude;
		_characterState = State.Idle;
		if( Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) )
		{
			wantedSpeed *= runSpeed;
			_characterState = State.Boosting;
		}
		else
		{
			wantedSpeed *= walkSpeed;
			_characterState = State.Swimming;
		}
		moveSpeed = Mathf.Lerp( moveSpeed, wantedSpeed, curSmuth );
	}
	// In air controls
	else
	{
		if( jumping )
			lockCameraTimer = 0.0;
		if( isMoving )
			inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
	}		
}

function ApplyJumping ()
{
	// Prevent jumping too fast after each other
	if( lastJumpTime + jumpRepeatTime > Time.time )
		return;
	if( IsGrounded() )
	{
		// Jump
		// - Only when pressing the button down
		// - With a timeout so you can press the button slightly before landing		
		if( canJump && Time.time < lastJumpButtonTime + jumpTimeout ) 
		{
			verticalSpeed = CalculateJumpVerticalSpeed( jumpHeight );
			SendMessage( "DidJump", SendMessageOptions.DontRequireReceiver );
		}
	}
}

function ApplyGravity ()
{
	if( isControllable )	// don't move player at all if not controllable.
	{
		// Apply gravity
		var jumpButton = Input.GetButton( "Jump" );
		// When we reach the apex of the jump we send out a message
		if( jumping && !jumpingReachedApex && verticalSpeed <= 0.0 )
		{
			jumpingReachedApex = true;
			SendMessage( "DidJumpReachApex", SendMessageOptions.DontRequireReceiver) ;
		}
		if( IsGrounded () )
			verticalSpeed = 0.0;
		else
			verticalSpeed -= gravity * Time.deltaTime;

	}
}

function CalculateJumpVerticalSpeed( targetJumpHeight : float ) : float
{
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt( 2 * targetJumpHeight * gravity );
}

function DidJump()
{
	jumping = true;
	jumpingReachedApex = false;
	lastJumpTime = Time.time;
	lastJumpStartHeight = transform.position.y;
	lastJumpButtonTime = -10;
	_characterState = State.Jumping;
}

function Update() 
{
	if( transform.position.y < water.position.y )
		underwater = true;
	else
		underwater = false;
	canJump = true;
	if( underwater )
		canJump = false;
	if (!isControllable)
		Input.ResetInputAxes();
	if( Input.GetButtonDown("Jump") && canJump )
		lastJumpButtonTime = Time.time;
	UpdateSmoothedMovementDirection();
	if( !underwater )
	{
		ApplyGravity();
		ApplyJumping();
	}
	if( !underwater )
		movement = moveDirection * moveSpeed + Vector3( 0, verticalSpeed, 0 ) + inAirVelocity;
	else
		movement = moveDirection * moveSpeed + Vector3( 0, bouyancy, 0 );
	movement *= Time.deltaTime;
	collisionFlags = controller.Move( movement );
	if( _animation ) 
	{
		if( _characterState == State.Jumping ) 
		{
			if( !jumpingReachedApex ) 
			{
				_animation[ jumpPoseAnimation.name ].speed = jumpAnimationSpeed;
				_animation[ jumpPoseAnimation.name ].wrapMode = WrapMode.ClampForever;
				_animation.CrossFade( jumpPoseAnimation.name );
			} 
			else 
			{
				_animation[ jumpPoseAnimation.name].speed = -landAnimationSpeed;
				_animation[ jumpPoseAnimation.name ].wrapMode = WrapMode.ClampForever;
				_animation.CrossFade( jumpPoseAnimation.name );				
			}
		} 
		else 
		{
			if( controller.velocity.sqrMagnitude < 0.1 ) 
			{
				//_animation[ idleAnimation.name ].speed = Mathf.Clamp( controller.velocity.magnitude, 0.0, idleAnimationSpeed );
				_animation.CrossFade( idleAnimation.name );
				if( underwater )
					_animation.CrossFade( "swim" );
			}
			else 
			{
				swimmingParticles.enableEmission = false;
				if( _characterState == State.Running ) 
				{
					_animation[ runAnimation.name ].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
					_animation.CrossFade( runAnimation.name );	
				}
				else if( _characterState == State.Trotting ) 
				{
					_animation[ walkAnimation.name ].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, trotMaxAnimationSpeed);
					_animation.CrossFade( walkAnimation.name );	
				}
				else if( _characterState == State.Walking ) 
				{
					_animation[ walkAnimation.name ].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, walkMaxAnimationSpeed);
					_animation.CrossFade( walkAnimation.name );	
				}
				else if( _characterState == State.Swimming )
				{
					swimmingParticles.enableEmission = true;
					_animation[ swimCycle.name ].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
					_animation.CrossFade( swimCycle.name );	
				}
				else if( _characterState == State.Boosting )
				{
					swimmingParticles.enableEmission = true;
					_animation[ swimCycle.name ].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
					_animation.CrossFade( swimCycle.name );	
				}
			}
		}
	}
	// ANIMATION sector
	if( !underwater )
	{
		// Set rotation to the move direction
		//if( IsGrounded() )
			//transform.rotation = Quaternion.LookRotation( moveDirection );
		//else
		//{
		//	var xzMove = movement;
		//	xzMove.y = 0;
		//	if( xzMove.sqrMagnitude > 0.001 )
		//		transform.rotation = Quaternion.LookRotation( xzMove );
		//}		
		// We are in jump mode but just became grounded
		if( IsGrounded() )
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if( jumping )
			{
				jumping = false;
				SendMessage( "DidLand", SendMessageOptions.DontRequireReceiver );
			}
		}
	}
	else
		if( moveDirection != Vector3.zero )
			transform.rotation = Quaternion.LookRotation( moveDirection );
}
 
function OnControllerColliderHit( hit : ControllerColliderHit )
{
//	Debug.DrawRay(hit.point, hit.normal);
	if( hit.moveDirection.y > 0.01 ) 
		return;
}

function GetSpeed () 
{
	return moveSpeed;
}

function IsJumping () 
{
	return jumping;
}

function IsGrounded () 
{
	return( collisionFlags & CollisionFlags.CollidedBelow ) != 0;
}

function GetDirection () 
{
	return moveDirection;
}

function IsMovingBackwards () 
{
	return movingBack;
}

function GetLockCameraTimer () 
{
	return lockCameraTimer;
}

function IsMoving ()  : boolean
{
	return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
}

function HasJumpReachedApex ()
{
	return jumpingReachedApex;
}

function IsGroundedWithTimeout ()
{
	return lastGroundedTime + groundedTimeout > Time.time;
}

function Reset ()
{
	gameObject.tag = "Player";
}

