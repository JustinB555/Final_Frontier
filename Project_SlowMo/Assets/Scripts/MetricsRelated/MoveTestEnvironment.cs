using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTestEnvironment : MonoBehaviour
{
    bool isInTrigger = false;
    float timerTime = 0;
    void Start()
    {

    }

    void Update()
    {
        if (isInTrigger)
        {
            timerTime += Time.deltaTime;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
            Debug.Log("MoveTestTimer: " + timerTime);
            timerTime = 0;
        }
    }
}
