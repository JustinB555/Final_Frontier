using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    PickupScript pickScript = null;
    PlayerAnimatorScript plyrAnim = null;
    // Start is called before the first frame update
    void Start()
    {
        pickScript = gameObject.GetComponentInParent<PickupScript>();
        plyrAnim = FindObjectOfType<PlayerAnimatorScript>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            plyrAnim.canPlayPickUp = true;
            pickScript.PlayerInTrigger();
        }
    }
}
