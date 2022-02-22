using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AR Values (Player)", menuName = "Weapon Values/AR", order = 3)]
public class JButler_ARValues : ScriptableObject
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
