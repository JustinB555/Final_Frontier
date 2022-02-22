//#define DEBUGGING

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_PowerSourceManager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Power Sources")]
    [Tooltip("This is an array for all the Power Sources in this scene.\nThis is used in tangent with the Option Savers script to store data.")]
    [SerializeField] JButler_PowerSource[] powerSources = null;
    [SerializeField] int totalPowerSources = 0;

    // private
    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        powerSources = FindObjectsOfType<JButler_PowerSource>();
        Array.Sort(powerSources, new PowerSourceComparer());
        data = FindObjectOfType<OptionsSaverScript>();

        if (powerSources.Length == 0)
            throw new System.Exception(name + " could not find any PowerSources.\tDid you mean to add this manager? If you did, please add some Power Sources to the scene.");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find the Option Saver script!</color></b>\tMake sure that there is only ONE option saver script on at all times!");

        if (data.worldValuesUsed)
            Recall_PS_Data();

        CountPowerSources();
        Store_PS_Data();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void Store_PS_Data()
    {
        data.ps_On.Clear();

        for (int i = 0; i < powerSources.Length; i++)
        {
            //Debug.Log("<b><color=lime>[" + i + "," + powerSources[i].On() + "]</color></b>");
            data.ps_On.Add(i, powerSources[i].On());
        }

        #region DEBUGGING
#if DEBUGGING
        foreach (KeyValuePair<int, bool> ps in data.ps_On)
        {
            Debug.Log("<b><color=green>[" + ps.Key + "," + ps.Value + "]</color></b>");
        }
#endif
    #endregion
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CountPowerSources()
    {
        int count = 0;
        foreach (JButler_PowerSource wp in powerSources)
            count++;
        totalPowerSources = count;
        data.totalPowerSources = count;
    }

    void Recall_PS_Data()
    {
        foreach (KeyValuePair<int, bool> ps in data.ps_On)
        {
            //Debug.Log("<b><color=red>[" + ps.Key + "," + ps.Value + "]</color></b>");
            powerSources[ps.Key].NewOn(ps.Value);
        }
    }

}


public class PowerSourceComparer : IComparer<JButler_PowerSource>
{
    public int Compare(JButler_PowerSource a, JButler_PowerSource b)
    {
        return a.name.CompareTo(b.name);
    }
}