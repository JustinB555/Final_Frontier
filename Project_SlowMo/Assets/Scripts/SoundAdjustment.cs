using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAdjustment : MonoBehaviour
{
    bool called = false;
    void Start()
    {
        
    }

    void Update()
    {
        if(Time.timeScale < 1)
        {
            if (!called)
            {
                AdjustmentSlow();
                called = true;
            }
        }
        else
        {
            AdjustmentNorm();
        }
    }

    void AdjustmentSlow()
    {
        GetComponent<AudioSource>().pitch = 0.7f;
    }

    void AdjustmentNorm()
    {
        GetComponent<AudioSource>().pitch = 1;
        called = false;
    }
}
