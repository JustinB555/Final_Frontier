using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_BodyPart : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("Player Reference")]
    [Tooltip("Put the Player_Values script here.")]
    public Player_Values p = null;

    [HideInInspector] public JButler_MaterialChange m;

    bool matToggle;

    private void Start()
    {
        if (GetComponent<JButler_MaterialChange>())
            m = GetComponent<JButler_MaterialChange>();
        else if (GetComponentInChildren<JButler_MaterialChange>())
            m = GetComponentInChildren<JButler_MaterialChange>();
        CheckField();
    }

    private void Update()
    {
        CheckField();
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void ToggleMat()
    {
        matToggle = !matToggle;

        if (matToggle)
        {
            m.SecondaryMat();
            Invoke("ToggleMat", 0.2f);
        }
        else
            m.DefaultMat();
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CheckField()
    {
        if (p == null)
            throw new System.Exception(name + " <b><color=red>has not set his Main Enemy</color></b>!\tMake sure to put whoever this part is attached too inside this field.");

        if (m == null)
            throw new System.Exception(name + "  could not find the component that uses <b><color=red>JButler_MaterialChange</color></b>!\tMake sure you add one to this GameObject.");
    }
}
