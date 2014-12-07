 
class Rahi extends MonoBehaviour{
	
	
	protected var level : int;
	protected var hp : int;
	protected var movementSpeed : float; //How fast the rahi will actually move
	protected var animationSpeed : float; //animation speed for defaultAnimation
	protected var idleSpeed : float; //animation speed for idle
	protected var sight: int; //how far should the range a rahi can se be?
	protected var obstacles : Transform[];
	//var rahi : GameObject;
	
	
	function Rahi (l : int, ms : float, aspeed : float, ispeed : float,  see : int, obs : Transform[])
	{
		level = l;
		movementSpeed = ms;
		animationSpeed = aspeed;
		idleSpeed = ispeed;
		sight = see;
		obstacles = obs;
		
	}
	
	function getLevel() {
		return level;
	}
	
	function setLevel(lev : int) {
		level = lev;
	}
	
	function getHP() {
		return hp;
	}
	function setHP(h : int) {
		hp = h;
	}
	
	function setMovementSpeed(sms : int)
	{
		movementSpeed = sms;
	}
	function getMovementSpeed() {
		return(movementSpeed);
	}
	
	function setSight(s : int) {
		sight = s;
	}
	function getSight() {
		return(sight);
	}
	
	function getAnimationSpeed() {
		return(animationSpeed);
	}
	function setAnimationSpeed(sas : int)
	{
		animationSpeed = sas;
	}
	
	function getIdleSpeed() {
		return(idleSpeed);
	}
	function setIdleSpeed(sas : int)
	{
		idleSpeed = sas;
	}
	
	function getObstacles()
	{
		return(obstacles);
	}
	function setObstacles(t : Transform[]) {
		obstacles = t;
	}
}


var transforms : Transform[];
var LEVEL : int;
var Movement_Speed : float; //How fast the rahi will actually move
var Animation_Speed : float; //animation speed for defaultAnimation
var Idle_Speed : float; //animation speed for idle
var range: int; //how far should the range a rahi can se be?


 var rahi : Rahi = new Rahi(LEVEL,Movement_Speed,Animation_Speed,Idle_Speed,range,transforms);



function Awake()
{
	setSight(range);
	setMovementSpeed(Movement_Speed);
	setObstacles(transforms);
	setAnimationSpeed(Animation_Speed);
	setIdleSpeed(Idle_Speed);
	
}

function Start()
{
	rahi.checkTarget();
	rahi.checkAnimation();
	Debug.Log("started");
}

function Update()
{
	rahi.rahiRun();
}

//Functions of Rahi
private var point : Vector3;
private var defaultAnimation: String;
protected var target : Transform;

function checkAnimation() //Checks animations of rahi
{
	if(this.animation["swim"] == null && this.animation["fly"] == null) {
		defaultAnimation = "crawl";
	}
	else if (this.animation["crawl"] == null && this.animation ["fly"] == null) {
		defaultAnimation = "swim";
	}
	else {
		defaultAnimation = "fly";
	}
	
}

function turn() {
	if(Random.value > 0.5) {
		this.transform.Rotate(Vector3(0,4,0));
	}
	else {
		this.transform.Rotate(Vector3(0,-4,0));
	}
}
	
function avoidObstacle()
{
	var hit : RaycastHit;
	if(Physics.Raycast(this.transform.position, Vector3.forward, hit, 4))
	{
		for(var i : int = 0; i < getObstacles().length; i++) {
			if(hit.transform == obstacles[i] ){
				this.transform.Rotate(Vector3(0,20,0) * Time.deltaTime);
				Debug.Log("hit obstacle");
			}
		}
	}
}

function checkTarget() //Sets target to Player	
{
	if (target == null && GameObject.FindWithTag("Player")) {
		target = GameObject.FindWithTag("Player").transform;
	}
}

function rahiRun() {
	var where: RaycastHit;
	if(Physics.Raycast(this.transform.position, Vector3.forward, where, getSight()))
	{
		if(where.transform.tag == "Player" || where.transform.tag == "Rahi") {
			/*if where.transform.*/
			target = where.transform;
		}
	}
	//Calculations
	var controller : CharacterController = this.GetComponent(CharacterController);
	var distance = Vector3.Distance(this.transform.position, target.position);
	var localTarget : Vector3 = this.transform.InverseTransformPoint(target.position);
	var angle : float = Mathf.Atan2(localTarget.x, localTarget.z);
	angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
	
	avoidObstacle();
	
	//Move forward while player is not in sight/too far aways
	if(distance > getSight() || angle > 118 || angle < -118) {
		if(this.animation.IsPlaying(defaultAnimation)) {
			if(Random.value > 0.1) {
				var forward : Vector3 = this.transform.TransformDirection((Vector3.forward * getMovementSpeed()));
				controller.SimpleMove(forward);
				Debug.Log("trying to play");
			}
			else {
				turn();
			}
		}
		else if(this.animation.IsPlaying("idle")) {
		}
		else {
			if(Random.value > 0.3) {
				var da : AnimationState = this.animation.CrossFadeQueued(defaultAnimation, 0.2, QueueMode.PlayNow);
				da.speed = getAnimationSpeed();

				Debug.Log("default is playing");
			}
			else {
				var idle : AnimationState = this.animation.CrossFadeQueued("idle",0.2, QueueMode.PlayNow);
				idle.speed = getIdleSpeed();
			}
		}
	}
	else if (distance < 2.5) {
		idle = this.animation.CrossFadeQueued("idle", 0.2, QueueMode.PlayNow);
		idle.speed = getIdleSpeed();
	}
	else {
		point = target.position; //Keeps this's head level
		point.y = this.transform.position.y;
		this.transform.LookAt(point);
		
		if(this.animation.IsPlaying(defaultAnimation)) {
			forward = this.transform.TransformDirection((Vector3.forward * getMovementSpeed()));
			controller.SimpleMove(forward);
		}
		else {
			da = this.animation.CrossFadeQueued(defaultAnimation, 0.2, QueueMode.PlayNow);
			da.speed = getAnimationSpeed();
		}
	}
}