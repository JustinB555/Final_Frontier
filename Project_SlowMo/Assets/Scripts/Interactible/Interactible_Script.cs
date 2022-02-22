using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactible_Script : MonoBehaviour
{
    public bool isPlayerInTrigger = false;
    public bool isWorldEventAcrtive = false;
    //bool isInteractUIDisabled = false;
    //bool hasObjectBeentriggered = false;

    [SerializeField] bool isTriggerWeapon = false;
    [Tooltip("This object's JButler_WeaponPickup script \n(not needed for all interactibles, so it can be left blank).")]
    [SerializeField] JButler_WeaponPickup weaponPickup = null;

    //string worldEventText = "Press E to Interact";
    //string weaponEventText = "Press F to Pick Up";

    public int thisInteractionValue = 0;

    public KeyCode interactKey = KeyCode.E;

    Sound_Manager sndmngr = null;
    SphereCollider interactCollider = null;
    //GameObject interactUI = null;
    //GameObject worldEventImage = null;
    //GameObject weaponEventImage = null;
    InteractableManager manager = null;
    PlayerAnimatorScript plyrAnim = null;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInTrigger = false;
        isWorldEventAcrtive = false;

        sndmngr = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        interactCollider = gameObject.GetComponentInChildren<SphereCollider>();
        manager = FindObjectOfType<InteractableManager>();
        plyrAnim = FindObjectOfType<PlayerAnimatorScript>();
        //interactUI = GameObject.Find("InteractUI");
        //worldEventImage = interactUI.transform.GetChild(0).gameObject;
        //weaponEventImage = interactUI.transform.GetChild(1).gameObject;

        if (isTriggerWeapon)
        {
            thisInteractionValue = 1;
            interactKey = KeyCode.F;
        }
        else
        {
            thisInteractionValue = 0;
            interactKey = KeyCode.E;
        }



        //interactUI.transform.GetChild(0).gameObject.SetActive(false);
        //Debug.Log("current interactCollider is in" + interactCollider);
        //hasObjectBeentriggered = false;
    }
    private void Update()
    {
        InteractUI();

        if (isPlayerInTrigger && Input.GetKeyDown(interactKey))
        {
            ObjectBeenTriggered();
        }
    }
    public void ObjectBeenTriggered()
    {
        //hasObjectBeentriggered = true;
        if (isTriggerWeapon)
            WeaponPickup();
        else
            WorldEvent();
        //Debug.Log("hasObjectBeenTriggered = " + hasObjectBeentriggered);
    }
    public void PlayerInTrigger()
    {
        isPlayerInTrigger = true;
        //Debug.Log("isPLayerInTrigger = " + isPlayerInTrigger);
    }
    public void PlayerLeftTrigger()
    {
        isPlayerInTrigger = false;
        manager.isPlayerInTriggerManager = false;
        //Debug.Log("isPLayerInTrigger = " + isPlayerInTrigger);
    }
    public void InteractUI()
    {
        if (isPlayerInTrigger)
        {
            manager.isPlayerInTriggerManager = true;
            //interactUI.transform.GetChild(thisInteractionValue).gameObject.SetActive(true);
            //if (!isTriggerWeapon)
            //{
            //    interactUI.GetComponentInChildren<Text>().text = worldEventText;
            //    interactKey = KeyCode.E;
            //}
            //else
            //{
            //    interactUI.GetComponentInChildren<Text>().text = weaponEventText;
            //    interactKey = KeyCode.F;
            //}
        }
        else
        {
            //manager.isPlayerInTriggerManager = false;
            //interactUI.transform.GetChild(thisInteractionValue).gameObject.SetActive(false);
        }
    }
    private void WeaponPickup()
    {
        if (weaponPickup == null)
            throw new System.Exception(name + (" <b><color=red> has not set its Weapon Pickup</color></b>!\tMake sure to added this object as the reference here."));

        weaponPickup.ChangeWeapons();
        plyrAnim.PickUp();
        if (weaponPickup.NoWeapon())
            PlayerLeftTrigger();

        //Debug.Log("Weapon has been picked up");
        //interactUI.transform.GetChild(thisInteractionValue).gameObject.SetActive(false);
        //manager.isPlayerInTriggerManager = false;
        //gameObject.SetActive(false);
    }
    private void WorldEvent()
    {
        plyrAnim.PickUp();
        isWorldEventAcrtive = true;
        //Debug.Log("World Event has been triggered");
        //This is where we'd have to call reference to an animation tied to the interactible
    }
}
