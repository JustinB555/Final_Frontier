using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen_Manager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Doors")]
    [Tooltip("This is an array for all the InteractDoor_Unlocked in this scene.\nThis is used in tangent with the Option Savers script to store data.")]
    [SerializeField] DoorOpen[] doors = null;
    [SerializeField] int totalDoors = 0;

    // private
    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        doors = FindObjectsOfType<DoorOpen>();
        Array.Sort(doors, new DoorOpenComparer());
        data = FindObjectOfType<OptionsSaverScript>();

        if (doors.Length == 0)
            throw new System.Exception(name + " could not find any PowerSources.\tDid you mean to add this manager? If you did, please add some Power Sources to the scene.");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find the Option Saver script!</color></b>\tMake sure that there is only ONE option saver script on at all times!");

        if (data.worldValuesUsed)
            Recall_Door_Data();

        CountDoors();
        Store_Door_Data();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void Store_Door_Data()
    {
        data.dr_WasOpen.Clear();

        for (int i = 0; i < doors.Length; i++)
            data.dr_WasOpen.Add(i, doors[i].WasOpen());
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CountDoors()
    {
        int count = 0;
        foreach (DoorOpen d in doors)
            count++;
        totalDoors = count;
        data.totalDoors = count;
    }

    void Recall_Door_Data()
    {
        foreach (KeyValuePair<int, bool> ws in data.dr_WasOpen)
            doors[ws.Key].NewWasOpen(ws.Value);
        foreach (DoorOpen d in doors)
            d.CheckWasOpen();
    }

}


public class DoorOpenComparer : IComparer<DoorOpen>
{
    public int Compare(DoorOpen a, DoorOpen b)
    {
        return a.name.CompareTo(b.name);
    }
}