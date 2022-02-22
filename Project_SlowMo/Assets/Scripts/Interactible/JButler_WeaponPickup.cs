using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class JButler_WeaponPickup : MonoBehaviour
{

    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public

    [Space(5)]
    [Header("This Pickup")]
    [Tooltip("Choose which weapon this pickup will be.")]
    [SerializeField] JButler_Shooting.WeaponType weaponType = JButler_Shooting.WeaponType.NONE;
    [Tooltip("Determine if this pickup is fresh or is storing an old weapon.")]
    [SerializeField] bool fresh = true;
    [Space(10)]
    [Tooltip("Attach the data values for this weapon (if Handgun).")]
    [SerializeField] JButler_HandGunValues handGunValues = null;
    [Tooltip("Attach the data values for this weapon (if Shotgun).")]
    [SerializeField] JButler_ShotgunValues shotgunValues = null;
    [Tooltip("Attach the data values for this weapon (if AR).")]
    [SerializeField] JButler_ARValues aRValues = null;

    [Space(5)]
    [Header("Weapon Values")]
    [Tooltip("This is how much damage the weapon deals.")]
    [Min(1f)]
    [SerializeField] private int damage = 5;
    [Tooltip("How frequently can the weapon shoot.")]
    [Min(0f)]
    [SerializeField] private float fireRate = 0.15f;
    [Tooltip("This is how far the ray for the gun actually reaches.")]
    [Min(0f)]
    [SerializeField] private float weaponRange = 50f;
    [Tooltip("Applies force to what it hits (enemies become ragdolls).")]
    [Min(0f)]
    [SerializeField] private float hitForce = 100.0f;

    [Space(5)]
    [Header("Ammo Values")]
    [Tooltip("This the current amount of ammo the weapon has in their magizine.")]
    [Min(0f)]
    [SerializeField] int curAmmo = 0;
    [Tooltip("Spare ammo that the weapon currently has.")]
    [Min(0f)]
    [SerializeField] int sprAmmo = 0;
    [Tooltip("This the max ammo this weapon can have.")]
    [Min(0f)]
    [SerializeField] int maxAmmo = 80;
    [Tooltip("This the size of the magizine for this weapon.")]
    [Min(0f)]
    [SerializeField] int limAmmo = 15;

    [Space(5)]
    [Header("Multipliers")]
    [Tooltip("This multiplier changes how much damage a head shot does.\nMore is better hehe.")]
    [Min(1)]
    [SerializeField] int headMod = 10;
    [Tooltip("This multiplier changes how much damage a body shot does.")]
    [Min(1)]
    [SerializeField] int bodyMod = 1;

    [Space(5)]
    [Header("Models")]
    [Tooltip("Put the Handgun Model (attached to this Game Object) here.")]
    [SerializeField] GameObject handgun = null;
    [Tooltip("Put the Shotgun Model (attached to this Game Object) here.")]
    [SerializeField] GameObject shotgun = null;
    [Tooltip("Put the AR Model (attached to this Game Object) here.")]
    [SerializeField] GameObject aR = null;

    [Space(5)]
    [Header("References")]
    [Tooltip("Put the player's Shooting Brain here.")]
    [SerializeField] JButler_Shooting shooting = null;
    [Tooltip("A reference to this object's animator.")]
    [SerializeField] Animator anim = null;

    // private
    Game_UI game_ui = null;
    JButler_WeaponPickupManager wPM = null;

    // Start is called before the first frame update
    void Start()
    {

        if (Application.IsPlaying(gameObject))
        {
            game_ui = FindObjectOfType<Game_UI>();
            wPM = FindObjectOfType<JButler_WeaponPickupManager>();
            CheckFields();
            ThisWeapon();
        }
        else
        {
            DetermineValues();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            DetermineValues();
        }
        else
        {

        }
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void ChangeWeapons()
    {
        if (weaponType != shooting.CheckWeapon1() && weaponType != shooting.CheckWeapon2())
        {
            // On a fresh pickup, use the default vaules.
            DetermineValues();
            // Store the old values before switching weapons.
            shooting.TempVlaues();
            // Store old weapon.
            shooting.OurWeapon();
            // Switch weapons + other stuff.
            shooting.ChangeWeaponType(weaponType);
            // Update with new vaules.
            shooting.ChangeValues(damage, fireRate, weaponRange, hitForce, curAmmo, sprAmmo, maxAmmo, limAmmo, headMod, bodyMod);
            // Update UI.
            game_ui.UpdateAmmoText();
            // Store old weapon data.
            OldWeaponValues();
            // Change model.
            ThisWeapon();
        }
    }

    public bool NoWeapon()
    {
        if (weaponType == JButler_Shooting.WeaponType.NONE)
            return true;
        else
            return false;
    }

    public int WeaponType()
    {
        switch (weaponType)
        {
            case JButler_Shooting.WeaponType.Handgun:
                return 0;

            case JButler_Shooting.WeaponType.Shotgun:
                return 1;

            case JButler_Shooting.WeaponType.AR:
                return 2;

            default:
                return 3;
        }
    }

    public bool Fresh()
    {
        return fresh;
    }

    public int CurAmmo()
    {
        return curAmmo;
    }

    public int SprAmmo()
    {
        return sprAmmo;
    }

    /// <summary>
    /// Use this to set the saved weaponType value from OptionsSaverScript here.
    /// </summary>
    /// <param name="wT">Put the stored value for wP_WT from OptionsSaverScript here.</param>
    public void NewWeapon(int wT)
    {
        if (wT == 0)
            weaponType = JButler_Shooting.WeaponType.Handgun;
        else if (wT == 1)
            weaponType = JButler_Shooting.WeaponType.Shotgun;
        else if (wT == 2)
            weaponType = JButler_Shooting.WeaponType.AR;
        else
            weaponType = JButler_Shooting.WeaponType.NONE;
    }

    /// <summary>
    /// Set the saved value from the OptionsSaverScript here.
    /// </summary>
    /// <param name="value">Put the stored value for wP_Fresh from OptionsSaverScript here.</param>
    public void NewFresh(bool value)
    {
        fresh = value;
    }

    /// <param name="amount">Put the stored value for wP_CurAmmo from OptionsSaverScript here.</param>
    public void NewCurAmmo(int amount)
    {
        curAmmo = amount;
    }

    /// <param name="amount">Put the stored value for wP_SprAmmo from OptionsSaverScript here.</param>
    public void NewSprAmmo(int amount)
    {
        sprAmmo = amount;
    }

    public void SetSavedValues()
    {
        if (weaponType == JButler_Shooting.WeaponType.Handgun)
            SetRestOfValues(handGunValues);
        else if (weaponType == JButler_Shooting.WeaponType.Shotgun)
            SetRestOfValues(shotgunValues);
        else if (weaponType == JButler_Shooting.WeaponType.AR)
            SetRestOfValues(aRValues);
        else if (weaponType == JButler_Shooting.WeaponType.NONE)
            SetRestOfValues();

        ThisWeapon();
    }


    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void DetermineValues()
    {
        // Determine with weapon values we are using and check to see if this a fresh pickup.
        if (fresh && weaponType == JButler_Shooting.WeaponType.Handgun)
            DetermineAmmo(handGunValues);
        else if (fresh && weaponType == JButler_Shooting.WeaponType.Shotgun)
            DetermineAmmo(shotgunValues);
        else if (fresh && weaponType == JButler_Shooting.WeaponType.AR)
            DetermineAmmo(aRValues);
    }

    void OldWeapon()
    {
        weaponType = shooting.t_weaponType;
    }

    void OldWeaponValues()
    {
        OldWeapon();

        damage = shooting.t_damage;
        fireRate = shooting.t_fireRate;
        weaponRange = shooting.t_weaponRange;
        hitForce = shooting.t_hitForce;
        curAmmo = shooting.t_curAmmo;
        sprAmmo = shooting.t_sprAmmo;
        maxAmmo = shooting.t_maxAmmo;
        limAmmo = shooting.t_limAmmo;
        headMod = shooting.t_headMod;
        bodyMod = shooting.t_bodyMod;

        try
        {
            wPM.Store_WP_Data();
        }
        catch (System.NullReferenceException e)
        {
            throw new System.Exception(name + " <b><color=red>could not find the JButler_WeaponPickupManager!</color></b>\tMake sure that there is one in the scene when every you use one of these.\n" + e.Message);
        }
    }

    void DetermineAmmo(JButler_HandGunValues values)
    {
        // Take the default gun values and store them. Also let it know that this is no long a fresh pickup.
        damage = values.damage;
        fireRate = values.fireRate;
        weaponRange = values.weaponRange;
        hitForce = values.hitForce;
        curAmmo = values.curAmmo;
        sprAmmo = values.sprAmmo;
        maxAmmo = values.maxAmmo;
        limAmmo = values.limAmmo;
        headMod = values.headMod;
        bodyMod = values.bodyMod;

        if (Application.IsPlaying(gameObject))
            fresh = false;
    }

    void DetermineAmmo(JButler_ShotgunValues values)
    {
        damage = values.damage;
        fireRate = values.fireRate;
        weaponRange = values.weaponRange;
        hitForce = values.hitForce;
        curAmmo = values.curAmmo;
        sprAmmo = values.sprAmmo;
        maxAmmo = values.maxAmmo;
        limAmmo = values.limAmmo;
        headMod = values.headMod;
        bodyMod = values.bodyMod;

        if (Application.IsPlaying(gameObject))
            fresh = false;
    }

    void DetermineAmmo(JButler_ARValues values)
    {
        damage = values.damage;
        fireRate = values.fireRate;
        weaponRange = values.weaponRange;
        hitForce = values.hitForce;
        curAmmo = values.curAmmo;
        sprAmmo = values.sprAmmo;
        maxAmmo = values.maxAmmo;
        limAmmo = values.limAmmo;
        headMod = values.headMod;
        bodyMod = values.bodyMod;

        if (Application.IsPlaying(gameObject))
            fresh = false;
    }

    void SetRestOfValues(JButler_HandGunValues values)
    {
        // Take the default gun values and store them. Also let it know that this is no long a fresh pickup.
        damage = values.damage;
        fireRate = values.fireRate;
        weaponRange = values.weaponRange;
        hitForce = values.hitForce;
        maxAmmo = values.maxAmmo;
        limAmmo = values.limAmmo;
        headMod = values.headMod;
        bodyMod = values.bodyMod;
    }

    void SetRestOfValues(JButler_ShotgunValues values)
    {
        // Take the default gun values and store them. Also let it know that this is no long a fresh pickup.
        damage = values.damage;
        fireRate = values.fireRate;
        weaponRange = values.weaponRange;
        hitForce = values.hitForce;
        maxAmmo = values.maxAmmo;
        limAmmo = values.limAmmo;
        headMod = values.headMod;
        bodyMod = values.bodyMod;
    }

    void SetRestOfValues(JButler_ARValues values)
    {
        // Take the default gun values and store them. Also let it know that this is no long a fresh pickup.
        damage = values.damage;
        fireRate = values.fireRate;
        weaponRange = values.weaponRange;
        hitForce = values.hitForce;
        maxAmmo = values.maxAmmo;
        limAmmo = values.limAmmo;
        headMod = values.headMod;
        bodyMod = values.bodyMod;
    }

    void SetRestOfValues()
    {
        // Take the default gun values and store them. Also let it know that this is no long a fresh pickup.
        damage = 0;
        fireRate = 0;
        weaponRange = 0;
        hitForce = 0;
        curAmmo = 0;
        sprAmmo = 0;
        maxAmmo = 0;
        limAmmo = 0;
        headMod = 0;
        bodyMod = 0;
    }

    void ThisWeapon()
    {
        switch (weaponType)
        {
            case JButler_Shooting.WeaponType.Handgun:
                {
                    handgun.SetActive(true);
                    anim.SetBool("Handgun", true);
                    shotgun.SetActive(false);
                    anim.SetBool("Shotgun", false);
                    aR.SetActive(false);
                    anim.SetBool("AR", false);
                }
                break;

            case JButler_Shooting.WeaponType.Shotgun:
                {
                    handgun.SetActive(false);
                    anim.SetBool("Handgun", false);
                    shotgun.SetActive(true);
                    anim.SetBool("Shotgun", true);
                    aR.SetActive(false);
                    anim.SetBool("AR", false);
                }
                break;

            case JButler_Shooting.WeaponType.AR:
                {
                    handgun.SetActive(false);
                    anim.SetBool("Handgun", false);
                    shotgun.SetActive(false);
                    anim.SetBool("Shotgun", false);
                    aR.SetActive(true);
                    anim.SetBool("AR", true);
                }
                break;

            default:
                {
                    handgun.SetActive(false);
                    anim.SetBool("Handgun", false);
                    shotgun.SetActive(false);
                    anim.SetBool("Shotgun", false);
                    aR.SetActive(false);
                    anim.SetBool("AR", false);
                }
                break;
        }
    }

    void CheckFields()
    {
        if (shooting == null)
            throw new System.Exception(name + " is <b><color=red>missing a reference the player's ShootingBrain!</color></b>\tMake sure to drag and drop the player's ShootingBrain here.");

        if (anim == null)
            throw new System.Exception(name + " is <b><color=red>missing the reference to its animator!</color></b>\tMake sure to reference this object here.");

        if (game_ui == null)
            throw new System.Exception(name + " <b><color=red>could not find Game_UI!</color></b>\tPlease have an active Game_UI in the scene at all times.");
    }
}
