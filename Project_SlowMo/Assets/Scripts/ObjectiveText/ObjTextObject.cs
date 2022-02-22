using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTextObject : MonoBehaviour
{
    [TextArea(5, 10)]
    public string objective = null;
    ObjTextSript ots = null;
    Sound_Manager sm;
    Pause_Menu pm;

    private void Start()
    {
        ots = FindObjectOfType<ObjTextSript>();
        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        pm = FindObjectOfType<Pause_Menu>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //ots.InsertText("Current Objective: " + objective);

            ots.objectiveText.text = objective;

            sm.Play("NewObjective");

            pm.NewObjective(objective);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ots.objectiveText.text = "";
            Destroy(gameObject);
        }
    }
}
