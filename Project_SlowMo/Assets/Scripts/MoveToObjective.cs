using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToObjective : MonoBehaviour
{
    ObjTextSript ots = null;
    Sound_Manager sm = null;
    Pause_Menu pm = null;

    bool hitOnce = false;
    void Start()
    {
        ots = FindObjectOfType<ObjTextSript>();
        sm = FindObjectOfType<Sound_Manager>();
        pm = FindObjectOfType<Pause_Menu>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hitOnce)
            {
                ots.objectiveText.color = Color.green;
                sm.Play("ObjectiveComplete");
                hitOnce = true;
                pm.CompleteObjective(Color.green);
            }
        }
    }
}
