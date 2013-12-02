#pragma strict

var next : Node;
var isWalkable : boolean = true;
var isOpen : boolean = true;
var g : float;
var h : float;
var position : Vector3;
var index : Vector2;

function place( pos : Vector3 )
{
	position = pos;
}

function calculateGValue( last : Vector3 )
{
	if( next != null )
	{
		g = Vector3.Distance( position, next.position );
		g += next.g;
	}
	else
		g = Vector3.Distance( position, last );
}

function calculateHValue( pos : Vector3 )
{
	h = Vector3.Distance( position, pos );
}