using UnityEngine;
using System.Collections;
using System.IO;

//the starting GUI will let the player enter a name. That script will then determine if there is a save file for that character or not.
//If not, the starting GUi will then become disabled and this script will become enabled. It will also be given the name of the player.
public class CharacterCreator : MonoBehaviour
{
    public string playerName = "player";
    public string[] koros = { "Ta Koro", "Ga Koro", "Le Koro", "Ko Koro", "Onu Koro", "Po Koro" };
	public Texture2D[] koroImages;
    public GUISkin mainSkin, koroSkin;
    public SkinnedMeshRenderer meshRenderer;
    public Material[] materials;
    private Material[] meshMats;
    public Texture2D[] matImages;
    public GameObject[] masks;
	public Texture2D[] maskImages;
	public MeshRenderer backWall;

    public int element, mainColor, secondaryColor, mask, str, spd, con, wil, srg, end;

    public CreationPanel panel = CreationPanel.Koro;

    public enum CreationPanel
    {
        Koro = 1,
        Colors = 2,
        Vitals = 3,
    }

    public void saveElement()
    {
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/" +playerName +".budf", false);
        writer.WriteLine("Element: " + element);
        writer.Close();
    }

    public void saveColors()
    {
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/" + playerName + ".budf", true);
        writer.WriteLine("Main Color: " + mainColor);
        writer.WriteLine("Secondary Color: " + secondaryColor);
        writer.Close();
    }

	public void Awake() {
	}

    public void Update()
    {
        meshMats = meshRenderer.materials;
        meshMats[2] = materials[mainColor];
        meshMats[3] = materials[secondaryColor];
        meshRenderer.materials = meshMats;
    }

    public void OnGUI()
	{
		GUI.skin = mainSkin;
		GUI.contentColor = new Color( 19, 19, 19 );
        if (panel == CreationPanel.Koro)
        {
            GUI.Box(new Rect((Screen.width / 2) - 128, 10, 256, 45), "Choose a home Koro");
            element = GUI.SelectionGrid(new Rect(10, 10, 256, 400), element, koros, 1);
			//We'll show a render of each Koro as the koros are selected
			//For now, though, a solid color will do fine
			backWall.material.mainTexture = koroImages[element];
            if( GUI.Button( new Rect( Screen.width-266, Screen.height-55, 256, 46), "Continue" ) )
            {
                saveElement();
                panel = CreationPanel.Colors;
            }
        }
        else if (panel == CreationPanel.Colors)
        {
            GUI.BeginGroup(new Rect(10, 50, 500, 500));
	            GUI.Box(new Rect(0, 0, 200, 250), "");
	            GUI.Label(new Rect(40, 10, 140, 30), "Choose a main color");
			mainColor = GUI.SelectionGrid(new Rect(10, 50, 180, 180), mainColor, matImages, 3);
			masks[mask].renderer.material = materials[mainColor];
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(Screen.width - 200, 50, 500, 500));
	            GUI.Box(new Rect(0, 0, 200, 250), "");
	            GUI.Label(new Rect(32, 10, 140, 30), "Choose a secondary color");
	            secondaryColor = GUI.SelectionGrid(new Rect(10, 50, 180, 180), secondaryColor, matImages, 3);
            GUI.EndGroup();
            if (GUI.Button(new Rect(Screen.width - 266, Screen.height - 55, 256, 46), "Continue"))
            {
                saveColors();
                panel = CreationPanel.Vitals;
            }
            else if (GUI.Button(new Rect(10, Screen.height - 55, 256, 46), "Go back"))
                panel = CreationPanel.Koro;
        }
        else if (panel == CreationPanel.Vitals)
        {
            GUI.BeginGroup(new Rect(10, 10, 200, 310));
	            GUI.Box(new Rect(0, 0, 200, 280), "");
	            GUI.Label(new Rect(30, 10, 140, 30), "Select a starting mask");
				masks[mask].SetActive( false );
	            mask = GUI.SelectionGrid(new Rect(10, 40, 180, 240), mask, maskImages, 3);
				masks[mask].SetActive( true );
				masks[mask].renderer.material = materials[mainColor];
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(10, 300, 200, 300));
	            GUI.Box(new Rect(0, 0, 200, 250), "");
	            GUI.Label(new Rect(30, 10, 140, 30), "Choose your vital stats");
	            GUI.Label(new Rect(10, 50, 90, 30), "Strength:");
	            str = int.Parse(GUI.TextField(new Rect(100, 50, 90, 30), "" + str));
	            GUI.Label(new Rect(10, 80, 90, 30), "Speed:");
	            spd = int.Parse(GUI.TextField(new Rect(100, 80, 90, 30), "" + spd));
	            GUI.Label(new Rect(10, 110, 90, 30), "Confidence:");
	            con = int.Parse(GUI.TextField(new Rect(100, 110, 90, 30), "" + con));
	            GUI.Label(new Rect(10, 140, 90, 30), "Willpower:");
	            wil = int.Parse(GUI.TextField(new Rect(100, 140, 90, 30), "" + wil));
	            GUI.Label(new Rect(10, 170, 90, 30), "Strategy:");
	            srg = int.Parse(GUI.TextField(new Rect(100, 170, 90, 30), "" + srg));
	            GUI.Label(new Rect(10, 200, 90, 30), "Endurance:");
	            end = int.Parse(GUI.TextField(new Rect(100, 200, 90, 30), "" + end));
            GUI.EndGroup();
            if (GUI.Button(new Rect(10, Screen.height - 55, 256, 46), "Go back"))
                panel = CreationPanel.Colors;
        }
	}
}