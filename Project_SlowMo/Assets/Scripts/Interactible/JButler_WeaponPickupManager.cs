using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_WeaponPickupManager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Weapon Pickups")]
    [Tooltip("This is an array for all the weapon pickups in this scene.\nThis is used in tangent with the Option Savers script to store data.")]
    [SerializeField] JButler_WeaponPickup[] weaponPickups = null;
    [SerializeField] int totalWeaponPickups = 0;

    // private
    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        weaponPickups = FindObjectsOfType<JButler_WeaponPickup>();
        Array.Sort(weaponPickups, new WeaponPickupComparer());
        data = FindObjectOfType<OptionsSaverScript>();

        if (weaponPickups.Length == 0)
            throw new System.Exception(name + " could not find any weapon pickups.\tDid you mean to add this manager? If you did, please add some weapon pickups to the scene.");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find the Option Saver script!</color></b>\tMake sure that there is only ONE option saver script on at all times!");

        if (data.worldValuesUsed)
            Recall_WP_Data();

        CountWeaponPickups();
        Store_WP_Data();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    /// <summary>
    /// Store the neccessary parts of the weapon pickup script here, so that we can recall them later. The values we need are: the WeaponType (int), the fresh (bool), the curAmmo (int), and the sprAmmo (int).
    /// </summary>
    public void Store_WP_Data()
    {
        data.wP_WT.Clear();
        data.wP_Fresh.Clear();
        data.wP_CurAmmo.Clear();
        data.wP_SprAmmo.Clear();

        for (int i = 0; i < weaponPickups.Length; i++)
        {
            data.wP_WT.Add(i, weaponPickups[i].WeaponType());
            data.wP_Fresh.Add(i, weaponPickups[i].Fresh());
            data.wP_CurAmmo.Add(i, weaponPickups[i].CurAmmo());
            data.wP_SprAmmo.Add(i, weaponPickups[i].SprAmmo());
        }
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CountWeaponPickups()
    {
        int count = 0;
        foreach (JButler_WeaponPickup wp in weaponPickups)
            count++;
        totalWeaponPickups = count;
        data.totalWeaponPickups = count;
    }

    /// <summary>
    /// Recall the stored values from the OptionsSaverScript.
    /// </summary>
    void Recall_WP_Data()
    {
        foreach (KeyValuePair<int, int> weaponType in data.wP_WT)
            weaponPickups[weaponType.Key].NewWeapon(weaponType.Value);

        foreach (KeyValuePair<int, bool> fresh in data.wP_Fresh)
            weaponPickups[fresh.Key].NewFresh(fresh.Value);

        foreach (KeyValuePair<int, int> curAmmo in data.wP_CurAmmo)
            weaponPickups[curAmmo.Key].NewCurAmmo(curAmmo.Value);

        foreach (KeyValuePair<int, int> sprAmmo in data.wP_SprAmmo)
            weaponPickups[sprAmmo.Key].NewSprAmmo(sprAmmo.Value);

        foreach (JButler_WeaponPickup wp in weaponPickups)
            wp.SetSavedValues();
    }

}


public class WeaponPickupComparer : IComparer<JButler_WeaponPickup>
{
    public int Compare(JButler_WeaponPickup a, JButler_WeaponPickup b)
    {
        return a.name.CompareTo(b.name);
    }
}
