using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBtPlatform : MonoBehaviour
{
    BtTimer BtTimer;

    private void Start()
    {
        BtTimer = GameObject.Find("TimerText").GetComponent<BtTimer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            

            BtTimer.StartTimer();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            BtTimer.StopTimer();
        }
    }
}
