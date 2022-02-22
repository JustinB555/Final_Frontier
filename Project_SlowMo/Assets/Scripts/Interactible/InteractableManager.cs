using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    [SerializeField] Interactible_Script[] interactables = null;
    GameObject interactUI = null;
    GameObject worldEventImage = null;
    GameObject weaponEventImage = null;
    public bool isPlayerInTriggerManager = false;
    KeyCode interactKey = KeyCode.E;
    public bool isUiCurrentlyOn = false;
    int thisInteractionValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        interactables = FindObjectsOfType<Interactible_Script>();
        interactUI = GameObject.Find("InteractUI");
        worldEventImage = interactUI.transform.GetChild(0).gameObject;
        weaponEventImage = interactUI.transform.GetChild(1).gameObject;
        isUiCurrentlyOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInTriggerManager)
        {

            foreach (Interactible_Script i in interactables)
            {
                if (i.isPlayerInTrigger == true && i.gameObject.activeInHierarchy)
                {
                    interactKey = i.interactKey;
                    thisInteractionValue = i.thisInteractionValue;
                }
                else
                {
                    continue;
                }
            }
            if (!isUiCurrentlyOn)
            {
                interactUI.transform.GetChild(thisInteractionValue).gameObject.SetActive(true);
                isUiCurrentlyOn = true;
            }

        }
        else
        {
            interactUI.transform.GetChild(0).gameObject.SetActive(false);
            interactUI.transform.GetChild(1).gameObject.SetActive(false);
            if (isUiCurrentlyOn)
            {
                ToggleUIBool();
            }
        }
    }

    void ToggleUIBool()
    {
        isUiCurrentlyOn = !isUiCurrentlyOn;
    }
}
