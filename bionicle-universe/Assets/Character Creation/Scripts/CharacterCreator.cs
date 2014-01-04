using UnityEngine;
using System.Collections;
using System.IO;
//********//********//********//********//********//********//********//********
/*the starting GUI will let the player enter a name. That script will then
 * determine if there is a save file for that character or not. If not, the
 * starting GUi will then become disabled and this script will become enabled.
 * It will also be given the name of the player. This script needs to be attached
 * to a camera
 */
public class CharacterCreator : MonoBehaviour
{
    public string playerName = "player";
    public string[] koros = {
		"Ga Koro",
		"Ta Koro",
		"Le Koro",
		"Ko Koro",
		"Po Koro",
		"Onu Koro"
	};
	public string[] koroDescriptions = {
		"The matoran of Ga-Koro are one with the sea. They are skilled navigarots"
		+" and talented crafters, making the tools for their ships out of the "
		+"materials found in Naho Bay. A Ga-Matoran is wise, moreso than others,"
		+" and is resistant to Water and Fire attacks but weak against Earth.",
		"The matoran of Ta-Koro are as fierce as the volcano on which they make"
		+" their home and are as strong as the rock of their fortress's walls." 
		+" A Ta-Matoran is stronger than matoran of other sorts. He is also "
		+"resistant to Ice and vulnerable to Water",
		"The matoran of Le-Koro are known for their swiftness in the trees and"
		+" their light-hearted demeanor. What a Le-Matoran lacks in height, he"
		+" makes up for in Speed. In addition, a Le-Matoran is strong against"
		+" Earth but weak aginst Fire.",
		"Hailing from the slopes of Mount Ihu, a Ko-Matoran contains vast"
		+" knowledge of the natural world. Although cold and unfriendly, a Ko-"
		+"Matoran has a great many things to speak of, if you can maintain a"
		+" conversation with one. Ko-Matoran are weak against Fire but strong"
		+" against Rock.",
		"Po-Matoran have always been known for their stoneworking, as a quick"
		+" glance outside their home will reveal. Po-Matoran are also masters"
		+" of bartering, using their inherent approachable nature to convince"
		+" other to part with precious goods. A Po-Matoran is weak against Ice"
		+" but strong against Air.",
		"An Onu-Matoran spends his whole life in the tunnels, and thus finds"
		+" it somewhat difficult to navigate in daylight, although an Onu-"
		+"Matoran's night vision is bested by none. An Onu-Matoran is also"
		+" rather strong of will, a trait brought upon by decades of battle"
		+" with the Earth itself. An Onu-Matoran fares well in a fight with"
		+" creatures of the Air, but has little trouble withstanding attacks"
		+" of Water."
	};
	public string[] maskDescriptions = {
		"Hau - Mask of Shileding\nWhen active, the Hau gives its wearer a protective shield",	//Implement this with a simple variable on the player's hitpoints script. Damage reduction proportional to Mask Use skill
		"Kaukau - Mask of Water-Breathing\nWhen active, the Kaukau lets its wearer breathe underwater",	//Underwater loot, anybody?	Time can breathe proportional to Mask Use skill
		"Miru - Mask of Levitation\nWhen active, the Miru lets its wearer hover above thr ground, granting bonuses to evasion",	//Yes, actually levitate. Plus, a chance to deal no damage based on Mask Use skill
		"Akaku - Mask of X-Ray Vision\nWhen actve, the user can detect protodermic life forms through visul obstructions",	//Protodermic because protodermis blocks x-rays, just like bones. Krana and Kraata are invisible here. Distance seeable is proportional to Mask use skill. Zoom is also a thign, accomplished by decreased FOV
		"Pakari - Mask of Strength\nWhen active, the wearer's strength is greately increased",	//Pretty simple. Strenght increased proportional to Mask Use skill
		"Kakama - Mask of Speed\nWhen active, the wearer is harder to hit and can travel nearly a hundred miles in a few seconds",	//See what I did there? I just justified fast travel. Regular movement speed and chance for no damage are proportional to Mask Use
		"Huna - Mask of Invisibility\nWhen active, the player is completely invisible, although sound is not muted",	//time invisible is proportinoal to Mask Use skill
		"Rau - Mask of Translation\nNo clue how this will effecti gameplay at all",	//HALP
		"Mahiki - Mask of Illusion\nWhen active, the wearer can disguise themself as any object in their vision",	//Obviously the stealth option. The illusion lasts longer with a higher Mask Use skill (Mask Use is important, no?). this.renderer.mesh = raycasthit.renderer.mesh
		"Matatu - Mask of Telekenesis\nWhen active, the wearer can telekenetically control any object in their vision",	//Heavier things require more Mask Use skill points to have been allocated
		"Ruru - Mask of Night Vision\nWhen active, the Ruru lets its user see in dark areas",	//We can have a light attached to this mask, and make it active when the mask is active. Can you guess what makes the light brighter?
		"Komau - Mask of Mind Control\nWhen active, the Komau lets its wearer take direct control of any being within their vision"	//The AI on a given Actor will give movement commands. We can just let those commands be given by imput. I'm not even going to tell you what allows you to take control of higher level creatures.
	};
	public string[] skills = {
		//Ga-Koro
		"Swimming (SPD)", "Fishing (SGY)", "Navigation (SPD)", "Healing (ACC)",
		//Ta=Koro
		"Forge Tool (STR)", "Forge Weapon (ACC)", "Forge Armor (STR)", "Lava Surfing (SPD)",
		//Le-Koro
		"Musicianship (CON)", "Ride Air Rahi (CON)", "Throw Disk (ACC)", "Balance (ACC)",
		//Onu-Koro
		"Prospecting (SGY)", "Ride Ground Rahi (CON)", "Cut Stone (STR)", "Spot (CON)",
		//Ko-Koro
		"Tracking (SGY)", "Prophesy (WIL)", "Survival (WIL)", "Sneaking (SGY)",
		//Po-Koro
		"Barter (CON)", "Carving (SGY)", "Climb (SGY)", "Animal Knowledge (CON)",
		//General
		"Melee Weapon (STR)", "Ranged Weapon (ACC)", "Mask Use (WIL)", "Diplomacy (CON)"

	};
	public int[] skillValues = {
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 0
	};
	private int[] skillBoosts;
	//str 5
	//spd 3
	//acc 4
	//sgy 6
	//con 7
	//wil 3
	public string[] statDescriptions = {
		"Strength (STR) allows a character to inflict more melee damage. It also influences a number of skills.",
		"Speed (SPD) gives a character a chance to decrease damage taken. It also affects movement speed and influences a number of skills.",
		"Confidence (CON) gives a character a significant leg up in social interactions. It can affect things like prices of goods or one's ability to convince others of a thing.",
		"Willpower (WIL) describes how hard one can concentrate. A high Willpower will increase one's ability to use a mask while under attack, and will allow one to take more damage.",
		"Strategy (SGY) lets a character formulate different ways of solving problems and thus can affect literally anything. It scales exponentially and affects a number of skills.",
		"Accuracy (ACC) is a mesaure of how careful a character can be. It affects ranged damage and a number of skills which involve the use os hands."
	};
    public GUISkin mainSkin, koroSkin;
    public SkinnedMeshRenderer meshRenderer;
    public Material[] materials;
    private Material[] meshMats;
    public Texture2D[] matImages;
    public GameObject[] masks;
	public Texture2D[] maskImages;

	public int element, mainColor, secondaryColor, mask, str, spd, con, wil;
	public int pointsAllowed = 72;
	public int srg, acc;

    public CreationPanel panel = CreationPanel.Koro;

	public Vector3 targetRotation;
	public Vector3 targetPosition;

	public Transform rotater;

	private string curmsg = "";

    public enum CreationPanel
    {
        Koro = 1,
        Colors = 2,
        Vitals = 3,
    }

    public void saveElement()
    {
        StreamWriter writer = new StreamWriter( Application.persistentDataPath + "/" +playerName +".budf", false);
        writer.WriteLine("Element: " + element);
        writer.Close();
    }

    public void saveColors()
    {
        StreamWriter writer = new StreamWriter( Application.persistentDataPath + "/" + playerName + ".budf", true);
        writer.WriteLine("Main Color: " + mainColor);
        writer.WriteLine("Secondary Color: " + secondaryColor);
        writer.Close();
    }

	public void Awake() {
		targetRotation = transform.eulerAngles;
		str = spd = con = wil = srg = acc = 10;
		skillBoosts = new int[28];
	}

    public void Update() {
        meshMats = meshRenderer.materials;
        meshMats[2] = materials[mainColor];
        meshMats[3] = materials[secondaryColor];
        meshRenderer.materials = meshMats;
		rotater.rotation = Quaternion.Slerp(
			rotater.rotation,
			Quaternion.Euler( targetRotation ),
			5 * Time.deltaTime );
    }

	public IEnumerator moveBack() {
		float startTime = Time.time;
		while( Time.time - 1 < startTime ) {
			transform.Translate( new Vector3( 0, 0, -3.5f ) * Time.deltaTime );
			yield return new WaitForEndOfFrame();
		}
	}

	public void loadSkillBoosts( int koro ) {
		switch( koro ) {
		case 0:
			skillBoosts = new int[28];
			skillBoosts[0] = 15;
			skillBoosts[1] = 10;
			skillBoosts[2] = 5;
			skillBoosts[3] = 10;
			skillBoosts[10] = 5;
			skillBoosts[17] = 10;
			skillBoosts[20] = 10;
			skillBoosts[23] = 5;
			break;
		}
	}

    public void OnGUI() {
		GUI.skin = mainSkin;
		GUI.contentColor = new Color( 19, 19, 19 );
        if (panel == CreationPanel.Koro) {
            GUI.Box(new Rect((Screen.width / 2) - 128, 10, 256, 45), "Choose a home Koro");
            element = GUI.SelectionGrid(new Rect(10, 10, 256, 400), element, koros, 1);
			targetRotation = new Vector3( 0, 60 * element, 0 );
			GUI.Box( new Rect( 10, 420, 256, 210 ), "" + koroDescriptions[element] );
            if( GUI.Button( new Rect( Screen.width-266, Screen.height-55,256, 46 ), "Continue" ) ) {
                saveElement();
				loadSkillBoosts( element );
                panel = CreationPanel.Colors;
				StartCoroutine( "moveBack" );
            }
        } 
		else if (panel == CreationPanel.Colors) {
            GUI.BeginGroup( new Rect(10, 50, 500, 500 ) );
	            GUI.Box( new Rect( 0, 0, 200, 250 ), "" );
	            GUI.Label( new Rect( 40, 10, 140, 30 ), "Choose a main color" );
			mainColor = GUI.SelectionGrid(new Rect(10, 50, 180, 180), mainColor, matImages, 3);
			masks[mask].renderer.material = materials[mainColor];
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(Screen.width - 200, 50, 500, 500));
	            GUI.Box(new Rect(0, 0, 200, 250), "");
	            GUI.Label(new Rect(32, 10, 140, 30), "Choose a secondary color");
	            secondaryColor = GUI.SelectionGrid(new Rect(10, 50, 180, 180), secondaryColor, matImages, 3);
            GUI.EndGroup();
            if (GUI.Button(
				new Rect(Screen.width - 266, Screen.height - 55, 256, 46),
				"Continue")) {
                saveColors();
                panel = CreationPanel.Vitals;
            }
            else if (GUI.Button(new Rect(10, Screen.height - 55, 256, 46),
			                    "Go back")) {
                panel = CreationPanel.Koro;
			}
        }
		else if (panel == CreationPanel.Vitals) {
			/****************************************************
			 * Masks
			 ****************************************************/
            GUI.BeginGroup(new Rect(10, 10, 200, 310));
	            GUI.Box(new Rect(0, 0, 200, 280), "");
	            GUI.Label(new Rect(30, -4, 140, 30), "Select a starting mask");
				masks[mask].SetActive( false );
	            mask = GUI.SelectionGrid(new Rect(10, 30, 180, 240), mask, maskImages, 3);
				masks[mask].SetActive( true );
				masks[mask].renderer.material = materials[mainColor];
            GUI.EndGroup();

			/*
			 * Dem stats
			 * 
			 * Strength (STR) - melee damage, affects some skills
			 * Speed (SPD) - AP if we have turn-based combat, movement speed otherwise. Can reduce damage dealt. Replaces dexterity and affects skills
			 * Accuracy (ACC) - Ranged damage, affects skills
			 * Strategy (SGY) - Can improve all things, replaces Wisdom. Adds skill points (2xSGY). Affects skills
			 * Confidence (CON) - Dramatically improves social interactions
			 * Willpower (WIL) - Affects hitpoints, mask use
			 */
            GUI.BeginGroup(new Rect(210, 10, 200, 310));
	            GUI.Box(new Rect(0, 0, 200, 280), "");
	            GUI.Label(new Rect(30, -4, 140, 30), "Choose your vital stats");
			
				GUI.Label( new Rect( 10, 50, 180, 30 ), 
			          "Points left: " +(pointsAllowed - (str + spd + con + wil + srg + acc)) );

			GUIStyle leftArrow = (GUIStyle)mainSkin.customStyles.GetValue( 2 );
			GUIStyle rightArrow = (GUIStyle)mainSkin.customStyles.GetValue( 3 );

			GUI.Label(new Rect(10, 80, 90, 30), "Strength:");
				if( GUI.Button( new Rect( 100, 80, 30, 30 ), "", leftArrow ) ) {
					if( str > 1 ) {
						str--;
						curmsg = statDescriptions[0];
					}
				}
	            GUI.Label(new Rect(140, 80, 90, 30), "" + str);
			if( GUI.Button( new Rect( 160, 80, 30, 30), "", rightArrow ) ) {
				if( str < 20 ) {
					str++;
					curmsg = statDescriptions[0];
				}
			}

	            GUI.Label(new Rect(10, 110, 90, 30), "Speed:");
			if( GUI.Button( new Rect( 100, 110, 30, 30 ), "", leftArrow ) ) {
					if( spd > 1 ) {
					spd--;
					curmsg = statDescriptions[1];
					}
				}
				GUI.Label(new Rect(140, 110, 90, 30), "" + spd);
			if( GUI.Button( new Rect( 160, 110, 30, 30), "", rightArrow ) ) {
					if( spd < 20 ) {
					spd++;
					curmsg = statDescriptions[1];
					}
				}

	            GUI.Label(new Rect(10, 140, 90, 30), "Confidence:");
			if( GUI.Button( new Rect( 100, 140, 30, 30 ), "", leftArrow ) ) {
					if( con > 1 ) {
					con--;
					curmsg = statDescriptions[2];
					}
				}
				GUI.Label(new Rect(140, 140, 90, 30), "" + con);
			if( GUI.Button( new Rect( 160, 140, 30, 30), "", rightArrow ) ) {
					if( con < 20 ) {
					con++;
					curmsg = statDescriptions[2];
					}
				}

	            GUI.Label(new Rect(10, 170, 90, 30), "Willpower:");
			if( GUI.Button( new Rect( 100, 170, 30, 30 ), "", leftArrow ) ) {
					if( wil > 1 ) {
					wil--;
					curmsg = statDescriptions[3];
					}
				}
				GUI.Label(new Rect(140, 170, 90, 30), "" + wil);
			if( GUI.Button( new Rect( 160, 170, 30, 30), "", rightArrow ) ) {
					if( wil < 20 ) {
					wil++;
					curmsg = statDescriptions[3];
					}
				}

	            GUI.Label(new Rect(10, 200, 90, 30), "Strategy:");
			if( GUI.Button( new Rect( 100, 200, 30, 30 ), "", leftArrow ) ) {
					if( srg > 1 ) {
					srg--;
					curmsg = statDescriptions[4];
					}
				}
				GUI.Label(new Rect(140, 200, 90, 30), "" + srg);
			if( GUI.Button( new Rect( 160, 200, 30, 30), "", rightArrow ) ) {
					if( srg < 20 ) {
					srg++;
					curmsg = statDescriptions[4];
					}
				}

	            GUI.Label(new Rect(10, 230, 90, 30), "Accuracy:");
			if( GUI.Button( new Rect( 100, 230, 30, 30 ), "", leftArrow ) ) {
					if( acc > 1 ) {
					acc--;
					curmsg = statDescriptions[5];
					}
				}
				GUI.Label(new Rect(140, 230, 90, 30), "" + acc);
			if( GUI.Button( new Rect( 160, 230, 30, 30), "", rightArrow ) ) {
					if( acc < 20 ) {
					acc++;
					curmsg = statDescriptions[5];
					}
				}
            GUI.EndGroup();

			//skills
			GUI.BeginGroup( new Rect( 410, 10, 250, 800 ) );
			GUI.Box(new Rect(0, 0, 250, 725), "");
			GUI.Label(new Rect(60, -4, 140, 30), "Allocate Skill Points");
			
			GUI.Label( new Rect( 10, 20, 180, 30 ), 
			          "Points left: " +(srg * 8) );
			for( int i = 0; i < 28; i++ ) {
				int posy = (i)*25 + 40;
				GUI.Label( new Rect( 10, posy, 200, 30 ), skills[i] );
				if( GUI.Button( new Rect( 170, posy, 30, 30 ), "", leftArrow ) ) {
					if( skillValues[i] > 0 ) {
						skillValues[i]--;
					}
				}
				GUI.Label ( new Rect( 205, posy - 2, 50, 30 ), "" +(skillValues[i] + skillBoosts[i]) );
				if( GUI.Button( new Rect( 220, posy, 30, 30 ), "", rightArrow ) ) {
					if( skillValues[i] < 20 ) {
						skillValues[i]++;
					}
				}
			}
			GUI.EndGroup();

			//descriptions
			GUI.BeginGroup( new Rect( 10, 320, 200, 310 ) );
			GUI.Box( new Rect( 0, 0, 200, 250 ), curmsg );
			GUI.EndGroup();

            if (GUI.Button(new Rect(10, Screen.height - 55, 256, 46), "Go back")) {
                panel = CreationPanel.Colors;
			} else if( GUI.Button( new Rect( Screen.width - 266, Screen.height-55, 256, 46 ), "Begin game" ) ) {
				Application.LoadLevel( element + 1 );
			}
        }
	}
}