using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    bool inRange = false;
    bool melee = false;
    bool cooldownActive;

    public GameObject Avatar;
    public GameObject armUpper;
    Color armColor;

    Sound_Manager sManager = null;
    void Start()
    {
        sManager = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        armColor = armUpper.GetComponent<Renderer>().material.color;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Avatar.GetComponent<MovementThirdPerson>().startMeleeAnim();
            if (inRange && !cooldownActive)
            {
                melee = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<JButler_Enemy>())
        {
            inRange = true;

            if (!cooldownActive && !other.GetComponentInParent<JButler_Enemy>().dead)
            {
                armUpper.GetComponent<Renderer>().material.color = new Color(armColor.r, 0.25f, 0.25f);
            }
            else
            {
                armUpper.GetComponent<Renderer>().material.color = new Color(armColor.r, armColor.g, armColor.b);
            }

            if (melee && !Avatar.GetComponent<MovementThirdPerson>().airTime)
            {
                if (!other.GetComponentInParent<JButler_Enemy>().dead)
                {
                    other.GetComponentInParent<JButler_Enemy>().TakeDamage(60);
                    sManager.Play("MeleeHitEnemy");
                    cooldownActive = true;
                    melee = false;
                    Invoke("Cooldown", 0.75f);
                }
                
            }
        }

        if (other.GetComponent<Destructables>())
        {

            //Debug.Log("InMeleeDestructableRange");
            inRange = true;

            if (!cooldownActive)
            {
                armUpper.GetComponent<Renderer>().material.color = new Color(armColor.r, 0.25f, 0.25f);
            }
            else
            {
                armUpper.GetComponent<Renderer>().material.color = new Color(armColor.r, armColor.g, armColor.b);
            }

            if (melee && !Avatar.GetComponent<MovementThirdPerson>().airTime)
            {
                other.GetComponent<Destructables>().TakeDamage(35);
                //sManager.Play("MeleeHitEnemy");
                cooldownActive = true;
                melee = false;
                Invoke("Cooldown", 0.75f);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<JButler_Enemy>() || other.GetComponent<Destructables>())
        {
            inRange = false;

            armUpper.GetComponent<Renderer>().material.color = new Color(armColor.r, armColor.g, armColor.b);
        }
    }

    void Cooldown()
    {
        cooldownActive = false;
    }
}
