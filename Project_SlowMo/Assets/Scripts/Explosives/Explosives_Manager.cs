using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosives_Manager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Explosives")]
    [Tooltip("This is an array for all the ExplosivesBarrels in this scene.\nThis is used in tangent with the Option Savers script to store data.")]
    [SerializeField] ExplosiveBarrel[] explosives = null;
    [SerializeField] int totalExplosives = 0;

    // private
    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        explosives = FindObjectsOfType<ExplosiveBarrel>();
        Array.Sort(explosives, new ExplosiveBarrelsComparer());
        data = FindObjectOfType<OptionsSaverScript>();

        if (explosives.Length == 0)
            throw new System.Exception(name + " could not find any ExplosiveBarrels.\tDid you mean to add this manager? If you did, please add some Power Sources to the scene.");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find the Option Saver script!</color></b>\tMake sure that there is only ONE option saver script on at all times!");

        if (data.worldValuesUsed)
            Recall_Exp_Data();

        CountExplosiveBarrels();
        Store_Exp_Data();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void Store_Exp_Data()
    {
        data.exp_Boom.Clear();

        for (int i = 0; i < explosives.Length; i++)
            data.exp_Boom.Add(i, explosives[i].Detonate());
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CountExplosiveBarrels()
    {
        int count = 0;
        foreach (ExplosiveBarrel exp in explosives)
            count++;
        totalExplosives = count;
        data.totalExplosiveBarrels = count;
    }

    void Recall_Exp_Data()
    {
        foreach (KeyValuePair<int, bool> exp in data.exp_Boom)
            explosives[exp.Key].NewDetonate(exp.Value);
        foreach (ExplosiveBarrel exp in explosives)
            exp.LoadExplosive();
    }

}


public class ExplosiveBarrelsComparer : IComparer<ExplosiveBarrel>
{
    public int Compare(ExplosiveBarrel a, ExplosiveBarrel b)
    {
        return a.name.CompareTo(b.name);
    }
}
