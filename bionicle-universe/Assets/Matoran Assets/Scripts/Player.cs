using UnityEngine;
using System.Collections;

class Player : MonoBehaviour
{
    public int health = 100;
    public int currentHealth = 100;
    public int essencePoints = 100;
    public int currentEssence = 100;
    public int essenceResistance = 2;
    public int essenceStrength = 2;
    public int physicalResistance = 2;
    public int physicalStrength = 2;
    public int hitModifier = 2;
    public int dodgeModifier = 2;
    public int[] skills;
    public int curMaskInd = 0;
    public int curInvInd = 0;
    public Transform currentWeapon;
    public Vector3 weaponOffset = new Vector3(0, 0, 0);
    public Transform rightHand;
    public bool underAttack = false;
    public GUIMode guiMode;
    public Element playerElement;
    //public Item[] inventory;
    //public Mask[] masks;
    public int money = 100;
    //MainGUI _GUI;
    public GUISkin skin;
    public Texture2D statsBackground;
    public Texture2D healthBar;
    public Texture2D energyBar;

   // public Item basicItem;

    public Vector2 screenMiddle;

    public Ray ray;
    public RaycastHit hit;

    public enum Element
    {
        Fire = 0,
        Water = 1,
        Earth = 2,
        Stone = 3,
        Air = 4,
        Ice = 5,
    }

    public enum GUIMode
    {
        Default = 0,
        Inventory = 1,
    }

    void Start()
    {
        //currentWeapon.parent = rightHand;
        screenMiddle.x = Screen.width / 2;
        screenMiddle.y = Screen.height / 2;
        StartCoroutine("checkHealthLoss");
        //inventory = new Item[32];
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (currentEssence <= 0)
        {
            currentEssence = 0;
            //masks[curMaskInd].deactivate();
        }
        if (Input.GetButtonDown("Inventory"))
            if (guiMode == GUIMode.Default)
                guiMode = GUIMode.Inventory;
            else
                guiMode = GUIMode.Default;
      //  if (Input.GetMouseButtonDown(0))
          //  if (Physics.Raycast(ray, out hit, 12.5f))
                //if (hit.transform.root.GetComponent<Shopkeeper>() != null)
                   // hit.transform.root.GetComponent<Shopkeeper>().openShop();
    }

    void OnGUI()
    {
        GUI.skin = skin;
        if (guiMode == GUIMode.Inventory)
        {
            GUI.BeginGroup(new Rect(screenMiddle.x - 250, screenMiddle.y - 250, 500, 500));
            GUI.Box(new Rect(0, 0, 500, 500), "Inventory:");
          //  for (int i = 0; i < inventory.Length; i++)
           //     if (GUI.Button(new Rect((55 * (i / 8)) + 10, (55 * (i % 8)) + 10, 50, 50), inventory[i].getIcon()))
          //          changeWeapon(inventory[i].transform);
            GUI.EndGroup();
        }
        GUI.BeginGroup(new Rect(0, 0, 245, 90));
        GUI.DrawTexture(new Rect(0, 0, 245, 90), statsBackground, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(15, 15, (currentHealth / health) * 220, 25), healthBar, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(15, 50, (currentEssence / essencePoints) * 220, 25), energyBar, ScaleMode.StretchToFill);
        GUI.EndGroup();

    }

    IEnumerator checkHealthLoss()
    {
        int lastHealth = currentHealth;
        yield return new WaitForSeconds(0.5f);
        if (currentHealth < lastHealth)
            underAttack = true;
    }

    public int getEssence()
    {
        return currentEssence;
    }

    public void changeMask(int newMaskInd)
    {
        curMaskInd = newMaskInd;
    }

    public void changeWeapon(Transform weapon)
    {
        currentWeapon = weapon;
        currentWeapon.position = rightHand.position;
        currentWeapon.position += weaponOffset;
    }

    /*public void addToInventory(Item toAdd)
    {
        if (curInvInd < inventory.Length)
        {
            inventory[curInvInd] = toAdd;
            curInvInd++;
            if (curInvInd == inventory.Length)
                for (int i = 0; i < 32; i++)
                    if (inventory[i] == null)
                    {
                        curInvInd = i;
                        break;
                    }
        }
    }*/

    public bool pay(int price)
    {
        money -= price;
        if (money < 0)
        {
            money += price;
            return false;
        }
        return true;
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        underAttack = true;
    }

    public void loseEssence(int essenceLost)
    {
        currentEssence -= essenceLost;
    }
}