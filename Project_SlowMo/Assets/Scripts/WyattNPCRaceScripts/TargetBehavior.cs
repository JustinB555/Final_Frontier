using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{

    [SerializeField] PlayerDoorOpen corrospondingDoor = null;
    Sound_Manager sndmngr = null;

    // Start is called before the first frame update
    void Start()
    {
        sndmngr = FindObjectOfType<Sound_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hitter()
    {
        corrospondingDoor.AddBrokeTarget();
        sndmngr.Play("TargetHit");
        gameObject.GetComponentInChildren<TargetRef>().gameObject.SetActive(false);
    }

    public void TargetReset()
    {
        gameObject.GetComponentInChildren<TargetRef>().gameObject.SetActive(true);
    }
}
