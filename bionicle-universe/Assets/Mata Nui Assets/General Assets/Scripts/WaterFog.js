var water : GameObject;

function Update () {
	if (this.transform.position.y <= water.transform.position.y) {
		RenderSettings.fog = true;
		RenderSettings.fogDensity = 0.02;
	}
	
	else {
		RenderSettings.fog = false;
	}
}