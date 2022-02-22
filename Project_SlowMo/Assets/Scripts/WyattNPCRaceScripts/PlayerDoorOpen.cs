using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDoorOpen : MonoBehaviour
{

    int numOfBrokenTargets = 0;
    [SerializeField] int numberRequiredToOpen = 0;
    [SerializeField] TextMesh textt = null;
    Sound_Manager sndmngr = null;

    public void AddBrokeTarget()
    {
        numOfBrokenTargets = numOfBrokenTargets + 1;
        textt.text = ("Current Broken = " + numOfBrokenTargets);
        if (numOfBrokenTargets == numberRequiredToOpen)
        {
            sndmngr.Play("DoorOpen");
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sndmngr = FindObjectOfType<Sound_Manager>();
    }

}
