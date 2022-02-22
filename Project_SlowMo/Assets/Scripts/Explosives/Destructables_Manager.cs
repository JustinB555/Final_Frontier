using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructables_Manager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Destructibles")]
    [Tooltip("This is an array for all the Destructables in this scene.\nThis is used in tangent with the Option Savers script to store data.")]
    [SerializeField] Destructables[] destructables = null;
    [SerializeField] int totalDestructables = 0;

    // private
    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        destructables = FindObjectsOfType<Destructables>();
        Array.Sort(destructables, new DestructiblesComparer());
        data = FindObjectOfType<OptionsSaverScript>();

        if (destructables.Length == 0)
            throw new System.Exception(name + " could not find any Destructables.\tDid you mean to add this manager? If you did, please add some Power Sources to the scene.");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find the Option Saver script!</color></b>\tMake sure that there is only ONE option saver script on at all times!");

        if (data.worldValuesUsed)
            Recall_DES_Data();

        CountDES();
        Store_DES_Data();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void Store_DES_Data()
    {
        data.des_Destroyed.Clear();

        for (int i = 0; i < destructables.Length; i++)
            data.des_Destroyed.Add(i, destructables[i].Destroyed());
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CountDES()
    {
        int count = 0;
        foreach (Destructables des in destructables)
            count++;
        totalDestructables = count;
        data.totalDestructibles = count;
    }

    void Recall_DES_Data()
    {
        foreach (KeyValuePair<int, bool> des in data.des_Destroyed)
            destructables[des.Key].NewDestroyed(des.Value);
        foreach (Destructables des in destructables)
            des.LoadDestoyed();
    }

}


public class DestructiblesComparer : IComparer<Destructables>
{
    public int Compare(Destructables a, Destructables b)
    {
        return a.name.CompareTo(b.name);
    }
}
