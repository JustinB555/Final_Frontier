using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Kill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Values>())
            other.GetComponent<Player_Values>().TakeDamage(150);
    }
}
