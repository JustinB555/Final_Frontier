using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Handgun Values (Player)", menuName = "Weapon Values/Handgun", order = 1)]
public class JButler_HandGunValues : ScriptableObject
{
    public int damage;
    public float fireRate;
    public float weaponRange;
    public float hitForce;
    public int curAmmo;
    public int sprAmmo;
    public int maxAmmo;
    public int limAmmo;
    public int headMod;
    public int bodyMod;

    public void EnemyAmmo()
    {
        curAmmo = limAmmo;
        sprAmmo = maxAmmo;
    }
}
