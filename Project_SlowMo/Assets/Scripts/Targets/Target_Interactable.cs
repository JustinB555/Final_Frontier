using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Interactable : MonoBehaviour
{
    [SerializeField]
    Animator TA = null;

    bool targetShield = false;


    Interactible_Script intScript = null;
    Sound_Manager sndmngr = null;

    public void Start()
    {
        intScript = gameObject.GetComponent<Interactible_Script>();
        sndmngr = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
    }

    public void Update()
    {
        if (intScript.isWorldEventAcrtive)
        {
            ShieldSpin();
            intScript.isWorldEventAcrtive = false;
        }
    }

    public void ShieldSpin()
    {
        if (targetShield == false) 
        {
            TA.SetBool("targetShield", true);
            TA.SetBool("targetShield2", true);
            TA.SetBool("targetShield3", true);
            targetShield = true;
            sndmngr.Play("Click");

        }
        else
        {
            TA.SetBool("targetShield", false);
            TA.SetBool("targetShield2", false);
            TA.SetBool("targetShield3", false);
            targetShield = false;
            sndmngr.Play("Click");
        }
        
    }



    


}
