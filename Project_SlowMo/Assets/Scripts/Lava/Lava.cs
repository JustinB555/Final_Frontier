using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeReference] Transform checkpoint = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = checkpoint.position;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
            other.GetComponent<Player_Values>().TakeDamage(15);
        }
    }

}
