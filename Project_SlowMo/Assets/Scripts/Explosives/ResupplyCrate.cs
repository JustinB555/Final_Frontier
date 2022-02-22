using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResupplyCrate : MonoBehaviour
{
    [Tooltip("Does this crate resupply all Ammuntion?")]
    [SerializeField] bool ammo = false;

    [Tooltip("Does this crate resupply all Grenades?")]
    [SerializeField] bool grenades = false;

    [SerializeField] GameObject AmmoBoxes;
    [SerializeField] GameObject Grenades;

    Player_Values pv = null;
    Sound_Manager sm = null;
    Game_UI gui = null;
    [Tooltip("Player ShootingBrain goes here")]
    [SerializeField] JButler_Shooting jshoot = null;
    ResupplyManager rm;

    bool withinRange = false;
    void Start()
    {
        pv = FindObjectOfType<Player_Values>();
        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        gui = GameObject.Find("Game_UI").GetComponent<Game_UI>();
        rm = GameObject.Find("ResupplyManager").GetComponent<ResupplyManager>();

        if (!ammo)
        {
            AmmoBoxes.SetActive(false);
        }
        if (!grenades)
        {
            Grenades.SetActive(false);
        }
    }

    void Update()
    {
        if (withinRange)
        {
            //Debug.Log("Player within range of Resupply Crate");

            if (rm.isOneActive)
            {
                gui.interact.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                sm.Play("Resupply");
                if (ammo)
                {
                    jshoot.MaxAmmoAll();
                }

                if (grenades)
                {
                    pv.grenadeCount += 3;
                }
            }
        }
        else
        {
            if (!rm.isOneActive)
            {
                gui.interact.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            withinRange = true;
            rm.isOneActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            withinRange = false;
            rm.isOneActive = false;
        }
    }
}
