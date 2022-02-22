using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    Animator Animator = null;
    [SerializeField]
    GameObject InvsWall = null;

    // Start is called before the first frame update
    void Start()
    {
        InvsWall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InvsWall.SetActive(true);
            Animator.SetBool("ElevatorOp", true);
        }

    }
}
