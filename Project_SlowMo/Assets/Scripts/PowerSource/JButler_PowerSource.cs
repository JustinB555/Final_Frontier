using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_PowerSource : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("References")]
    [Tooltip("This allows us to talk to this object's animator.")]
    [SerializeField] Animator anim = null;
    [Tooltip("This are the shields that are connected to this power source.")]
    [SerializeField] JButler_PSShield[] shields = null;
    [Tooltip("This allows us to change this object's material.")]
    [SerializeField] JButler_MaterialChange mat = null;

    [Space(5)]
    [Header("Statis")]
    [Tooltip("This will show when the power source is on or not.")]
    [SerializeField] bool on = true;

    // private
    JButler_PowerSourceManager psm = null;

    // Start is called before the first frame update
    void Start()
    {
        psm = FindObjectOfType<JButler_PowerSourceManager>();
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
        foreach (JButler_PSShield shield in shields)
            shield.Deactivate();
        mat.SecondaryMat();
        on = false;
        psm.Store_PS_Data();
    }

    public void LoadDeactivate()
    {
        anim.SetBool("Deactivated", true);
        foreach (JButler_PSShield shield in shields)
            shield.Deactivate();
        mat.SecondaryMat();
        on = false;
    }

    public bool On()
    {
        return on;
    }

    public void NewOn(bool value)
    {
        on = value;
        if (!on)
            LoadDeactivate();
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

}
