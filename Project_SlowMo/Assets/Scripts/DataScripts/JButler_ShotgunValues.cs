using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shotgun Values (Player)", menuName = "Weapon Values/Shotgun", order = 2)]
public class JButler_ShotgunValues : ScriptableObject
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
