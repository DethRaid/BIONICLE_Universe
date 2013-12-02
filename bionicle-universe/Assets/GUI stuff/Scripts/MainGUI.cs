using UnityEngine;
using System.Collections;

public class MainGUI : MonoBehaviour
{
    public GUISkin mySkin;

    public bool selectStuffGUI = false;
    public bool weaponSelectWindow = false;
    public bool inventoryWindow = false;
    public bool questLogWindow = false;
    public bool mainMenuWindow = false;
    public bool weaponOne = true;
    public bool weaponTwo = false;
    public bool weaponThree = false;
    public bool weaponFour = false;

    public Texture2D statsBackground;
    public Texture2D healthBar;
    public Texture2D energyBar;
    public Texture2D centerPoint;

    public Transform _player;
    public float playerHealth;
    public float currentPlayerHealth;
    public float playerEssence;
    public float currentPlayerEssence;

    public bool errorMessage = false;
    public string newMessage;

    //var attackScript = GetComponent(ThirdPersonController.AttackScript);

    /*void Start()
    {
        playerHealth = _player.GetComponent( "Player" ).health;
        currentPlayerHealth = _player.GetComponent( "Player" ).currentHealth;
        playerEssence = _player.GetComponent( "Player" ).essencePoints;
        currentPlayerEssence = _player.GetComponent( "Player" ).currentEssence;
    }

        void Update()
    {
        currentPlayerHealth = _player.GetComponent( "Player" ).currentHealth;
        currentPlayerEssence = _player.GetComponent( "Player" ).currentEssence;
    }*/

    void Update() {
        if( Input.GetButtonDown( "SelectionGUI" ) ) {
            selectStuffGUI = !selectStuffGUI;
            MouseLook[] looks = _player.GetComponentsInChildren<MouseLook>();
            foreach( MouseLook m in looks ) {
                m.enabled = !selectStuffGUI;
            }
        }
    }

    void OnGUI() {
        GUI.skin = mySkin;

        GUI.DrawTexture( new Rect( Screen.width / 2 - 5, Screen.height / 2 - 5, 10, 10 ), centerPoint, ScaleMode.StretchToFill );

        GUI.BeginGroup( new Rect( Screen.width / 2 - 270, Screen.height / 2 + 200, 600, 600 ) );
        weaponSelectWindow = GUI.Toggle( new Rect( 0, 25, 150, 60 ), weaponSelectWindow, "Weapons Menu", "button" );
        inventoryWindow = GUI.Toggle( new Rect( 150, 25, 150, 60 ), inventoryWindow, "Inventory", "button" );
        questLogWindow = GUI.Toggle( new Rect( 300, 25, 150, 60 ), questLogWindow, "Quest Log", "button" );
        mainMenuWindow = GUI.Toggle( new Rect( 450, 25, 150, 60 ), mainMenuWindow, "Main Menu", "button" );
        GUI.EndGroup();
        /*GUI.BeginGroup (new Rect (Screen.width / 2 - 620, Screen.height / 2 - 320, 600, 600));
        GUI.Box (new Rect (0,0,300,115), "Life/energy" );
        GUI.Box (new Rect (15,20,Stats.life,40), "", "lifeSkin" );
        GUI.Box (new Rect (15,65,Stats.energy / 2,40), "", "energySkin" );
        GUI.EndGroup ();*/
        //health and energy bars
        /*GUI.BeginGroup( new Rect(0, 0, 245, 90) );
            GUI.DrawTexture( new Rect(0, 0, 245, 90), statsBackground, ScaleMode.StretchToFill );
            GUI.DrawTexture( new Rect(15, 15, (currentPlayerHealth/playerHealth)*220, 25), healthBar, ScaleMode.StretchToFill );
            GUI.DrawTexture( new Rect(15, 50, (currentPlayerEssence/playerEssence)*220, 25), energyBar, ScaleMode.StretchToFill );
        GUI.EndGroup();*/

        if( errorMessage )
            GUI.Box( new Rect( 10, 100, 100, 500 ), newMessage );
        if( selectStuffGUI ) {
            SelectStuffGUI();
        } else if( weaponSelectWindow == true ) {
            WeaponSelectorGUI();
        } else if( questLogWindow == true ) {
            QuestLogGUI();
        }
    }

    void SelectStuffGUI() {
        GUI.BeginGroup( new Rect( Screen.width / 2 - 150, Screen.height / 2 - 300, 300, 600 ) );
        GUI.DrawTexture( new Rect( 0, 0, 300, 600 ), statsBackground, ScaleMode.StretchToFill );
        GUI.Label( new Rect( 30, 15, 240, 100 ), "Pause Menu" );
        weaponSelectWindow = GUI.Button( new Rect( 30, 130, 240, 50 ), "Select Weapon" );
        questLogWindow = GUI.Button( new Rect( 30, 195, 240, 50 ), "Quest Log" );
        inventoryWindow = GUI.Button( new Rect( 30, 260, 240, 50 ), "Inventory" );
        GUI.EndGroup();
    }

    void WeaponSelectorGUI() {
        //This is the weapon attack selector group
        GUI.BeginGroup( new Rect( Screen.width / 2 - 200, Screen.height / 2 + 40, 500, 500 ) );

        GUI.Box( new Rect( 0, 0, 450, 100 ), "Weapons Menu" );
        weaponOne = GUI.Toggle( new Rect( 5, 30, 110, 60 ), weaponOne, "Attack 1" );
        weaponTwo = GUI.Toggle( new Rect( 115, 40, 110, 60 ), weaponTwo, "Attack 2" );
        weaponThree = GUI.Toggle( new Rect( 225, 40, 110, 60 ), weaponThree, "Attack 3" );
        weaponFour = GUI.Toggle( new Rect( 335, 30, 110, 60 ), weaponFour, "Attack 4" );

        GUI.EndGroup();

        //if( weaponOne )
        //attackScript.selectWeapon(0);
        //else if( weaponTwo )
        //attackScript.selectWeapon(1);
        //else if( weaponThree )
        //attackScript.selectWeapon(2);
        //	else if( weaponFour )
        //attackScript.selectWeapon(3);
    }

    void QuestLogGUI() {
        GUI.BeginGroup( new Rect( Screen.width / 2 - 500, Screen.height / 2 - 300, 500, 500 ) );
        GUI.Box( new Rect( 0, 0, 350, 400 ), "Quest Log", "window" );
        GUI.EndGroup();
    }

    void displayMessage( string message ) {
        newMessage = message;
        errorMessage = true;
    }
}