using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_PSShield : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("References")]
    [Tooltip("This allows us to talk to this object's animator.")]
    [SerializeField] Animator anim = null;

    // private

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void Deactivate()
    {
        anim.SetBool("Deactivated", true);
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////
}
