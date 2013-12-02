#pragma strict

var name : String;
var speechData;

var inventory : int[];

var bambooSold : boolean;
var airBladderSold : boolean;
var ProtodermisShardsSold : boolean;
var fireShardSold : boolean;
var waterShardSold : boolean;
var earthShardSold : boolean;
var stoneShardSold : boolean;
var iceShardSold : boolean;
var airShardSold : boolean;
var lightstoneSold : boolean;
var heatstoneSold : boolean;
var simpleAxeSold : boolean;
var sturdyAxeSold : boolean;
var simpleSickleSold : boolean;
var sturdySickleSold : boolean;
var lightstoneRifleSold : boolean;
var stoneHammerSold : boolean;
var throwingDiskSold : boolean;

var bambooTexture : Texture2D;

var bambooPrice : int = 5;

var shopOpen : boolean;
//var _player : Player;

var curX : int;
var curY : int;
var iconWidth : int = 50;
var iconHeight : int = 50;

var shopkeeperSkin : GUISkin;

var curGuiMessage : String = "";

function Start()
{
	var obj : GameObject = GameObject.FindGameObjectWithTag( "Player" );
	//_player = obj.GetComponent( Player );
	if( shopkeeperSkin )
		GUI.skin = shopkeeperSkin;
	inventory[0] = 5;
}

function OnGUI() : void
{
	curX = 0;
	curY = 0;
	if( curGuiMessage != "" )
		GUI.Box( Rect(Screen.width/2-100, Screen.height/2-15, 200, 30), curGuiMessage );
	if( shopOpen )
	{
		var xStart : int = Screen.width/2;
		xStart -= 250;
		var yStart : int = Screen.height/2;
		yStart -= 250;
		GUI.BeginGroup( Rect(xStart, yStart, 500, 500) );
			GUI.Box( Rect(0, 0, 500, 500), "" );
			if( bambooSold )
			{
				if( GUI.Button( Rect((curX*75)+10, (curY*75)+10, iconWidth, iconHeight), bambooTexture ) )
					sellItem( "Bamboo" );
				curX++;
				if( curX > 500/(iconWidth+10) )
				{
					curX = 0;
					curY++;
					if( curY > 500/(iconHeight+10) )
						curY = 0;
				}
			}
			if( GUI.Button( Rect(425, 5, 20, 20), "X" ) || Input.GetKeyDown( "escape" ) )
			{
				shopOpen = false;
				//_player.transform.GetComponent( ThirdPersonSwimmingController ).isControllable = true;
			}
		GUI.EndGroup();
	}
}

function OnMouseOver()
{
	//if( Vector3.Distance( transform.position, _player.transform.position ) > 5 )
		//curGuiMessage = "Click to open shop";
}

function openShop()
{
	shopOpen = true;
	//_player.transform.GetComponent( ThirdPersonSwimmingController ).isControllable = false;
}

function sellItem( s : String ) : void
{
	//if( s == "Bamboo" )
	//{
		//if( _player.pay( bambooPrice ) )
		//{
			//_player.addToInventory( Bamboo() );
		//  	inventory[0]--;
		//}
	//}
}