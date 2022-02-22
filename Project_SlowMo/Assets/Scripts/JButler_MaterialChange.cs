using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_MaterialChange : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("Materials")]
    [Tooltip("These are the different materials this object can switch to.\n0) Will always be default\n1) Will always be secoundary\n2+ can be for whatever.")]
    [SerializeField] Material[] mats;
    [Tooltip("Reference the object you want to change materials with. Should have a mesh renderer of some kind.")]
    [SerializeField] Renderer changing = null;

    Material curMat;

    // Start is called before the first frame update
    void Start()
    {
        CheckFields();
        CheckArray();

        curMat = mats[0];
        if (changing == null && GetComponent<Renderer>())
            changing = GetComponent<Renderer>();
        else if (changing == null)
            throw new System.Exception(name + " is <b><color=red>missing its renderer reference!</color></b>\tMake sure to reference the correct Renderer for this object.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void DefaultMat()
    {
        try
        {
            curMat = mats[0];
            changing.material = curMat;
        }
        catch (MissingComponentException e)
        {
            throw new System.Exception(name + " <b><color=red>does not have a Renderer</color></b> to change materials with!\tYou must pair this script with a Mesh Renderer or put it on a child that has one.\n" + e.Message);
        }
    }

    public void SecondaryMat()
    {
        try
        {
            curMat = mats[1];
            changing.material = curMat;
        }
        catch (MissingComponentException e)
        {
            throw new System.Exception(name + " <b><color=red>does not have a Renderer</color></b> to change materials with!\tYou must pair this script with a Mesh Renderer or put it on a child that has one.\n" + e.Message);
        }
    }


    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CheckArray()
    {
        for (int i = 0; i < mats.Length; i++)
            if (mats[i] != null)
                continue;
            else
                throw new System.Exception(name + "'s <b><color=red>Mats Element " + i + " returned null</color></b>!\tMake sure to either add a material or change the array size.");
    }

    void CheckFields()
    {
        if (mats.Length == 0)
            throw new System.Exception(name + "'s Mats <b><color=red>array size is set to 0</color></b>!\tMake sure add elements to the array.");
    }
}
