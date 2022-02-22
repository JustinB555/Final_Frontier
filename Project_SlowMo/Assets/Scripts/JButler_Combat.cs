using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Combat : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<JButler_ChangeCamera>())
            other.GetComponent<JButler_ChangeCamera>().inCombat = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<JButler_ChangeCamera>())
            other.GetComponent<JButler_ChangeCamera>().inCombat = false;
    }
}
