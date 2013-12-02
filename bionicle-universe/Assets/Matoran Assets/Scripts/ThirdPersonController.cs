#pragma strict
// Require a character controller to be attached to the same game object
[RequireComponent( typeof( CharacterController ) )]

class ThirdpersonController {
	public GameObject swimming;
	public bool canWalk = true;

	private Animator animator;

	enum State {
		Idle = 0,
		Walking = 1,
		Trotting = 2,
		Running = 3,
		Swimming = 4,
		Boosting = 5,
		Jumping = 6,
	}

	private State _characterState;

	// The speed when walking
	public float walkSpeed = 2.0;
	// when pressing "Fire3" button (cmd) we start running
	public float runSpeed = 6.0;
	public float bouyancy = 0.01;
	public Transform water;
	public ParticleSystem swimmingParticles;
	public float inAirControlAcceleration = 3.0;
	// How high do we jump when pressing jump and letting go immediately
	public float jumpHeight = 0.5;
	// The gravity for the character
	public float gravity = 20.0;
	// The gravity in controlled descent mode
	public float speedSmoothing = 10.0;
	public float rotateSpeed = 500.0;
	public float canJump = true;
	public float underwater = false;

	private float jumpRepeatTime = 0.5;
	private float jumpTimeout = 0.15;
	private float groundedTimeout = 0.25;
	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0;
	// The current move direction in x-z
	private var moveDirection = Vector3.zero;
	// The current vertical speed
	private float verticalSpeed = 0.0;
	// The current x-z move speed
	private float moveSpeed = 0.0;
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumping = false;
	private bool jumpingReachedApex = false;
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;
	// When did the user start walking (Used for going into trot after a while)
	private float walkTimeStart = 0.0;
	// Last time the jump button was clicked down
	private float lastJumpButtonTime = -10.0;
	// Last time we performed a jump
	private float lastJumpTime = -1.0;
	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private float lastJumpStartHeight = 0.0;
	private Vector3 inAirVelocity = Vector3.zero;
	private float lastGroundedTime = 0.0;
	public bool isControllable = true;

	private Vector3 movement;

	public CharacterController controller;

	public Transform cameraTransform;

	public void Awake()	{
		Screen.showCursor = false;
		moveDirection = transform.TransformDirection( Vector3.forward );
		animator = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		cameraTransform = Camera.main.transform;
	}

	public void updateSmoothedMovementDirection ()
	{
		bool grounded;
		grounded = IsGrounded();
		if( underwater ) {
			grounded = false;
		}
		
		var forward = cameraTransform.TransformDirection( Vector3.forward );
		if( !underwater ) {
			forward.y = 0;
		}
		forward = forward.normalized;
		
		var right = Vector3(forward.z, 0, -forward.x);
		
		var v = Input.GetAxisRaw("Vertical");
		var h = Input.GetAxisRaw("Horizontal");
		
		if( v < -0.2 ) {
			movingBack = true;
		} else {
			movingBack = false;
		}
		bool wasMoving = isMoving;
		isMoving = Mathf.Abs( h ) > 0.1 || Mathf.Abs( v ) > 0.1;
		// Target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;
		// Grounded controls
		if( grounded ) {
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if( isMoving != wasMoving ) {
				lockCameraTimer = 0.0;
			}
			if( targetDirection != Vector3.zero ) {
				// If we are really slow, just snap to the target direction
				if( moveSpeed < walkSpeed * 0.5 && grounded ) {
					moveDirection = targetDirection.normalized;
				// Otherwise smoothly turn towards it
				} else {
					moveDirection = Vector3.RotateTowards( moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000 );
					moveDirection = moveDirection.normalized;
				}
			}
			float curSmooth = speedSmoothing * Time.deltaTime;
			float targetSpeed = Mathf.Min( targetDirection.magnitude, 1.0 );
			_characterState = State.Idle;
			if( Input.GetKey( KeyCode.LeftShift ) || Input.GetKey( KeyCode.RightShift ) ) {
				targetSpeed *= runSpeed;
				_characterState = State.Running;
			} else {
				targetSpeed *= walkSpeed;
				_characterState = State.Walking;
			}
			moveSpeed = Mathf.Lerp( moveSpeed, targetSpeed, curSmooth );
			if( moveSpeed < walkSpeed * 0.3 ) {
				walkTimeStart = Time.time;
			}
		} else if( underwater ) {
			lockCameraTimer += Time.deltaTime;
			if( isMoving != wasMoving ) {
				lockCameraTimer = 0.0;
			}
			moveDirection = Vector3.RotateTowards( moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000 );
			moveDirection = moveDirection.normalized;
			var curSmuth = speedSmoothing * Time.deltaTime;
			var wantedSpeed = targetDirection.magnitude;
			_characterState = State.Idle;
			if( Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ) {
				wantedSpeed *= runSpeed;
				_characterState = State.Boosting;
			} else {
				wantedSpeed *= walkSpeed;
				_characterState = State.Swimming;
			}
			moveSpeed = Mathf.Lerp( moveSpeed, wantedSpeed, curSmuth );
		} else {
			if( jumping ) {
				lockCameraTimer = 0.0;
			}
			if( isMoving ) {
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
			}
		}		
		animator.SetValue( "speed", moveSpeed );
	}

	public void ApplyJumping() {
		// Prevent jumping too fast after each other
		if( lastJumpTime + jumpRepeatTime > Time.time ) {
			return;
		}
		if( IsGrounded() ) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if( canJump && Time.time < lastJumpButtonTime + jumpTimeout ) {
				verticalSpeed = CalculateJumpVerticalSpeed( jumpHeight );
				SendMessage( "DidJump", SendMessageOptions.DontRequireReceiver );
			}
		}
	}

	public void ApplyGravity() {
		if( isControllable ) {
			// Apply gravity
			var jumpButton = Input.GetButton( "Jump" );
			// When we reach the apex of the jump we send out a message
			if( jumping && !jumpingReachedApex && verticalSpeed <= 0.0 ) {
				jumpingReachedApex = true;
				SendMessage( "DidJumpReachApex", SendMessageOptions.DontRequireReceiver) ;
			}
			if( IsGrounded () ) {
				verticalSpeed = 0.0;
			} else {
				verticalSpeed -= gravity * Time.deltaTime;
			}			
		}
	}

	public float CalculateJumpVerticalSpeed( float targetJumpHeight ) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt( 2 * targetJumpHeight * gravity );
	}

	public void DidJump() {
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
		_characterState = State.Jumping;
	}

	public void Update() {
		if( transform.position.y < water.position.y ) {
			underwater = true;
		} else {
			underwater = false;
		}
		canJump = !underwater;
		if (!isControllable) {
			Input.ResetInputAxes();
		}
		if( Input.GetButtonDown("Jump") && canJump ) {
			lastJumpButtonTime = Time.time;
		}
		UpdateSmoothedMovementDirection();
		if( !underwater ) {
			ApplyGravity();
			ApplyJumping();
		}
		if( !underwater ) {
			movement = moveDirection * moveSpeed + Vector3( 0, verticalSpeed, 0 ) + inAirVelocity;
		} else {
			movement = moveDirection * moveSpeed + Vector3( 0, bouyancy, 0 );
		}
		movement *= Time.deltaTime;
		collisionFlags = controller.Move( movement );
		// ANIMATION sector
		if( !underwater ) {
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
}
