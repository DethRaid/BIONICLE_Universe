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
    public GUISkin mainSkin, koroSkin;
    public SkinnedMeshRenderer meshRenderer;
    public Material[] materials;
    private Material[] meshMats;
    public Texture2D[] matImages;
    public GameObject[] masks;
	public Texture2D[] maskImages;

	public int element, mainColor, secondaryColor, mask, str, spd, con, wil;
	public int pointsAllowed = 72;
	public int srg, end;

    public CreationPanel panel = CreationPanel.Koro;

	public Vector3 targetRotation;
	public Vector3 targetPosition;

	public Transform rotater;

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
		str = spd = con = wil = srg = end = 1;
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
                panel = CreationPanel.Colors;
				StartCoroutine( "moveBack" );
            }
        } else if (panel == CreationPanel.Colors) {
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
        } else if (panel == CreationPanel.Vitals) {
            GUI.BeginGroup(new Rect(10, 10, 200, 310));
	            GUI.Box(new Rect(0, 0, 200, 280), "");
	            GUI.Label(new Rect(30, 10, 140, 30), "Select a starting mask");
				masks[mask].SetActive( false );
	            mask = GUI.SelectionGrid(new Rect(10, 40, 180, 240), mask, maskImages, 3);
				masks[mask].SetActive( true );
				masks[mask].renderer.material = materials[mainColor];
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(10, 300, 200, 300));
	            GUI.Box(new Rect(0, 0, 200, 300), "");
	            GUI.Label(new Rect(30, 10, 140, 30), "Choose your vital stats");

	            GUI.Label(new Rect(10, 50, 90, 30), "Strength:");
				if( GUI.Button( new Rect( 100, 50, 30, 30 ), "", (GUIStyle)mainSkin.customStyles.GetValue( 2 ) ) ) {
					if( str > 1 ) {
						str--;
					}
				}
	            GUI.Label(new Rect(100, 50, 90, 30), "" + str);
				if( GUI.Button( new Rect( 160, 50, 30, 30), "", (GUIStyle)mainSkin.customStyles.GetValue( 3 ) ) ) {
					if( str < 20 ) {
						str++;
					}
				}

	            GUI.Label(new Rect(10, 80, 90, 30), "Speed:");
				if( GUI.Button( new Rect( 100, 80, 30, 30 ), "", (GUIStyle)mainSkin.customStyles.GetValue( 2 ) ) ) {
					if( spd > 1 ) {
						spd--;
					}
				}
				GUI.Label(new Rect(100, 80, 90, 30), "" + spd);
				if( GUI.Button( new Rect( 160, 80, 30, 30), "", (GUIStyle)mainSkin.customStyles.GetValue( 3 ) ) ) {
					if( spd < 20 ) {
						spd++;
					}
				}

	            GUI.Label(new Rect(10, 110, 90, 30), "Confidence:");
				if( GUI.Button( new Rect( 100, 110, 30, 30 ), "", (GUIStyle)mainSkin.customStyles.GetValue( 2 ) ) ) {
					if( con > 1 ) {
						con--;
					}
				}
				GUI.Label(new Rect(100, 110, 90, 30), "" + con);
				if( GUI.Button( new Rect( 160, 110, 30, 30), "", (GUIStyle)mainSkin.customStyles.GetValue( 3 ) ) ) {
					if( con < 20 ) {
						con++;
					}
				}

	            GUI.Label(new Rect(10, 140, 90, 30), "Willpower:");
				if( GUI.Button( new Rect( 100, 140, 30, 30 ), "", (GUIStyle)mainSkin.customStyles.GetValue( 2 ) ) ) {
					if( wil > 1 ) {
						wil--;
					}
				}
				GUI.Label(new Rect(100, 140, 90, 30), "" + wil);
				if( GUI.Button( new Rect( 160, 140, 30, 30), "", (GUIStyle)mainSkin.customStyles.GetValue( 3 ) ) ) {
					if( wil < 20 ) {
						wil++;
					}
				}

	            GUI.Label(new Rect(10, 170, 90, 30), "Strategy:");
				if( GUI.Button( new Rect( 100, 170, 30, 30 ), "", (GUIStyle)mainSkin.customStyles.GetValue( 2 ) ) ) {
					if( srg > 1 ) {
						srg--;
					}
				}
				GUI.Label(new Rect(100, 170, 90, 30), "" + srg);
				if( GUI.Button( new Rect( 160, 170, 30, 30), "", (GUIStyle)mainSkin.customStyles.GetValue( 3 ) ) ) {
					if( srg < 20 ) {
						srg++;
					}
				}

	            GUI.Label(new Rect(10, 200, 90, 30), "Endurance:");
				if( GUI.Button( new Rect( 100, 200, 30, 30 ), "", (GUIStyle)mainSkin.customStyles.GetValue( 2 ) ) ) {
					if( end > 1 ) {
						end--;
					}
				}
				GUI.Label(new Rect(100, 200, 90, 30), "" + end);
				if( GUI.Button( new Rect( 160, 200, 30, 30), "", (GUIStyle)mainSkin.customStyles.GetValue( 3 ) ) ) {
					if( end < 20 ) {
						end++;
					}
				}

				GUI.Label( new Rect( 10, 230, 180, 30 ), 
			          "Points left: " +(pointsAllowed - (str + spd + con + wil + srg + end)) );
            GUI.EndGroup();
            if (GUI.Button(new Rect(10, Screen.height - 55, 256, 46), "Go back")) {
                panel = CreationPanel.Colors;
			} else if( GUI.Button( new Rect( Screen.width - 266, Screen.height-55, 256, 46 ), "Begin game" ) ) {
				Application.LoadLevel( element + 1 );
			}
        }
	}
}