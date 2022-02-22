using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible_Trigger : MonoBehaviour
{
    [Space(5)]
    [Header("Script Modifiers")]
    [Tooltip("If you are using the EnemyAreaCounter to keep track of enemies per room (seperate from total enemy count), use this reference and set the bool to true.")]
    [SerializeField] JButler_EnemyAreaCounter enemyCount = null;
    [Tooltip("Set to true if using the above script.")]
    [SerializeField] bool useEnemyCount = false;
    [Tooltip("To allow some doors to be interacted with at all times.")]
    [SerializeField] bool useDoor = false;
    [Tooltip("On which clear # will this interactible be available.\tUsed with EnemyCount.")]
    [SerializeField] int clearNumber = 0;

    Interactible_Script intScript = null;

    // Start is called before the first frame update
    void Start()
    {
        intScript = gameObject.GetComponentInParent<Interactible_Script>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (useEnemyCount)
        {
            if (other.CompareTag("Player") && ((enemyCount.AllDead() && enemyCount.Clears() == clearNumber) || useDoor))
            {
                intScript.PlayerInTrigger();
                useDoor = true;
            }
        }
        else if (!useEnemyCount)
        {
            if (other.CompareTag("Player"))
            {
                intScript.PlayerInTrigger();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (useEnemyCount)
        {
            if (other.CompareTag("Player") && ((enemyCount.AllDead() && enemyCount.Clears() == clearNumber) || useDoor))
            {
                intScript.PlayerLeftTrigger();
            }
        }
        else if (!useEnemyCount)
        {
            if (other.CompareTag("Player"))
            {
                intScript.PlayerLeftTrigger();
            }
        }
    }

}
