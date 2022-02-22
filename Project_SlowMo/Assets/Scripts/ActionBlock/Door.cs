using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    Animator anim = null;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("playerEnterTrigger");
        }
    }
}
