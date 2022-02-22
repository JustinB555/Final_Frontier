using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    Animator PZ1DAnim = null;
    [SerializeField]
    Animator PZ2DAnim = null;
    [SerializeField]
    Animator PZ3DAnim = null;

    GameObject tar11;
    GameObject tar12;
    GameObject tar13;
    GameObject tar21;
    GameObject tar22;
    GameObject tar23;
    GameObject tar24;
    GameObject tar31;
    GameObject tar32;
    GameObject tar33;
    GameObject tar34;

    Sound_Manager sndmngr = null;

    public void Start()
    {
        sndmngr = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();

        tar11 = GameObject.Find("Hit11");
        tar12 = GameObject.Find("Hit12");
        tar13 = GameObject.Find("Hit13");
        tar21 = GameObject.Find("Hit21");
        tar22 = GameObject.Find("Hit22");
        tar23 = GameObject.Find("Hit23");
        tar24 = GameObject.Find("Hit24");
        tar31 = GameObject.Find("Hit31");
        tar32 = GameObject.Find("Hit32");
        tar33 = GameObject.Find("Hit33");
        tar34 = GameObject.Find("Hit34");
    }

    //Targets
    public void PZL1()
    {    
        PD1();
    }

    public void PZL2()
    {
        PD2();
    }

    public void PZL3()
    {
        PD3();
    }

    //Door Anim
    public void PD1()
    {
        if (tar11.activeInHierarchy == false && tar12.activeInHierarchy == false && tar13.activeInHierarchy == false)
        {
            sndmngr.Play("DoorOpen");
            PZ1DAnim.SetTrigger("Proto7Door1");
            PZ1DAnim.SetBool("PZ1Door", true);
        }

    }
    public void PD2()
    {
        if (tar21.activeInHierarchy == false && tar22.activeInHierarchy == false && tar23.activeInHierarchy == false && tar24.activeInHierarchy == false)
        {
            sndmngr.Play("DoorOpen");
            PZ2DAnim.SetBool("PZ2Door", true);
            PZ2DAnim.SetTrigger("Proto7Door2");
        }
    }
    public void PD3()
    {
        if (tar31.activeInHierarchy == false && tar32.activeInHierarchy == false && tar33.activeInHierarchy == false && tar34.activeInHierarchy == false)
        {
            sndmngr.Play("DoorOpen");
            PZ3DAnim.SetBool("PZ3Door", true);
            
        }
    }

}
