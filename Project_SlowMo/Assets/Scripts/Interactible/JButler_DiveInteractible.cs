using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_DiveInteractible : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Interactible Object")]
    [Tooltip("Put what object you want to interact with here.")]
    [SerializeField] GameObject interactible = null;
    [Tooltip("If you need one interact to turn on once a condition has been meet.")]
    [SerializeField] GameObject nextInteract = null;

    [Space(5)]
    [Header("Gates and Valves")]
    [Tooltip("If this interactible is a gate, set this to true.")]
    [SerializeField] bool gate = false;
    [Tooltip("If this interactible is a valve, set this to true.")]
    [SerializeField] bool valve = false;
    [Tooltip("Hide the next interact.")]
    [SerializeField] bool hide = false;
    [Tooltip("Reveal the next interact.")]
    [SerializeField] bool reveal = false;

    // private
    MovementThirdPerson mtp = null;

    // Start is called before the first frame update
    void Start()
    {
        mtp = FindObjectOfType<MovementThirdPerson>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Collision Events
    //////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovementThirdPerson>() && mtp.bdActive && gate)
            interactible.SetActive(false);

        if (other.GetComponent<MovementThirdPerson>() && mtp.bdActive && valve)
            interactible.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<MovementThirdPerson>() && mtp.bdActive && gate)
            interactible.SetActive(false);

        if (other.GetComponent<MovementThirdPerson>() && mtp.bdActive && valve)
            interactible.SetActive(true);

        if (other.GetComponent<MovementThirdPerson>() && !mtp.bdActive && hide && nextInteract != null)
            nextInteract.SetActive(false);

        if (other.GetComponent<MovementThirdPerson>() && !mtp.bdActive && reveal && nextInteract != null)
            nextInteract.SetActive(true);
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////


    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

}
