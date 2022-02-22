using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenInteractable : MonoBehaviour
{
    [SerializeField]
    Animator DoorAnimator = null;
    [SerializeField]
    Animator LightAnimator = null;
    [SerializeField] bool closeBehind = false;
    bool isDoorOpen = false;
    Interactible_Script interactScript = null;

    private void Start()
    {
        isDoorOpen = false;
        interactScript = gameObject.GetComponent<Interactible_Script>();
    }

    private void Update()
    {
        if (interactScript.isWorldEventAcrtive)
        {
            if (isDoorOpen)
            {
                DoorClose();
                if (LightAnimator != null)
                {
                    LightAnimator.SetBool("IsGreen", false);
                }
            }
            else if (closeBehind)
            {
                DoorOpen();
                AutoDoorClose();
            }
            else
            {
                DoorOpen();
                if(LightAnimator != null)
                {
                    LightAnimator.SetBool("IsGreen", true);
                }
            }
            interactScript.isWorldEventAcrtive = false;
        }
    }

    public void DoorOpen()
    {
        DoorAnimator.SetBool("IsDoorOpen", true);
        isDoorOpen = true;
    }

    public void AutoDoorClose()
    {
        DoorAnimator.SetBool("CloseBehind", true);
        Invoke("DoorClose", 0.1f);
    }

    public void DoorClose()
    {
        DoorAnimator.SetBool("IsDoorOpen", false);
        isDoorOpen = false;
    }

    public void ChangeLight()
    {

    }
}
