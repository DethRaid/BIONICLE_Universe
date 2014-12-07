#pragma strict

var bottomRightOfScanningArea : Vector3;
var height : float;
var maxAngleAtNode : float;
var distanceBetweenNodes : float;
var xNodes : int;
var yNodes : int;
var allNodes : Node[,];

var curScanningPos : Vector3;
var hit : RaycastHit;

function Start()
{
	//allNodes = new Array();
	for( var i = 0; i < xNodes; i++ )
		for( var j = 0; j < yNodes; j++ )
		{
			curScanningPos.x = (distanceBetweenNodes*i)+bottomRightOfScanningArea.x;
			curScanningPos.y = bottomRightOfScanningArea.y+height;
			curScanningPos.x = (distanceBetweenNodes*j)+bottomRightOfScanningArea.z;
			if( Physics.Raycast(curScanningPos, -Vector3.up, hit) )
			{
				if( hit.normal.magnitude > maxAngleAtNode )
					continue;
				else
				{
					var nodePos : Vector3;
					nodePos.x = curScanningPos.x;
					nodePos.y = Terrain.activeTerrain.SampleHeight( curScanningPos );
					nodePos.z = curScanningPos.y;
					allNodes[i,j].place( nodePos );
					allNodes[i,j].index = Vector2( i, j );
				}
			}
		}
}