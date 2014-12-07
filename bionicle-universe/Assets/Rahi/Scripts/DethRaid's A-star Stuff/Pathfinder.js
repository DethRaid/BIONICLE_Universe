#pragma strict

var scanner : MapScanner;
var openNodes : Node[];
var closedNodes : Node[];
var curNodeInd : int = 0;
var pathLength : int = 0;
var endPos : Vector3;
var startPos : Vector3;

function Start()
{
//	scanner = Object.FindObjectOfType( MapScanner ) as MapScanner;
}

function findClosestNode() : Vector2
{
	for( var i = 0; i < scanner.xNodes; i++ )
		for( var j = 0; j < scanner.yNodes; j++ )
			if( Vector3.Distance(transform.position, scanner.allNodes[i,j].position) < scanner.distanceBetweenNodes/2 )
				return Vector2( i, j );
}

function loadOpenNodes( curNode : Vector2 )
{
	var allNodes = scanner.allNodes;
	var curNodeI = curNode.x;
	var curNodeJ = curNode.y;
	if( allNodes[curNodeI-1,curNodeJ-1].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI-1,curNodeJ-1];
		curNodeInd++;
	}
	if( allNodes[curNodeI,curNodeJ-1].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI,curNodeJ-1];
		curNodeInd++;
	}
	if( allNodes[curNodeI+1,curNodeJ-1].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI+1,curNodeJ-1];
		curNodeInd++;
	}
	if( allNodes[curNodeI-1,curNodeJ].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI-1,curNodeJ];
		curNodeInd++;
	}
	if( allNodes[curNodeI+1,curNodeJ].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI+1,curNodeJ];
		curNodeInd++;
	}
	if( allNodes[curNodeI-1,curNodeJ+1].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI-1,curNodeJ+1];
		curNodeInd++;
	}
	if( allNodes[curNodeI,curNodeJ+1].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI,curNodeJ+1];
		curNodeInd++;
	}
	if( allNodes[curNodeI+1,curNodeJ+1].isOpen )
	{
		openNodes[curNodeInd] = allNodes[curNodeI+1,curNodeJ+1];
		curNodeInd++;
	}
	for( var node in openNodes )
	{
		node.calculateGValue( startPos );
		node.calculateHValue( endPos );
	}
}

function findPath( end : Vector3 )
{
	endPos = end;
	var ourNode : Vector3 = findClosestNode();
	loadOpenNodes( ourNode );
	var biggestG : float;
	for( var node in openNodes )
		if( node.g > biggestG )
		{}
			
}