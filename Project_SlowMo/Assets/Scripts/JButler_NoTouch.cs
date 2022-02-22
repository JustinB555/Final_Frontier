using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_NoTouch : MonoBehaviour
{
    [SerializeField] Transform moveHere = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.transform.position = moveHere.position;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}
