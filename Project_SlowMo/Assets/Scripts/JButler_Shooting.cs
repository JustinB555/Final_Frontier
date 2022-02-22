//#define TEST
//#define TEST2
//#define SPREAD
//#define AMMO
//#define OLD
//#define FIRERATE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class JButler_Shooting : MonoBehaviour
{
    public enum WeaponType { Handgun, Shotgun, AR, NONE }

    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public or Serialized

    [Space(5)]
    [Header("User")]
    [Tooltip("Who is using this script, an enemy or the player?\nPlayer = true\nEnemy = false")]
    [SerializeField] bool isPlayer = false;

    [Space(5)]
    [Header("Current Weapon")]
    [Tooltip("This is the current weapon that is in the main hand.")]
    [SerializeField] private WeaponType weaponType = WeaponType.Handgun;

    [Space(5)]
    [Header("Avaiable Weapons")]
    [Tooltip("This is your starting weapon.\nWrite down the name of the weapon you want to use (don't mispell)")]
    [SerializeField] WeaponType weapon1 = WeaponType.Handgun;
    [Tooltip("This is the second weapon you have\nLeave blank if you don't want a second weapon.")]
    [SerializeField] WeaponType weapon2 = WeaponType.Shotgun;

    [Space(5)]
    [Header("Data")]
    [Tooltip("Put the Player's Handgun values here.\nIF this is an enemy, put the Enemy's Handgun values here instead.\n\nChange these values instead of the ones below.")]
    [SerializeField] JButler_HandGunValues handgunValues = null;
    [Tooltip("Put the Player's Shotgun values here.\nIF this is an enemy, put the Enemy's Shotgun values here instead.\n\nChange these values instead of the ones below.")]
    [SerializeField] JButler_ShotgunValues shotgunValues = null;
    [Tooltip("Put the Player's AR values her.\nIF this is an enemy, put the Enemy's AR values here istead.\n\nChange these values instead of the ones below.")]
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
    [Tooltip("This multiplier changes how fast you shoot during Bullet Time.")]
    [SerializeField] float fireMod = 0.05f;

    [Space(5)]
    [Header("Weapon Points")]
    [Tooltip("These are the weapon locations that are on the end of the barrels.\nIt is used to shoot the ray out from the weapon, so it is VERY important that these are setup properly.")]
    [SerializeField] private JButler_ShootFrom[] locations = null;

    [Space(5)]
    [Header("Raycast Layers")]
    [Tooltip("Choose which layers the raycast can hit.")]
    [SerializeField] LayerMask layers;
    [Tooltip("This will ignore everything but enemies and targets.")]
    [SerializeField] LayerMask aimLayers;

    [Space(5)]
    [Header("VFX")]
    [Tooltip("This is to show where the 'bullet' hit.")]
    [SerializeField] private GameObject bulletHole = null;
    [Tooltip("This is to show that the player is shooting, by showing them a bullet trail.")]
    [SerializeField] ParticleSystem bulletTrace = null;
    [Tooltip("Put the Handgun Models here.")]
    [SerializeField] GameObject[] handguns = null;
    [Tooltip("Put the Shotgun Models here.")]
    [SerializeField] GameObject[] shotguns = null;
    [Tooltip("Put the AR Models here.")]
    [SerializeField] GameObject[] aRs = null;

    [Space(5)]
    [Header("UI")]
    [Tooltip("Put the pistol UI image here.")]
    [SerializeField] GameObject pistolUI = null;
    [Tooltip("Put the pistol UI S image here.\nThis shows what the secondary weapon you have avaliable.")]
    [SerializeField] GameObject pistol2UI = null;
    [Tooltip("Put the shotgun's UI image here.")]
    [SerializeField] GameObject shottyUI = null;
    [Tooltip("Put ShotgunUI (S) image here.")]
    [SerializeField] GameObject shotty2UI = null;
    [Tooltip("Put the AR's UI image here.")]
    [SerializeField] GameObject aR_UI = null;
    [Tooltip("Put the AR_UI (S) image here.")]
    [SerializeField] GameObject aR2_UI = null;

    public Target tar = null;

    [HideInInspector] public WeaponType t_weaponType;
    [HideInInspector] public int t_damage = 5;
    [HideInInspector] public float t_fireRate = 0.15f;
    [HideInInspector] public float t_weaponRange = 50f;
    [HideInInspector] public float t_hitForce = 100.0f;
    [HideInInspector] public int t_curAmmo = 0;
    [HideInInspector] public int t_sprAmmo = 0;
    [HideInInspector] public int t_maxAmmo = 80;
    [HideInInspector] public int t_limAmmo = 15;
    [HideInInspector] public int t_headMod = 10;
    [HideInInspector] public int t_bodyMod = 1;
    [HideInInspector] public bool isReloading = false;

    // private

    float gunTimer = 0.0f;
    float reloadTimer = 0.0f;
    bool continuious = false;
    JButler_ShootFrom here = null;
    JButler_Aim aim = null;
    JButler_ChangeCamera cams = null;
    Sound_Manager sndmngr = null;
    Player_Values pv = null;
    Game_UI game_ui = null;
    DebugScript ds = null;
    OptionsSaverScript data = null;
    MovementThirdPerson mtp = null;
    CheckpointManager cm = null;
    Reload_UI rui = null;
    bool rayCheck = false;
    bool shootingRay = false;
    bool checkedTime = false;
    bool checkOld = true;
    bool reloading = false;
    bool reloadSound = false;

    //////////////////////////////////////////////////
    // Compiler
    //////////////////////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.IsPlaying(gameObject))
            DetermineWeapon();
        else
        {
            aim = GetComponent<JButler_Aim>();
            cams = FindObjectOfType<JButler_ChangeCamera>();
            sndmngr = FindObjectOfType<Sound_Manager>();
            pv = FindObjectOfType<Player_Values>();
            ds = FindObjectOfType<DebugScript>();
            data = FindObjectOfType<OptionsSaverScript>();
            game_ui = FindObjectOfType<Game_UI>();
            mtp = FindObjectOfType<MovementThirdPerson>();
            cm = FindObjectOfType<CheckpointManager>();
            if (isPlayer)
            {
                rui = FindObjectOfType<Reload_UI>();
            }

            checkOld = true;


            CheckFields();
            GunCheck();
            StartingWeapon();
            DetermineWeapon();
            RecallOldValues();
            ImageCheck();
            StartingWeaponIcon();
            SwitchWeapons();
            SwitchWeaponsEnemies();
            SetHere();
            SaveValues();
            //tar = GameObject.Find("Target").GetComponentInChildren<Target>();
            //tar = FindObjectOfType<Target>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Editor Logic
        if (!Application.IsPlaying(gameObject))
        {
            DetermineWeapon();
        }
        // Play Logic
        else
        {
            CheckFields();

            gunTimer += Time.deltaTime;

            Inputs();
            SwitchWeapons();
            SaveOldValues();
            //SetHere();

            #region DEBUGGING
#if SPREAD
            aim.RandomSpread();
            Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.spread[0]), Color.green);
            Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.spread[1]), Color.green);
            Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.spread[2]), Color.green);
            Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.spread[3]), Color.green);
            Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.spread[4]), Color.green);
#endif
            #endregion
        }

        if (reloading)
        {
            if(Time.timeScale < 1)
            {
                reloadTimer += (Time.deltaTime * 7);
                ReloadAmmo();
            }
            else
            {
                reloadTimer += Time.deltaTime;
                ReloadAmmo();
            }
        }
    }

    private void FixedUpdate()
    {
        if (Application.IsPlaying(gameObject) && Time.timeScale > 0)
        {
            AimGun();
        }
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public Transform Here()
    {
        return here.transform;
    }

    public float GunTimer()
    {
        return gunTimer;
    }

    public float FireRate()
    {
        return fireRate;
    }

    public int CurAmmo()
    {
        return curAmmo;
    }

    public void AimGun()
    {
        if (here != null && aim != null)
            aim.AimAt(here.transform, aim.target);
        #region DEBUGGING
#if TEST
        Debug.DrawLine(here.transform.position, aim.tarPoint.position, Color.red); // Correct Direction
        Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform,aim.tarPoint), Color.green); // Should Match
#endif
        #endregion
    }

    public void Shoot()
    {
        gunTimer = 0.0f;

        Vector3 rayOrigin = Vector3.zero;
        if (here != null && aim != null)
            rayOrigin = here.transform.position;
        else
            CheckFields();
        RaycastHit hit;

        #region DEBUGGING
#if TEST2
        Debug.Log("<b><color=magenta>Here Pos</color></b>: <b><color=orange>" + here.transform.position + "</color></b>");
        Debug.DrawLine(here.transform.position - Vector3.forward * 0.1f, here.transform.position + Vector3.forward * 0.1f, Color.magenta, 50.0f);
        Debug.DrawLine(here.transform.position - Vector3.right * 0.1f, here.transform.position + Vector3.right * 0.1f, Color.magenta, 50.0f);
        Debug.DrawLine(here.transform.position - Vector3.up * 0.1f, here.transform.position + Vector3.up * 0.1f, Color.magenta, 50.0f);

        Debug.DrawLine(here.transform.position, aim.target.position, Color.magenta);
        //Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.target.transform), Color.yellow);
#endif
        #endregion

        if (weaponType == WeaponType.Handgun && !FindObjectOfType<MovementThirdPerson>().Rolling())
        {
            if (curAmmo > 0)
            {
                sndmngr.Play("ShootHandgun");

                TraceLocation(rayOrigin);
                bulletTrace.Play();
                curAmmo--;
                SaveValues();
                StoreValues();
            }
            else if (curAmmo <= 0 && isPlayer)
            {
                sndmngr.Play("EmptyMag");
                //Debug.LogError(weaponType + " is out of ammo!\t<b><color=orange>Reload!</color></b>");
                if (!game_ui.ammoNotif.activeSelf && sprAmmo > 0)
                {
                    game_ui.EnableAmmoNotif("Reload");
                }
                else if (!game_ui.ammoNotif.activeSelf && sprAmmo <= 0)
                {
                    game_ui.EnableAmmoNotif("LowAmmo");
                }
                return;
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.target.transform), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);
                }
            }
        }
        else if (weaponType == WeaponType.Shotgun && !FindObjectOfType<MovementThirdPerson>().Rolling())
        {
            if (curAmmo > 0)
            {
                sndmngr.Play("ShootShotgun");

                TraceLocation(rayOrigin);
                bulletTrace.Play();
                aim.RandomSpread();
                curAmmo--;
                SaveValues();
                StoreValues();
            }
            else if (curAmmo <= 0 && isPlayer)
            {
                sndmngr.Play("EmptyMag");
                //Debug.LogError(weaponType + " is out of ammo!\t<b><color=orange>Reload!</color></b>");
                if (!game_ui.ammoNotif.activeSelf && sprAmmo > 0)
                {
                    game_ui.EnableAmmoNotif("Reload");
                }
                else if (!game_ui.ammoNotif.activeSelf && sprAmmo <= 0)
                {
                    game_ui.EnableAmmoNotif("LowAmmo");
                }
                return;
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.target.transform), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);

                }
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.spread[0]), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);

                }
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.spread[1]), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);

                }
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.spread[2]), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);

                }
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.spread[3]), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);

                }
            }

            if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.spread[4]), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);

                }
            }
        }
        else if (weaponType == WeaponType.AR && !FindObjectOfType<MovementThirdPerson>().Rolling())
        {
            if (curAmmo > 0)
            {
                RandomARSound();

                TraceLocation(rayOrigin);
                bulletTrace.Play();
                curAmmo--;
                SaveValues();
                StoreValues();
            }
            else if (curAmmo <= 0 && isPlayer)
            {
                sndmngr.Play("EmptyMag");
                //Debug.LogError(weaponType + " is out of ammo!\t<b><color=orange>Reload!</color></b>");
                if (!game_ui.ammoNotif.activeSelf && sprAmmo > 0)
                {
                    game_ui.EnableAmmoNotif("Reload");
                }
                else if (!game_ui.ammoNotif.activeSelf && sprAmmo <= 0)
                {
                    game_ui.EnableAmmoNotif("LowAmmo");
                }
                return;
            }

            if (!continuious && Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.target.transform), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);
                }
            }
            else if (continuious && Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.RandomShoot()), out hit, weaponRange, layers))
            {
                try
                {
                    if (hit.collider.GetComponent<JButler_EnemyTarget>())
                        hit.collider.GetComponent<JButler_EnemyTarget>().SetBulletHole(bulletHole, hit);
                    else if (hit.collider.GetComponent<JButler_Shield>())
                        hit.collider.GetComponent<JButler_Shield>().SetBulletHole(bulletHole, hit);
                    else
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Damage(hit);
                }
                catch (UnassignedReferenceException e)
                {
                    if (bulletHole == null)
                        throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\tDid you forget to add a GameObject to Bullet Hole?\n" + e.Message);
                }
            }
        }
    }

    public bool ShootingRay()
    {
        Vector3 rayOrigin = Vector3.zero;
        if (here != null && aim != null)
            rayOrigin = here.transform.position;
        else
            CheckFields();
        RaycastHit hit;
        bool ray = false;
        if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.target.transform), out hit, weaponRange, layers) && (hit.collider.GetComponent<JButler_EnemyTarget>() || hit.collider.GetComponent<JButler_Target>()))
        {
            ray = true;
            shootingRay = true;
            return ray;
        }
        else
        {
            ray = false;
            shootingRay = false;
            return ray;
        }
    }

    public RaycastHit ShootingHit()
    {
        Vector3 rayOrigin = Vector3.zero;
        if (here != null && aim != null)
            rayOrigin = here.transform.position;
        else
            CheckFields();
        RaycastHit hit;

        Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.target.transform), out hit, weaponRange, layers);

        return hit;
    }

    public bool RayCheck()
    {
        Vector3 rayOrigin = Vector3.zero;
        if (here != null && aim != null)
            rayOrigin = here.transform.position;
        else
            CheckFields();
        RaycastHit hit;
        bool ray = false;
        #region DEBUGGING
#if TEST
        Debug.Log("<b><color=magenta>Here Pos</color></b>: <b><color=orange>" + here.transform.position + "</color></b>");
        Debug.DrawLine(here.transform.position - Vector3.forward * 0.1f, here.transform.position + Vector3.forward * 0.1f, Color.magenta, 50.0f);
        Debug.DrawLine(here.transform.position - Vector3.right * 0.1f, here.transform.position + Vector3.right * 0.1f, Color.magenta, 50.0f);
        Debug.DrawLine(here.transform.position - Vector3.up * 0.1f, here.transform.position + Vector3.up * 0.1f, Color.magenta, 50.0f);

        Debug.DrawLine(here.transform.position, aim.target.position, Color.magenta);
        //Debug.DrawRay(here.transform.position, aim.RayDirection(here.transform, aim.target.transform), Color.yellow);
#endif
        #endregion

        if (Physics.Raycast(rayOrigin, aim.RayDirection(here.transform, aim.target.transform), out hit, weaponRange, aimLayers))
        {
            ray = true;
            rayCheck = true;
            return ray;
        }
        else
        {
            ray = false;
            rayCheck = false;
            return ray;
        }
    }

    public void OurWeapon()
    {
        t_weaponType = weaponType;
    }

    public WeaponType OurCurrentWeapon()
    {
        return weaponType;
    }

    public WeaponType CheckWeapon1()
    {
        return weapon1;
    }

    public WeaponType CheckWeapon2()
    {
        return weapon2;
    }

    public void ChangeWeaponType(WeaponType weapon)
    {
        // What you are currently holding.
        if (weapon1 == weaponType)
        {
            // Update it with the new weapon.
            weapon1 = weapon;
            // Push that change to the actual weapon.
            weaponType = weapon1;
            // Make a SFX.
            sndmngr.Play("WeaponSwap");
            // Change the UI and model.
            SwitchWeapons();
            // Update where to shoot from.
            SetHere();
        }
        else if (weapon2 == weaponType)
        {
            weapon2 = weapon;
            weaponType = weapon2;
            sndmngr.Play("WeaponSwap");
            SwitchWeapons();
            SetHere();
        }
    }

    public void TempVlaues()
    {
        if (Time.timeScale < 1)
            fireRate = fireRate / fireMod;
        #region DEBUGGING
#if FIRERATE
        Debug.LogError("<b><color=yellow>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
        #endregion

        // Step 1: Store old values
        t_damage = damage;
        t_fireRate = fireRate;
        t_weaponRange = weaponRange;
        t_hitForce = hitForce;
        t_curAmmo = curAmmo;
        t_sprAmmo = sprAmmo;
        t_maxAmmo = maxAmmo;
        t_limAmmo = limAmmo;
        t_headMod = headMod;
        t_bodyMod = bodyMod;
    }

    public void ChangeValues(int _damage, float _fireRate, float _weaponRange, float _hitForce, int _curAmmo, int _sprAmmo, int _maxAmmo, int _limAmmo, int _headMod, int _bodyMod)
    {
        // Step 2: Push new values.
        damage = _damage;
        fireRate = _fireRate;
        weaponRange = _weaponRange;
        hitForce = _hitForce;
        curAmmo = _curAmmo;
        sprAmmo = _sprAmmo;
        maxAmmo = _maxAmmo;
        limAmmo = _limAmmo;
        headMod = _headMod;
        bodyMod = _bodyMod;

        if (Time.timeScale < 1)
            fireRate = fireRate * fireMod;
        #region DEBUGGING
#if FIRERATE
        Debug.LogError("<b><color=yellow>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
        #endregion

        SaveWeapon();
        SaveValues();
        StoreValues();
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }

    public void MaxAmmo()
    {
        curAmmo = limAmmo;
        sprAmmo = maxAmmo;

        if (isPlayer)
        {
            SaveValues();
            StoreValues();
            game_ui.UpdateAmmoText();
        }
    }

    public void MaxAmmoAll()
    {
        MaxAmmo();

        if (weaponType == weapon1 && isPlayer)
        {
            switch (weapon2)
            {
                case WeaponType.Handgun:
                    // Happen in Editor
                    handgunValues.curAmmo = handgunValues.limAmmo;
                    handgunValues.sprAmmo = handgunValues.maxAmmo;
                    // Happen in Application
                    data.w1_curAmmo = handgunValues.limAmmo;
                    data.w1_sprAmmo = handgunValues.maxAmmo;
                    break;

                case WeaponType.Shotgun:
                    shotgunValues.curAmmo = shotgunValues.limAmmo;
                    shotgunValues.sprAmmo = shotgunValues.maxAmmo;
                    data.w2_curAmmo = shotgunValues.limAmmo;
                    data.w2_sprAmmo = shotgunValues.maxAmmo;
                    break;

                case WeaponType.AR:
                    aRValues.curAmmo = aRValues.limAmmo;
                    aRValues.sprAmmo = aRValues.maxAmmo;
                    data.w3_curAmmo = aRValues.limAmmo;
                    data.w3_sprAmmo = aRValues.maxAmmo;
                    break;

                default:
                    break;
            }
        }
        else if (weaponType == weapon2 && isPlayer)
        {
            switch (weapon1)
            {
                case WeaponType.Handgun:
                    // Happen in Editor
                    handgunValues.curAmmo = handgunValues.limAmmo;
                    handgunValues.sprAmmo = handgunValues.maxAmmo;
                    // Happen in Application
                    data.w1_curAmmo = handgunValues.limAmmo;
                    data.w1_sprAmmo = handgunValues.maxAmmo;
                    break;

                case WeaponType.Shotgun:
                    shotgunValues.curAmmo = shotgunValues.limAmmo;
                    shotgunValues.sprAmmo = shotgunValues.maxAmmo;
                    data.w2_curAmmo = shotgunValues.limAmmo;
                    data.w2_sprAmmo = shotgunValues.maxAmmo;
                    break;

                case WeaponType.AR:
                    aRValues.curAmmo = aRValues.limAmmo;
                    aRValues.sprAmmo = aRValues.maxAmmo;
                    data.w3_curAmmo = aRValues.limAmmo;
                    data.w3_sprAmmo = aRValues.maxAmmo;
                    break;

                default:
                    break;
            }
        }
    }

    public void AddAmmo(int amount)
    {
        sprAmmo += amount;
        if (sprAmmo > maxAmmo)
            sprAmmo = maxAmmo;
        SaveValues();
        StoreValues();
        game_ui.UpdateAmmoText();
    }

    public int PickupAmmo()
    {
        int amount = 0;

        if ((sprAmmo + limAmmo) > maxAmmo)
            amount = maxAmmo - sprAmmo;
        else
            amount = limAmmo;

        game_ui.UpdateAmmoText();
        return amount;
    }

    public void SubtractAmmo(int amount)
    {
        sprAmmo -= amount;
        if (sprAmmo < 0)
            sprAmmo = 0;
        SaveValues();
        StoreValues();
        game_ui.UpdateAmmoText();
    }

    public void ReloadAmmo()
    {
        if(sprAmmo != 0 && curAmmo < limAmmo)
        {
            reloading = true;
            isReloading = true;
        }
        if(weaponType == WeaponType.Handgun)
        {
            if (!reloadSound)
            {
                if(sprAmmo != 0 && curAmmo < limAmmo)
                {
                    sndmngr.Play("HandgunReload");
                    reloadSound = true;
                    rui.startReload = true;
                    rui.totalReload = 1;
                    if(cams.Cam1Priority() == 0)
                    {
                        cams.ResetTimer();
                        cams.ShootingCam();
                    }
                }
            }
            if (reloadTimer > 1)
            {
                if (sprAmmo != 0 && curAmmo < limAmmo)
                {
                    int temp = limAmmo - curAmmo;

                    if (temp > sprAmmo)
                    {
                        curAmmo += sprAmmo;
                        sprAmmo -= sprAmmo;
                    }
                    else
                    {
                        sprAmmo -= temp;
                        curAmmo += temp;
                        
                    }

                    reloading = false;
                    reloadSound = false;
                    reloadTimer = 0;
                    isReloading = false;
                    SaveValues();
                    StoreValues();
                    game_ui.UpdateAmmoText();
                }
                else if (curAmmo <= 0 && sprAmmo <= 0)
                {
                    //Debug.LogError(weaponType + " is out of ammo!\t<b><color=orange>Look for some ammo to pickup!</color></b>");
                    if (!game_ui.ammoNotif.activeSelf)
                    {
                        game_ui.EnableAmmoNotif("LowAmmo");
                    }
                }
            }
            
        }
        else if(weaponType == WeaponType.AR)
        {
            if (!reloadSound)
            {
                if(sprAmmo != 0 && curAmmo < limAmmo)
                {
                    sndmngr.Play("ARReload");
                    reloadSound = true;
                    rui.startReload = true;
                    rui.totalReload = 0.5f;
                    if (cams.Cam1Priority() == 0)
                    {
                        cams.ResetTimer();
                        cams.ShootingCam();
                    }
                }
            }
            else
            {
                cams.ResetTimer();
            }
            if (reloadTimer > 2)
            {
                if (sprAmmo != 0 && curAmmo < limAmmo)
                {
                    int temp = limAmmo - curAmmo;

                    if (temp > sprAmmo)
                    {
                        curAmmo += sprAmmo;
                        sprAmmo -= sprAmmo;
                    }
                    else
                    {
                        sprAmmo -= temp;
                        curAmmo += temp;
                        
                    }

                    reloading = false;
                    reloadSound = false;
                    reloadTimer = 0;
                    isReloading = false;
                    SaveValues();
                    StoreValues();
                    game_ui.UpdateAmmoText();
                }
                else if (curAmmo <= 0 && sprAmmo <= 0)
                {
                    //Debug.LogError(weaponType + " is out of ammo!\t<b><color=orange>Look for some ammo to pickup!</color></b>");
                    if (!game_ui.ammoNotif.activeSelf)
                    {
                        game_ui.EnableAmmoNotif("LowAmmo");
                    }
                }
            }
        }
        else if(weaponType == WeaponType.Shotgun)
        {
            if (!reloadSound)
            {
                if(sprAmmo != 0 && curAmmo < limAmmo)
                {
                    sndmngr.Play("ShotgunReload");
                    reloadSound = true;
                    rui.startReload = true;
                    rui.totalReload = 0.38f;
                    if (cams.Cam1Priority() == 0)
                    {
                        cams.ResetTimer();
                        cams.ShootingCam();
                    }
                }
            }
            else
            {
                cams.ResetTimer();
            }
            if (reloadTimer > 2.75)
            {
                if (sprAmmo != 0 && curAmmo < limAmmo)
                {
                    int temp = limAmmo - curAmmo;

                    if (temp > sprAmmo)
                    {
                        curAmmo += sprAmmo;
                        sprAmmo -= sprAmmo;
                    }
                    else
                    {
                        sprAmmo -= temp;
                        curAmmo += temp;
                        
                    }

                    reloading = false;
                    reloadSound = false;
                    reloadTimer = 0;
                    isReloading = false;
                    SaveValues();
                    StoreValues();
                    game_ui.UpdateAmmoText();
                }
                else if (curAmmo <= 0 && sprAmmo <= 0)
                {
                    //Debug.LogError(weaponType + " is out of ammo!\t<b><color=orange>Look for some ammo to pickup!</color></b>");
                    if (!game_ui.ammoNotif.activeSelf)
                    {
                        game_ui.EnableAmmoNotif("LowAmmo");
                    }
                }
            }
        }
        
    }

    public void EnemyReload()
    {
        if (!isPlayer && curAmmo <= 0)
        {
            MaxAmmo();
        }
    }

    public string CurrentAmmo()
    {
        return curAmmo.ToString();
    }

    public string SpareAmmo()
    {
        return sprAmmo.ToString();
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void Inputs()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse2) && Time.timeScale > 0 && isPlayer && !ds.DebugActive())
        //{
        //    MaxAmmo();
        //}

        if (!isReloading)
        {
            if (((Input.mouseScrollDelta.y > 0 && weapon1 == WeaponType.Handgun) || (Input.mouseScrollDelta.y < 0 && weapon2 == WeaponType.Handgun)) && Time.timeScale > 0 && isPlayer && !ds.DebugActive())
            {
                if (weaponType != WeaponType.Handgun)
                {
                    if (Time.timeScale < 1)
                        fireRate = fireRate / fireMod;
                    #region DEBUGGING
#if FIRERATE
                Debug.LogError("<b><color=orange>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                    #endregion

                    sndmngr.Play("WeaponSwap");
                    //pistolUI.SetActive(true);
                    //shottyUI.SetActive(false);
                    weaponType = WeaponType.Handgun;
                    DetermineWeapon();

                    if (Time.timeScale < 1)
                        fireRate = fireRate * fireMod;
                    #region DEBUGGING
#if FIRERATE
                Debug.LogError("<b><color=orange>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                    #endregion

                    SwitchWeapons();
                    SetHere();
                    game_ui.UpdateAmmoText();
                }
            }
            else if (((Input.mouseScrollDelta.y > 0 && weapon1 == WeaponType.Shotgun) || (Input.mouseScrollDelta.y < 0 && weapon2 == WeaponType.Shotgun)) && Time.timeScale > 0 && isPlayer && !ds.DebugActive())
            {
                if (weaponType != WeaponType.Shotgun)
                {
                    if (Time.timeScale < 1)
                        fireRate = fireRate / fireMod;
                    #region DEBUGGING
#if FIRERATE
                Debug.LogError("<b><color=orange>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                    #endregion

                    sndmngr.Play("WeaponSwap");
                    //pistolUI.SetActive(false);
                    //shottyUI.SetActive(true);
                    weaponType = WeaponType.Shotgun;
                    DetermineWeapon();

                    if (Time.timeScale < 1)
                        fireRate = fireRate * fireMod;
                    #region DEBUGGING
#if FIRERATE
                Debug.LogError("<b><color=orange>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                    #endregion

                    SwitchWeapons();
                    SetHere();
                    game_ui.UpdateAmmoText();
                }
            }
            else if (((Input.mouseScrollDelta.y > 0 && weapon1 == WeaponType.AR) || (Input.mouseScrollDelta.y < 0 && weapon2 == WeaponType.AR)) && Time.timeScale > 0 && isPlayer && !ds.DebugActive())
            {
                if (weaponType != WeaponType.AR)
                {
                    if (Time.timeScale < 1)
                        fireRate = fireRate / fireMod;
                    #region DEBUGGING
#if FIRERATE
                Debug.LogError("<b><color=orange>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                    #endregion

                    sndmngr.Play("WeaponSwap");
                    //pistolUI.SetActive(false);
                    //shottyUI.SetActive(true);
                    weaponType = WeaponType.AR;
                    DetermineWeapon();

                    if (Time.timeScale < 1)
                        fireRate = fireRate * fireMod;
                    #region DEBUGGING
#if FIRERATE
                Debug.LogError("<b><color=orange>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                    #endregion

                    SwitchWeapons();
                    SetHere();
                    game_ui.UpdateAmmoText();
                }
            }
            else if (((Input.mouseScrollDelta.y > 0 && weapon1 == WeaponType.NONE) || (Input.mouseScrollDelta.y < 0 && weapon2 == WeaponType.NONE)) && Time.timeScale > 0 && isPlayer && !ds.DebugActive())
            {
                if (weaponType != WeaponType.NONE)
                {
                    sndmngr.Play("WeaponSwap");
                    //pistolUI.SetActive(false);
                    //shottyUI.SetActive(true);
                    weaponType = WeaponType.NONE;
                    DetermineWeapon();
                    SwitchWeapons();
                    game_ui.UpdateAmmoText();
                }
            }

            if (isPlayer && Time.timeScale < 1 && !checkedTime)
            {
                fireRate = fireRate * fireMod;
                #region DEBUGGING
#if FIRERATE
            Debug.LogError("<b><color=cyan>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                #endregion

                checkedTime = true;
            }
            else if (isPlayer && Time.timeScale == 1 && checkedTime)
            {
                fireRate = fireRate / fireMod;
                #region DEBUGGING
#if FIRERATE
            Debug.LogError("<b><color=cyan>" + weaponType + "'s Fire Rate</color></b> = <b><color=lime>" + fireRate + "</color></b>");
#endif
                #endregion

                checkedTime = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && gunTimer >= fireRate && cams.ActiveNeutral() && Time.timeScale > 0 && !ds.DebugActive() && isPlayer)
            {
                cams.ResetTimer();
                cams.ShootingCam();
                Shoot();
                game_ui.UpdateAmmoText();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && gunTimer >= fireRate && Time.timeScale > 0 && !ds.DebugActive() && isPlayer)
            {
                cams.ResetTimer();
                Shoot();
                game_ui.UpdateAmmoText();
            }
            else if ((Input.GetKey(KeyCode.Mouse0) && weaponType == WeaponType.AR) && gunTimer >= fireRate && Time.timeScale > 0 && !ds.DebugActive() && isPlayer)
            {
                cams.ResetTimer();
                continuious = true;
                Shoot();
                game_ui.UpdateAmmoText();
            }

            if ((!Input.GetKey(KeyCode.Mouse0) && weaponType == WeaponType.AR) && Time.timeScale > 0 && !ds.DebugActive() && isPlayer)
                continuious = false;

            if (Input.GetKey(KeyCode.Mouse1) && Time.timeScale > 0 && !ds.DebugActive() && isPlayer)
                cams.ADSCam();
            else if (Input.GetKeyUp(KeyCode.Mouse1) && Time.timeScale > 0 && !ds.DebugActive() && isPlayer)
            {
                cams.ResetTimer();
                cams.ShootingCam();
            }

            if (Input.GetKeyDown(KeyCode.R) && Time.timeScale > 0 && isPlayer)
            {
                ReloadAmmo();
            }
        }
    }

    void RandomARSound()
    {
        float ran = Random.value;
        if (ran >= 0.5f)
            sndmngr.Play("AR_Shot1");
        else if (ran < 0.5f)
            sndmngr.Play("AR_Shot2");
    }

    void SaveWeapon()
    {
        if (isPlayer)
        {
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    data.currentWeapon = 0;
                    break;

                case WeaponType.Shotgun:
                    data.currentWeapon = 1;
                    break;

                case WeaponType.AR:
                    data.currentWeapon = 2;
                    break;

                default:
                    data.currentWeapon = 3;
                    break;
            }

            switch (weapon1)
            {
                case WeaponType.Handgun:
                    data.weapon1 = 0;
                    break;

                case WeaponType.Shotgun:
                    data.weapon1 = 1;
                    break;

                case WeaponType.AR:
                    data.weapon1 = 2;
                    break;

                default:
                    data.weapon1 = 3;
                    break;
            }

            switch (weapon2)
            {
                case WeaponType.Handgun:
                    data.weapon2 = 0;
                    break;

                case WeaponType.Shotgun:
                    data.weapon2 = 1;
                    break;

                case WeaponType.AR:
                    data.weapon2 = 2;
                    break;

                default:
                    data.weapon2 = 3;
                    break;
            }
        }

        if (isPlayer && cm.StartLevel())
        {
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    data.old_currentWeapon = 0;
                    break;

                case WeaponType.Shotgun:
                    data.old_currentWeapon = 1;
                    break;

                case WeaponType.AR:
                    data.old_currentWeapon = 2;
                    break;

                default:
                    data.old_currentWeapon = 3;
                    break;
            }

            switch (weapon1)
            {
                case WeaponType.Handgun:
                    data.old_weapon1 = 0;
                    break;

                case WeaponType.Shotgun:
                    data.old_weapon1 = 1;
                    break;

                case WeaponType.AR:
                    data.old_weapon1 = 2;
                    break;

                default:
                    data.old_weapon1 = 3;
                    break;
            }

            switch (weapon2)
            {
                case WeaponType.Handgun:
                    data.old_weapon2 = 0;
                    break;

                case WeaponType.Shotgun:
                    data.old_weapon2 = 1;
                    break;

                case WeaponType.AR:
                    data.old_weapon2 = 2;
                    break;

                default:
                    data.old_weapon2 = 3;
                    break;
            }
        }
    }

    void RecallOldValues()
    {
        if (weaponType == weapon1 && isPlayer && data.oldPVUsed)
        {
            weaponType = weapon2;
            DetermineWeapon();
            weaponType = weapon1;
            DetermineWeapon();
            #region DEBUGGING
#if OLD
            Debug.LogError("<b><color=yellow>#1 Weapon 1 Ammo Load</color></b>: <b><color=lightblue>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>\n<b><color=lime>Weapon 2 Ammo Load</color></b>: <b><color=cyan>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
            #endregion
        }
        else if (weaponType == weapon2 && isPlayer && data.oldPVUsed)
        {
            weaponType = weapon1;
            DetermineWeapon();
            weaponType = weapon2;
            DetermineWeapon();
            #region DEBUGGING
#if OLD
            Debug.LogError("<b><color=yellow>#2 Weapon 1 Ammo Load</color></b>: <b><color=lightblue>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>\n<b><color=lime>Weapon 2 Ammo Load</color></b>: <b><color=cyan>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
            #endregion
        }
    }

    void SaveOldValues()
    {
        if (weaponType == weapon1 && cm.StartLevel() && isPlayer && checkOld)
        {
            weaponType = weapon2;
            DetermineWeapon();
            weaponType = weapon1;
            DetermineWeapon();
            checkOld = false;
            #region DEBUGGING
#if OLD
            Debug.LogError("<b><color=yellow>#1 Weapon 1 Ammo Save</color></b>: <b><color=lightblue>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>\n<b><color=lime>Weapon 2 Ammo Save</color></b>: <b><color=cyan>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
            #endregion
        }
        else if (weaponType == weapon2 && cm.StartLevel() && isPlayer && checkOld)
        {
            weaponType = weapon1;
            DetermineWeapon();
            weaponType = weapon2;
            DetermineWeapon();
            checkOld = false;
            #region DEBUGGING
#if OLD
            Debug.LogError("<b><color=yellow>#2 Weapon 1 Ammo Save</color></b>: <b><color=lightblue>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>\n<b><color=lime>Weapon 2 Ammo Save</color></b>: <b><color=cyan>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
            #endregion
        }
    }

    void SaveValues()
    {
        if (isPlayer)
        {
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    data.w1_curAmmo = curAmmo;
                    data.w1_sprAmmo = sprAmmo;
                    break;

                case WeaponType.Shotgun:
                    data.w2_curAmmo = curAmmo;
                    data.w2_sprAmmo = sprAmmo;
                    break;

                case WeaponType.AR:
                    data.w3_curAmmo = curAmmo;
                    data.w3_sprAmmo = sprAmmo;
                    break;
            }
        }

        if (isPlayer && cm.StartLevel() && checkOld)
        {
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    data.old_w1_curAmmo = curAmmo;
                    data.old_w1_sprAmmo = sprAmmo;
                    #region DEBUGGING
#if OLD
                    Debug.LogError("<b><color=orange>Old Handgun</color></b> = <b><color=silver>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>");
#endif
                    #endregion
                    break;

                case WeaponType.Shotgun:
                    data.old_w2_curAmmo = curAmmo;
                    data.old_w2_sprAmmo = sprAmmo;
                    #region DEBUGGING
#if OLD
                    Debug.LogError("<b><color=orange>Old Shotgun</color></b> = <b><color=silver>[" + data.old_w2_curAmmo + "/" + data.old_w2_sprAmmo + "]</color></b>");
#endif
                    #endregion
                    break;

                case WeaponType.AR:
                    data.old_w3_curAmmo = curAmmo;
                    data.old_w3_sprAmmo = sprAmmo;
                    #region DEBUGGING
#if OLD
                    Debug.LogError("<b><color=orange>Old AR</color></b> = <b><color=silver>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
                    #endregion
                    break;
            }
            //switch (weapon1)
            //{
            //    case WeaponType.Handgun:
            //        data.old_w1_curAmmo = data.w1_curAmmo;
            //        data.old_w1_sprAmmo = data.w1_sprAmmo;
            //        break;

            //    case WeaponType.Shotgun:
            //        data.old_w2_curAmmo = data.w2_curAmmo;
            //        data.old_w2_sprAmmo = data.w2_sprAmmo;
            //        break;

            //    case WeaponType.AR:
            //        data.old_w3_curAmmo = data.w3_curAmmo;
            //        data.old_w3_sprAmmo = data.w3_sprAmmo;
            //        break;
            //}
            //switch (weapon2)
            //{
            //    case WeaponType.Handgun:
            //        data.old_w1_curAmmo = data.w1_curAmmo;
            //        data.old_w1_sprAmmo = data.w1_sprAmmo;
            //        break;

            //    case WeaponType.Shotgun:
            //        data.old_w2_curAmmo = data.w2_curAmmo;
            //        data.old_w2_sprAmmo = data.w2_sprAmmo;
            //        break;

            //    case WeaponType.AR:
            //        data.old_w3_curAmmo = data.w3_curAmmo;
            //        data.old_w3_sprAmmo = data.w3_sprAmmo;
            //        break;
            //}
        }
    }

    void StoreValues()
    {
        switch (weaponType)
        {
            case WeaponType.Handgun:
                handgunValues.damage = damage;
                handgunValues.fireRate = fireRate;
                handgunValues.weaponRange = weaponRange;
                handgunValues.hitForce = hitForce;
                handgunValues.curAmmo = curAmmo;
                handgunValues.sprAmmo = sprAmmo;
                handgunValues.maxAmmo = maxAmmo;
                handgunValues.limAmmo = limAmmo;
                handgunValues.headMod = headMod;
                handgunValues.bodyMod = bodyMod;
                break;

            case WeaponType.Shotgun:
                shotgunValues.damage = damage;
                shotgunValues.fireRate = fireRate;
                shotgunValues.weaponRange = weaponRange;
                shotgunValues.hitForce = hitForce;
                shotgunValues.curAmmo = curAmmo;
                shotgunValues.sprAmmo = sprAmmo;
                shotgunValues.maxAmmo = maxAmmo;
                shotgunValues.limAmmo = limAmmo;
                shotgunValues.headMod = headMod;
                shotgunValues.bodyMod = bodyMod;
                break;

            case WeaponType.AR:
                aRValues.damage = damage;
                aRValues.fireRate = fireRate;
                aRValues.weaponRange = weaponRange;
                aRValues.hitForce = hitForce;
                aRValues.curAmmo = curAmmo;
                aRValues.sprAmmo = sprAmmo;
                aRValues.maxAmmo = maxAmmo;
                aRValues.limAmmo = limAmmo;
                aRValues.headMod = headMod;
                aRValues.bodyMod = bodyMod;
                break;

            default:
                damage = 0;
                fireRate = 0f;
                weaponRange = 0f;
                hitForce = 0f;
                curAmmo = 0;
                sprAmmo = 0;
                maxAmmo = 0;
                limAmmo = 0;
                headMod = 1;
                bodyMod = 1;
                break;
        }
    }

    private void DetermineWeapon()
    {
        // Default
        if (Application.IsPlaying(gameObject) && ((!data.playerValuesUsed && !data.oldPVUsed) || !isPlayer))
        {
            #region DEBUGGING
#if AMMO
            if (isPlayer)
                Debug.LogError("<b><color=yellow>Hit Default</color></b>");
#endif
            #endregion
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    damage = handgunValues.damage;
                    fireRate = handgunValues.fireRate;
                    weaponRange = handgunValues.weaponRange;
                    hitForce = handgunValues.hitForce;
                    curAmmo = handgunValues.curAmmo;
                    sprAmmo = handgunValues.sprAmmo;
                    maxAmmo = handgunValues.maxAmmo;
                    limAmmo = handgunValues.limAmmo;
                    headMod = handgunValues.headMod;
                    bodyMod = handgunValues.bodyMod;
                    break;

                case WeaponType.Shotgun:
                    damage = shotgunValues.damage;
                    fireRate = shotgunValues.fireRate;
                    weaponRange = shotgunValues.weaponRange;
                    hitForce = shotgunValues.hitForce;
                    curAmmo = shotgunValues.curAmmo;
                    sprAmmo = shotgunValues.sprAmmo;
                    maxAmmo = shotgunValues.maxAmmo;
                    limAmmo = shotgunValues.limAmmo;
                    headMod = shotgunValues.headMod;
                    bodyMod = shotgunValues.bodyMod;
                    break;

                case WeaponType.AR:
                    damage = aRValues.damage;
                    fireRate = aRValues.fireRate;
                    weaponRange = aRValues.weaponRange;
                    hitForce = aRValues.hitForce;
                    curAmmo = aRValues.curAmmo;
                    sprAmmo = aRValues.sprAmmo;
                    maxAmmo = aRValues.maxAmmo;
                    limAmmo = aRValues.limAmmo;
                    headMod = aRValues.headMod;
                    bodyMod = aRValues.bodyMod;
                    break;

                default:
                    damage = 0;
                    fireRate = 0f;
                    weaponRange = 0f;
                    hitForce = 0f;
                    curAmmo = 0;
                    sprAmmo = 0;
                    maxAmmo = 0;
                    limAmmo = 0;
                    headMod = 1;
                    bodyMod = 1;
                    break;
            }
        }
        // During Editor
        else if (!Application.IsPlaying(gameObject))
        {
            #region DEBUGGING
#if AMMO
            if (isPlayer)
                Debug.LogError("<b><color=lime>Hit During Editor</color></b>");
#endif
            #endregion
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    damage = handgunValues.damage;
                    fireRate = handgunValues.fireRate;
                    weaponRange = handgunValues.weaponRange;
                    hitForce = handgunValues.hitForce;
                    curAmmo = handgunValues.curAmmo;
                    sprAmmo = handgunValues.sprAmmo;
                    maxAmmo = handgunValues.maxAmmo;
                    limAmmo = handgunValues.limAmmo;
                    headMod = handgunValues.headMod;
                    bodyMod = handgunValues.bodyMod;
                    break;

                case WeaponType.Shotgun:
                    damage = shotgunValues.damage;
                    fireRate = shotgunValues.fireRate;
                    weaponRange = shotgunValues.weaponRange;
                    hitForce = shotgunValues.hitForce;
                    curAmmo = shotgunValues.curAmmo;
                    sprAmmo = shotgunValues.sprAmmo;
                    maxAmmo = shotgunValues.maxAmmo;
                    limAmmo = shotgunValues.limAmmo;
                    headMod = shotgunValues.headMod;
                    bodyMod = shotgunValues.bodyMod;
                    break;

                case WeaponType.AR:
                    damage = aRValues.damage;
                    fireRate = aRValues.fireRate;
                    weaponRange = aRValues.weaponRange;
                    hitForce = aRValues.hitForce;
                    curAmmo = aRValues.curAmmo;
                    sprAmmo = aRValues.sprAmmo;
                    maxAmmo = aRValues.maxAmmo;
                    limAmmo = aRValues.limAmmo;
                    headMod = aRValues.headMod;
                    bodyMod = aRValues.bodyMod;
                    break;

                default:
                    damage = 0;
                    fireRate = 0f;
                    weaponRange = 0f;
                    hitForce = 0f;
                    curAmmo = 0;
                    sprAmmo = 0;
                    maxAmmo = 0;
                    limAmmo = 0;
                    headMod = 1;
                    bodyMod = 1;
                    break;
            }
        }
        // With OptionSaver and next level/last checkpoint/continue
        else if (data.playerValuesUsed && !data.oldPVUsed && isPlayer)
        {
            #region DEBUGGING
#if AMMO
            Debug.LogError("<b><color=cyan>Hit Player Values Used</color></b>\nData.w1_curAmmo = <b><color=magenta>" + data.w1_curAmmo + "</color></b>\nData.w1_sprAmmo = <b><color=lightblue>" + data.w1_sprAmmo + "</color></b>");
#endif
            #endregion
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    damage = handgunValues.damage;
                    fireRate = handgunValues.fireRate;
                    weaponRange = handgunValues.weaponRange;
                    hitForce = handgunValues.hitForce;
                    curAmmo = data.w1_curAmmo;
                    sprAmmo = data.w1_sprAmmo;
                    maxAmmo = handgunValues.maxAmmo;
                    limAmmo = handgunValues.limAmmo;
                    headMod = handgunValues.headMod;
                    bodyMod = handgunValues.bodyMod;
                    break;

                case WeaponType.Shotgun:
                    damage = shotgunValues.damage;
                    fireRate = shotgunValues.fireRate;
                    weaponRange = shotgunValues.weaponRange;
                    hitForce = shotgunValues.hitForce;
                    curAmmo = data.w2_curAmmo;
                    sprAmmo = data.w2_sprAmmo;
                    maxAmmo = shotgunValues.maxAmmo;
                    limAmmo = shotgunValues.limAmmo;
                    headMod = shotgunValues.headMod;
                    bodyMod = shotgunValues.bodyMod;
                    break;

                case WeaponType.AR:
                    damage = aRValues.damage;
                    fireRate = aRValues.fireRate;
                    weaponRange = aRValues.weaponRange;
                    hitForce = aRValues.hitForce;
                    curAmmo = data.w3_curAmmo;
                    sprAmmo = data.w3_sprAmmo;
                    maxAmmo = aRValues.maxAmmo;
                    limAmmo = aRValues.limAmmo;
                    headMod = aRValues.headMod;
                    bodyMod = aRValues.bodyMod;
                    break;

                default:
                    damage = 0;
                    fireRate = 0f;
                    weaponRange = 0f;
                    hitForce = 0f;
                    curAmmo = 0;
                    sprAmmo = 0;
                    maxAmmo = 0;
                    limAmmo = 0;
                    headMod = 1;
                    bodyMod = 1;
                    break;
            }
        }
        // With OptionSaver and restart level
        else if (!data.playerValuesUsed && data.oldPVUsed && isPlayer)
        {
            #region DEBUGGING
#if AMMO
            Debug.LogError("<b><color=orange>Hit Old PV Used</color></b>");
#endif
            #endregion
            switch (weaponType)
            {
                case WeaponType.Handgun:
                    damage = handgunValues.damage;
                    fireRate = handgunValues.fireRate;
                    weaponRange = handgunValues.weaponRange;
                    hitForce = handgunValues.hitForce;
                    curAmmo = data.old_w1_curAmmo;
                    sprAmmo = data.old_w1_sprAmmo;
                    maxAmmo = handgunValues.maxAmmo;
                    limAmmo = handgunValues.limAmmo;
                    headMod = handgunValues.headMod;
                    bodyMod = handgunValues.bodyMod;
                    break;

                case WeaponType.Shotgun:
                    damage = shotgunValues.damage;
                    fireRate = shotgunValues.fireRate;
                    weaponRange = shotgunValues.weaponRange;
                    hitForce = shotgunValues.hitForce;
                    curAmmo = data.old_w2_curAmmo;
                    sprAmmo = data.old_w2_sprAmmo;
                    maxAmmo = shotgunValues.maxAmmo;
                    limAmmo = shotgunValues.limAmmo;
                    headMod = shotgunValues.headMod;
                    bodyMod = shotgunValues.bodyMod;
                    break;

                case WeaponType.AR:
                    damage = aRValues.damage;
                    fireRate = aRValues.fireRate;
                    weaponRange = aRValues.weaponRange;
                    hitForce = aRValues.hitForce;
                    curAmmo = data.old_w3_curAmmo;
                    sprAmmo = data.old_w3_sprAmmo;
                    maxAmmo = aRValues.maxAmmo;
                    limAmmo = aRValues.limAmmo;
                    headMod = aRValues.headMod;
                    bodyMod = aRValues.bodyMod;
                    break;

                default:
                    damage = 0;
                    fireRate = 0f;
                    weaponRange = 0f;
                    hitForce = 0f;
                    curAmmo = 0;
                    sprAmmo = 0;
                    maxAmmo = 0;
                    limAmmo = 0;
                    headMod = 1;
                    bodyMod = 1;
                    break;
            }
        }
        else
        {
            throw new System.Exception("PlayerValuesUsed and OldPVUsed are both set to true.");
        }


        if (Application.IsPlaying(gameObject) && isPlayer)
        {
            SaveWeapon();
            #region DEBUGGING
#if AMMO
            Debug.LogError("<b><color=brown>Before SaveValues</color></b>\nData.w1_curAmmo = <b><color=magenta>" + data.w1_curAmmo + "</color></b>\nData.w1_sprAmmo = <b><color=lightblue>" + data.w1_sprAmmo + "</color></b>");
#endif
            #endregion
            SaveValues();
            #region DEBUGGING
#if AMMO
            Debug.LogError("<b><color=maroon>After SaveValues</color></b>\nData.w1_curAmmo = <b><color=magenta>" + data.w1_curAmmo + "</color></b>\nData.w1_sprAmmo = <b><color=lightblue>" + data.w1_sprAmmo + "</color></b>");
#endif
            #endregion
            StoreValues();
        }
    }

    void StartingWeapon()
    {
        if ((!data.playerValuesUsed && !data.oldPVUsed) || !isPlayer)
        {
            if (weapon1 == WeaponType.Handgun)
                weaponType = WeaponType.Handgun;
            else if (weapon1 == WeaponType.Shotgun)
                weaponType = WeaponType.Shotgun;
            else if (weapon1 == WeaponType.AR)
                weaponType = WeaponType.AR;
            else
                weaponType = WeaponType.NONE;
        }
        else if (data.playerValuesUsed && !data.oldPVUsed && isPlayer)
        {
            switch (data.weapon1)
            {
                case 0:
                    weapon1 = WeaponType.Handgun;
                    break;
                case 1:
                    weapon1 = WeaponType.Shotgun;
                    break;
                case 2:
                    weapon1 = WeaponType.AR;
                    break;
                default:
                    weapon1 = WeaponType.NONE;
                    break;
            }

            switch (data.weapon2)
            {
                case 0:
                    weapon2 = WeaponType.Handgun;
                    break;
                case 1:
                    weapon2 = WeaponType.Shotgun;
                    break;
                case 2:
                    weapon2 = WeaponType.AR;
                    break;
                default:
                    weapon2 = WeaponType.NONE;
                    break;
            }

            switch (data.currentWeapon)
            {
                case 0:
                    weaponType = WeaponType.Handgun;
                    break;
                case 1:
                    weaponType = WeaponType.Shotgun;
                    break;
                case 2:
                    weaponType = WeaponType.AR;
                    break;
                default:
                    weaponType = WeaponType.NONE;
                    break;
            }
        }
        else if (!data.playerValuesUsed && data.oldPVUsed && isPlayer)
        {
            switch (data.old_weapon1)
            {
                case 0:
                    weapon1 = WeaponType.Handgun;
                    break;
                case 1:
                    weapon1 = WeaponType.Shotgun;
                    break;
                case 2:
                    weapon1 = WeaponType.AR;
                    break;
                default:
                    weapon1 = WeaponType.NONE;
                    break;
            }

            switch (data.old_weapon2)
            {
                case 0:
                    weapon2 = WeaponType.Handgun;
                    break;
                case 1:
                    weapon2 = WeaponType.Shotgun;
                    break;
                case 2:
                    weapon2 = WeaponType.AR;
                    break;
                default:
                    weapon2 = WeaponType.NONE;
                    break;
            }

            switch (data.old_currentWeapon)
            {
                case 0:
                    weaponType = WeaponType.Handgun;
                    break;
                case 1:
                    weaponType = WeaponType.Shotgun;
                    break;
                case 2:
                    weaponType = WeaponType.AR;
                    break;
                default:
                    weaponType = WeaponType.NONE;
                    break;
            }
        }
        else
        {
            throw new System.Exception("PlayerValuesUsed and OldPVUsed are both set to true.");
        }
    }

    void SwitchWeapons()
    {
        switch (weaponType)
        {
            case WeaponType.Handgun:
                foreach (GameObject handgun in handguns)
                    if (isPlayer && !handgun.activeInHierarchy)
                    {
                        handgun.SetActive(true);
                        pistolUI.SetActive(true);
                        pistol2UI.SetActive(false);
                        shottyUI.SetActive(false);
                        shotty2UI.SetActive(false);
                        if (weapon1 == WeaponType.Shotgun || weapon2 == WeaponType.Shotgun)
                            shotty2UI.SetActive(true);
                        aR_UI.SetActive(false);
                        aR2_UI.SetActive(false);
                        if (weapon1 == WeaponType.AR || weapon2 == WeaponType.AR)
                            aR2_UI.SetActive(true);
                    }
                    else
                        continue;
                foreach (GameObject shotgun in shotguns)
                    if (isPlayer && shotgun.activeInHierarchy)
                        shotgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject AR in aRs)
                    if (isPlayer && AR.activeInHierarchy)
                        AR.SetActive(false);
                    else
                        continue;
                break;

            case WeaponType.Shotgun:
                foreach (GameObject shotgun in shotguns)
                    if (isPlayer && !shotgun.activeInHierarchy)
                    {
                        shotgun.SetActive(true);
                        pistolUI.SetActive(false);
                        pistol2UI.SetActive(false);
                        if (weapon1 == WeaponType.Handgun || weapon2 == WeaponType.Handgun)
                            pistol2UI.SetActive(true);
                        shottyUI.SetActive(true);
                        shotty2UI.SetActive(false);
                        aR_UI.SetActive(false);
                        aR2_UI.SetActive(false);
                        if (weapon1 == WeaponType.AR || weapon2 == WeaponType.AR)
                            aR2_UI.SetActive(true);
                    }
                    else
                        continue;
                foreach (GameObject handgun in handguns)
                    if (isPlayer && handgun.activeInHierarchy)
                        handgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject AR in aRs)
                    if (isPlayer && AR.activeInHierarchy)
                        AR.SetActive(false);
                    else
                        continue;
                break;

            case WeaponType.AR:
                foreach (GameObject AR in aRs)
                    if (isPlayer && !AR.activeInHierarchy)
                    {
                        AR.SetActive(true);
                        pistolUI.SetActive(false);
                        pistol2UI.SetActive(false);
                        if (weapon1 == WeaponType.Handgun || weapon2 == WeaponType.Handgun)
                            pistol2UI.SetActive(true);
                        shottyUI.SetActive(false);
                        shotty2UI.SetActive(false);
                        if (weapon1 == WeaponType.Shotgun || weapon2 == WeaponType.Shotgun)
                            shotty2UI.SetActive(true);
                        aR_UI.SetActive(true);
                        aR2_UI.SetActive(false);
                    }
                    else
                        continue;
                foreach (GameObject handgun in handguns)
                    if (isPlayer && handgun.activeInHierarchy)
                        handgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject shotgun in shotguns)
                    if (isPlayer && shotgun.activeInHierarchy)
                        shotgun.SetActive(false);
                    else
                        continue;
                break;

            default:
                foreach (GameObject handgun in handguns)
                    if (isPlayer && handgun.activeInHierarchy)
                    {
                        handgun.SetActive(false);
                        pistolUI.SetActive(false);
                        pistol2UI.SetActive(false);
                        if (weapon1 == WeaponType.Handgun || weapon2 == WeaponType.Handgun)
                            pistol2UI.SetActive(true);
                        shottyUI.SetActive(false);
                        shotty2UI.SetActive(false);
                        if (weapon1 == WeaponType.Shotgun || weapon2 == WeaponType.Shotgun)
                            shotty2UI.SetActive(true);
                        aR_UI.SetActive(false);
                        aR2_UI.SetActive(false);
                        if (weapon1 == WeaponType.AR || weapon2 == WeaponType.AR)
                            aR2_UI.SetActive(true);
                    }
                    else
                        continue;
                foreach (GameObject shotgun in shotguns)
                    if (isPlayer && shotgun.activeInHierarchy)
                    {
                        shotgun.SetActive(false);
                        pistolUI.SetActive(false);
                        pistol2UI.SetActive(false);
                        if (weapon1 == WeaponType.Handgun || weapon2 == WeaponType.Handgun)
                            pistol2UI.SetActive(true);
                        shottyUI.SetActive(false);
                        shotty2UI.SetActive(false);
                        if (weapon1 == WeaponType.Shotgun || weapon2 == WeaponType.Shotgun)
                            shotty2UI.SetActive(true);
                        aR_UI.SetActive(false);
                        aR2_UI.SetActive(false);
                        if (weapon1 == WeaponType.AR || weapon2 == WeaponType.AR)
                            aR2_UI.SetActive(true);
                    }
                    else
                        continue;
                foreach (GameObject AR in aRs)
                    if (isPlayer && AR.activeInHierarchy)
                    {
                        AR.SetActive(false);
                        pistolUI.SetActive(false);
                        pistol2UI.SetActive(false);
                        if (weapon1 == WeaponType.Handgun || weapon2 == WeaponType.Handgun)
                            pistol2UI.SetActive(true);
                        shottyUI.SetActive(false);
                        shotty2UI.SetActive(false);
                        if (weapon1 == WeaponType.Shotgun || weapon2 == WeaponType.Shotgun)
                            shotty2UI.SetActive(true);
                        aR_UI.SetActive(false);
                        aR2_UI.SetActive(false);
                        if (weapon1 == WeaponType.AR || weapon2 == WeaponType.AR)
                            aR2_UI.SetActive(true);
                    }
                    else
                        continue;
                break;
        }
    }

    void SwitchWeaponsEnemies()
    {
        switch (weaponType)
        {
            case WeaponType.Handgun:
                foreach (GameObject handgun in handguns)
                    if (!isPlayer && !handgun.activeInHierarchy)
                        handgun.SetActive(true);
                    else
                        continue;
                foreach (GameObject shotgun in shotguns)
                    if (!isPlayer && shotgun.activeInHierarchy)
                        shotgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject AR in aRs)
                    if (!isPlayer && AR.activeInHierarchy)
                        AR.SetActive(false);
                    else
                        continue;
                break;

            case WeaponType.Shotgun:
                foreach (GameObject shotgun in shotguns)
                    if (!isPlayer && !shotgun.activeInHierarchy)
                        shotgun.SetActive(true);
                    else
                        continue;
                foreach (GameObject handgun in handguns)
                    if (!isPlayer && handgun.activeInHierarchy)
                        handgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject AR in aRs)
                    if (!isPlayer && AR.activeInHierarchy)
                        AR.SetActive(false);
                    else
                        continue;
                break;

            case WeaponType.AR:
                foreach (GameObject AR in aRs)
                    if (!isPlayer && !AR.activeInHierarchy)
                        AR.SetActive(true);
                    else
                        continue;
                foreach (GameObject handgun in handguns)
                    if (!isPlayer && handgun.activeInHierarchy)
                        handgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject shotgun in shotguns)
                    if (!isPlayer && shotgun.activeInHierarchy)
                        shotgun.SetActive(false);
                    else
                        continue;
                break;

            default:
                foreach (GameObject handgun in handguns)
                    if (!isPlayer && handgun.activeInHierarchy)
                        handgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject shotgun in shotguns)
                    if (!isPlayer && shotgun.activeInHierarchy)
                        shotgun.SetActive(false);
                    else
                        continue;
                foreach (GameObject AR in aRs)
                    if (!isPlayer && AR.activeInHierarchy)
                        AR.SetActive(false);
                    else
                        continue;
                break;
        }
    }

    private void Damage(RaycastHit hit)
    {
        // Enemy
        if (hit.collider.CompareTag("Head") && hit.collider.GetComponent<JButler_EnemyTarget>())
        {
            if (!hit.collider.GetComponent<JButler_EnemyTarget>().e.IsDead())
                pv.IncreaseBt(10f);
            hit.collider.GetComponent<JButler_EnemyTarget>().ToggleMat();
            hit.collider.GetComponent<JButler_EnemyTarget>().e.TakeDamage(damage * headMod);
            hit.rigidbody.AddForce(-hit.normal * hitForce);
        }
        else if (hit.collider.CompareTag("Body") && hit.collider.GetComponent<JButler_EnemyTarget>())
        {
            if (!hit.collider.GetComponent<JButler_EnemyTarget>().e.IsDead())
                pv.IncreaseBt(10f);
            hit.collider.GetComponent<JButler_EnemyTarget>().ToggleMat();
            hit.collider.GetComponent<JButler_EnemyTarget>().e.TakeDamage(damage * bodyMod);
            hit.rigidbody.AddForce(-hit.normal * hitForce);
        }
        else if (hit.collider.CompareTag("Arm") && hit.collider.GetComponent<JButler_EnemyTarget>())
        {
            if (!hit.collider.GetComponent<JButler_EnemyTarget>().e.IsDead())
                pv.IncreaseBt(10f);
            hit.collider.GetComponent<JButler_EnemyTarget>().ToggleMat();
            hit.collider.GetComponent<JButler_EnemyTarget>().e.TakeDamage(damage * bodyMod);
            hit.rigidbody.AddForce(-hit.normal * hitForce);
        }

        // Player
        if (hit.collider.CompareTag("Head") && hit.collider.GetComponent<JButler_BodyPart>())
        {
            hit.collider.GetComponent<JButler_BodyPart>().ToggleMat();
            hit.collider.GetComponent<JButler_BodyPart>().p.TakeDamage(damage * headMod);
            //hit.rigidbody.AddForce(-hit.normal * hitForce);
            pv.IncreaseBt(10f);
            if (!DI_System.CheckIfObjectInSight(this.transform))
            {
                DI_System.CreateIndicator(this.transform);
                sndmngr.Play("DamageIndicator");
            }
        }
        else if (hit.collider.CompareTag("Body") && hit.collider.GetComponent<JButler_BodyPart>())
        {
            hit.collider.GetComponent<JButler_BodyPart>().ToggleMat();
            hit.collider.GetComponent<JButler_BodyPart>().p.TakeDamage(damage * bodyMod);
            //hit.rigidbody.AddForce(-hit.normal * hitForce);
            pv.IncreaseBt(10f);
            if (!DI_System.CheckIfObjectInSight(this.transform))
            {
                DI_System.CreateIndicator(this.transform);
                sndmngr.Play("DamageIndicator");
            }
        }
        else if (hit.collider.CompareTag("Arm") && hit.collider.GetComponent<JButler_BodyPart>())
        {
            hit.collider.GetComponent<JButler_BodyPart>().ToggleMat();
            hit.collider.GetComponent<JButler_BodyPart>().p.TakeDamage(damage * bodyMod);
            //hit.rigidbody.AddForce(-hit.normal * hitForce);
            pv.IncreaseBt(10f);
            if (!DI_System.CheckIfObjectInSight(this.transform))
            {
                DI_System.CreateIndicator(this.transform);
                sndmngr.Play("DamageIndicator");
            }
        }

        // Shield
        if (hit.collider.CompareTag("Shield") && hit.collider.GetComponent<JButler_Shield>())
        {
            sndmngr.Play("ShieldHit");
        }

        // PowerSource
        if (hit.collider.CompareTag("Power Source") && hit.collider.GetComponent<JButler_PowerSource>())
        {
            JButler_PowerSource ps = hit.collider.GetComponent<JButler_PowerSource>();
            ps.Deactivate();
        }

        // Targets
        if (hit.collider.CompareTag("1 Point") && hit.collider.GetComponent<JButler_Target>() && hit.collider.GetComponent<JButler_Target>().NeedsBD() && mtp.bdActive)
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        else if (hit.collider.CompareTag("1 Point") && hit.collider.GetComponent<JButler_Target>() && !hit.collider.GetComponent<JButler_Target>().NeedsBD())
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        if (hit.collider.CompareTag("2 Point") && hit.collider.GetComponent<JButler_Target>() && hit.collider.GetComponent<JButler_Target>().NeedsBD() && mtp.bdActive)
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        else if (hit.collider.CompareTag("2 Point") && hit.collider.GetComponent<JButler_Target>() && !hit.collider.GetComponent<JButler_Target>().NeedsBD())
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        if (hit.collider.CompareTag("3 Point") && hit.collider.GetComponent<JButler_Target>() && hit.collider.GetComponent<JButler_Target>().NeedsBD() && mtp.bdActive)
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        else if (hit.collider.CompareTag("3 Point") && hit.collider.GetComponent<JButler_Target>() && !hit.collider.GetComponent<JButler_Target>().NeedsBD())
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        if (hit.collider.CompareTag("4 Point") && hit.collider.GetComponent<JButler_Target>() && hit.collider.GetComponent<JButler_Target>().NeedsBD() && mtp.bdActive)
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        else if (hit.collider.CompareTag("4 Point") && hit.collider.GetComponent<JButler_Target>() && !hit.collider.GetComponent<JButler_Target>().NeedsBD())
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        if (hit.collider.CompareTag("5 Point") && hit.collider.GetComponent<JButler_Target>() && hit.collider.GetComponent<JButler_Target>().NeedsBD() && mtp.bdActive)
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        else if (hit.collider.CompareTag("5 Point") && hit.collider.GetComponent<JButler_Target>() && !hit.collider.GetComponent<JButler_Target>().NeedsBD())
        {
            JButler_Target target = hit.collider.GetComponent<JButler_Target>();
            target.WhosHit();
            target.scoreboard.AddScore();
            target.Break();
        }
        if (hit.collider.GetComponent<TargetRef>() && hit.collider.CompareTag("Target"))
        {
            hit.collider.gameObject.GetComponentInParent<TargetBehavior>().Hitter();
        }
        else if (hit.collider.CompareTag("Target"))
        {
            sndmngr.Play("TargetHit");
            hit.collider.gameObject.SetActive(false);
            tar.PZL1();
        }
        if (hit.collider.CompareTag("Target2"))
        {
            sndmngr.Play("TargetHit");
            hit.collider.gameObject.SetActive(false);
            tar.PZL2();
        }
        if (hit.collider.CompareTag("Target3"))
        {
            sndmngr.Play("TargetHit");
            hit.collider.gameObject.SetActive(false);
            tar.PZL3();
        }

        // Barrel
        if (hit.collider.GetComponent<ExplosiveBarrel>())
        {
            hit.collider.GetComponent<ExplosiveBarrel>().TakeDamage(damage);
        }

        //Destructable Object
        if (hit.collider.GetComponent<Destructables>())
        {
            hit.collider.GetComponent<Destructables>().TakeBulletDamage(damage);
        }
    }

    void TraceLocation(Vector3 originPos)
    {
        try
        {
            #region DEBUGGING
#if TEST2
            Debug.Log("<b><color=yellow>Trace Pos before</color></b>: <b><color=cyan>" + bulletTrace.transform.position + "</color></b>\n<b><color=yellow>Trace Rot before</color></b>: <b><color=cyan>" + bulletTrace.transform.rotation + "</color></b>");
            Debug.DrawLine(bulletTrace.transform.position - Vector3.forward * 0.01f, bulletTrace.transform.position + Vector3.forward * 0.01f, Color.yellow, 50.0f);
            Debug.DrawLine(bulletTrace.transform.position - Vector3.right * 0.01f, bulletTrace.transform.position + Vector3.right * 0.01f, Color.yellow, 50.0f);
            Debug.DrawLine(bulletTrace.transform.position - Vector3.up * 0.01f, bulletTrace.transform.position + Vector3.up * 0.01f, Color.yellow, 50.0f);

            Debug.Log("<b><color=red>Here Pos</color></b>: <b><color=orange>" + originPos + "</color></b>");
            Debug.DrawLine(originPos - Vector3.forward * 0.1f, originPos + Vector3.forward * 0.1f, Color.red, 50.0f);
            Debug.DrawLine(originPos - Vector3.right * 0.1f, originPos + Vector3.right * 0.1f, Color.red, 50.0f);
            Debug.DrawLine(originPos - Vector3.up * 0.1f, originPos + Vector3.up * 0.1f, Color.red, 50.0f);
#endif
            #endregion
            bulletTrace.transform.position = originPos;
            bulletTrace.transform.LookAt(aim.target);
            #region DEBUGGING
#if TEST2
            Debug.Log("<b><color=lime>Trace Pos before</color></b>: <b><color=cyan>" + bulletTrace.transform.position + "</color></b>\n<b><color=lime>Trace Rot before</color></b>: <b><color=cyan>" + bulletTrace.transform.rotation + "</color></b>");
            Debug.DrawLine(bulletTrace.transform.position - Vector3.forward * 0.01f, bulletTrace.transform.position + Vector3.forward * 0.01f, Color.green, 50.0f);
            Debug.DrawLine(bulletTrace.transform.position - Vector3.right * 0.01f, bulletTrace.transform.position + Vector3.right * 0.01f, Color.green, 50.0f);
            Debug.DrawLine(bulletTrace.transform.position - Vector3.up * 0.01f, bulletTrace.transform.position + Vector3.up * 0.01f, Color.green, 50.0f);
#endif
            #endregion
        }
        catch (UnassignedReferenceException e)
        {
            throw new System.Exception(name + "'s field <b><color=red>Bullet Trace</color></b> is null!\t\tDid you forget to add the partical BulletTrace to Bullet Trace?\n" + e.Message);
        }
    }

    private void SetHere()
    {
        ArrayCheck();
        foreach (JButler_ShootFrom point in locations)
        {
            if (point.gameObject.activeInHierarchy)
                here = point;
            else
                continue;
        }
    }

    private void ArrayCheck()
    {
        for (int i = 0; i < locations.Length; i++)
            if (locations[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Location Element " + i + " returned null</color></b>!\tMake sure to either add JButler_ShootFrom or change the array size.");

        int active = locations.Length;

        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                here = null;
                throw new System.Exception("All <b><color=red>Locations</color></b> in " + name + " are inactive!\tMake sure that there is one location active at all times!");
            }
        }
    }

    void GunCheck()
    {
        for (int i = 0; i < handguns.Length; i++)
            if (handguns[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Handguns Element " + i + " returned null</color></b>!\tMake sure to add all Guns from the Actor or change the array size.");

        for (int i = 0; i < shotguns.Length; i++)
            if (shotguns[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Shotguns Element " + i + " returned null</color></b>!\tMake sure to add all Shotguns from the Actor or change the array size.");

        for (int i = 0; i < aRs.Length; i++)
            if (aRs[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>ARs Element " + i + " returned null</color></b>!\tMake sure to add all ARs from the Actor or change the array size.");

        int active = handguns.Length;

        for (int i = 0; i < handguns.Length; i++)
        {
            if (handguns[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                throw new System.Exception("All <b><color=red>Handguns</color></b> in " + name + " are inactive!\tMake sure that there is one location active at all times!");
            }
        }

        int active2 = shotguns.Length;

        for (int i = 0; i < shotguns.Length; i++)
        {
            if (shotguns[i].gameObject.activeInHierarchy)
                active2++;
            else
                active2--;
            if (active2 == 0)
            {
                throw new System.Exception("All <b><color=red>Shotguns</color></b> in " + name + " are inactive!\tMake sure that there is one location active at all times!");
            }
        }

        int active3 = aRs.Length;

        for (int i = 0; i < aRs.Length; i++)
        {
            if (aRs[i].gameObject.activeInHierarchy)
                active3++;
            else
                active3--;
            if (active3 == 0)
            {
                throw new System.Exception("All <b><color=red>ARs</color></b> in " + name + " are inactive!\tMake sure that there is one location active at all times!");
            }
        }
    }

    private void CheckFields()
    {
        if (bulletHole == null)
            throw new System.Exception(name + "'s field <b><color=red>BulletHole</color></b> is null!\t\tDid you forget to add a GameObject to Bullet Hole?");

        if (bulletTrace == null)
            throw new System.Exception(name + "'s field <b><color=red>Bullet Trace</color></b> is null!\t\tDid you forget to add the partical BulletTrace to Bullet Trace?");

        if (handgunValues == null)
            throw new System.Exception(name + "'s field <b><color=red>Handgun Values</color></b> is null!\t\tDid you forget to add the correct data to this object?");

        if (shotgunValues == null)
            throw new System.Exception(name + "'s field <b><color=red>Shotgun Values</color></b> is null!\t\tDid you forget to add the correct data to this object?");

        if (aRValues == null)
            throw new System.Exception(name + "'s field <b><color=red>AR Values</color></b> is null!\t\tDid you forget to add the correct data to this object?");

        if (here == null && locations.Length == 0)
            throw new System.Exception(name + "'s Locations <b><color=red>array size is set to 0</color></b>!\tMake sure add elements to the array.");

        if (isPlayer && handguns.Length == 0)
            throw new System.Exception(name + "'s Handguns <b><color=red>array size is set to 0</color></b>!\tMake sure add elements to the array.");

        if (isPlayer && shotguns.Length == 0)
            throw new System.Exception(name + "'s Shotguns <b><color=red>array size is set to 0</color></b>!\tMake sure add elements to the array.");

        if (handguns == null && handguns.Length == 0)
            throw new System.Exception(name + "'s Locations <b><color=red>array size is set to 0</color></b>!\tMake sure add elements to the array.");

        if (ds == null)
            throw new System.Exception(name + " <b><color=red>could not find DebugScript</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (mtp == null)
            throw new System.Exception(name + " <b><color=red>could not find Mtp</color></b>!\tMake sure that there is an active MovementThirdPerson script in the scene at all times!");

        if (cm == null)
            throw new System.Exception(name + " <b><color=red>could not find Cm</color></b>!\tMake sure that there is an active CheckpointManager script in the scene at all times!");

        if (data == null)
            throw new System.Exception(name + " <b><color=red>could not find OptionsSaverScript</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (aim == null)
            throw new System.Exception(name + "  could not find an object that uses <b><color=red>JButler_Aim</color></b>!\tMake sure that there is an active one in your scene as it is required for aiming and shooting!");

        if (cams == null)
            throw new System.Exception(name + " <b><color=red>could not find JButler_ChangeCamera</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (sndmngr == null)
            throw new System.Exception(name + " <b><color=red>could not find Sound_Manager</color></b>!\tMake sure that there is an active one in the scene at all times!");
    }

    void ImageCheck()
    {
        if (isPlayer && pistolUI == null)
            throw new System.Exception(name + "'s field <b><color=red>Pistol UI</color></b> is null!\t\tDid you forget to add the image PistolUI to this field?");

        if (isPlayer && pistol2UI == null)
            throw new System.Exception(name + "'s field <b><color=red>Pistol 2UI</color></b> is null!\t\tDid you forget to add the image PistolUI (S) to this field?");

        if (isPlayer && shottyUI == null)
            throw new System.Exception(name + "'s field <b><color=red>Shotty UI</color></b> is null!\t\tDid you forget to add the image ShotgunUI to this field?");

        if (isPlayer && shotty2UI == null)
            throw new System.Exception(name + "'s field <b><color=red>Shotty 2UI</color></b> is null!\t\tDid you forget to add the image ShotgunUI (S) to this field?");

        if (isPlayer && aR_UI == null)
            throw new System.Exception(name + "'s field <b><color=red>AR_UI</color></b> is null!\t\tDid you forget to add the image AR_UI to this field?");

        if (isPlayer && aR2_UI == null)
            throw new System.Exception(name + "'s field <b><color=red>AR2_UI</color></b> is null!\t\tDid you forget to add the image AR_UI (S) to this field?");
    }

    private void StartingWeaponIcon()
    {
        if (isPlayer && !data.playerValuesUsed && !data.oldPVUsed)
        {
            //pistolUI = GameObject.Find("PistolUI");
            //shottyUI = GameObject.Find("ShotgunUI");
            if (weapon1 == WeaponType.Handgun)
            {
                pistolUI.SetActive(true);
                pistol2UI.SetActive(false);
                shottyUI.SetActive(false);
                shotty2UI.SetActive(false);
                if (weapon2 == WeaponType.Shotgun)
                    shotty2UI.SetActive(true);
                aR_UI.SetActive(false);
                aR2_UI.SetActive(false);
                if (weapon2 == WeaponType.AR)
                    shotty2UI.SetActive(true);
            }
            else if (weapon1 == WeaponType.Shotgun)
            {
                pistolUI.SetActive(false);
                pistol2UI.SetActive(false);
                if (weapon2 == WeaponType.Handgun)
                    pistol2UI.SetActive(true);
                shottyUI.SetActive(true);
                shotty2UI.SetActive(false);
                aR_UI.SetActive(false);
                aR2_UI.SetActive(false);
                if (weapon2 == WeaponType.AR)
                    shotty2UI.SetActive(true);
            }
            else if (weapon1 == WeaponType.AR)
            {
                pistolUI.SetActive(false);
                pistol2UI.SetActive(false);
                if (weapon2 == WeaponType.Handgun)
                    pistol2UI.SetActive(true);
                shottyUI.SetActive(false);
                shotty2UI.SetActive(false);
                if (weapon2 == WeaponType.Shotgun)
                    shotty2UI.SetActive(true);
                aR_UI.SetActive(true);
                aR2_UI.SetActive(false);
            }
            else
            {
                pistolUI.SetActive(false);
                shottyUI.SetActive(false);
                aR_UI.SetActive(false);
                pistol2UI.SetActive(false);
                shotty2UI.SetActive(false);
                aR2_UI.SetActive(false);
            }
        }
        else if (isPlayer)
        {
            if (weaponType == WeaponType.Handgun)
            {
                pistolUI.SetActive(true);
                pistol2UI.SetActive(false);
                shottyUI.SetActive(false);
                shotty2UI.SetActive(false);
                if (weapon2 == WeaponType.Shotgun)
                    shotty2UI.SetActive(true);
                aR_UI.SetActive(false);
                aR2_UI.SetActive(false);
                if (weapon2 == WeaponType.AR)
                    shotty2UI.SetActive(true);
            }
            else if (weaponType == WeaponType.Shotgun)
            {
                pistolUI.SetActive(false);
                pistol2UI.SetActive(false);
                if (weapon2 == WeaponType.Handgun)
                    pistol2UI.SetActive(true);
                shottyUI.SetActive(true);
                shotty2UI.SetActive(false);
                aR_UI.SetActive(false);
                aR2_UI.SetActive(false);
                if (weapon2 == WeaponType.AR)
                    shotty2UI.SetActive(true);
            }
            else if (weaponType == WeaponType.AR)
            {
                pistolUI.SetActive(false);
                pistol2UI.SetActive(false);
                if (weapon2 == WeaponType.Handgun)
                    pistol2UI.SetActive(true);
                shottyUI.SetActive(false);
                shotty2UI.SetActive(false);
                if (weapon2 == WeaponType.Shotgun)
                    shotty2UI.SetActive(true);
                aR_UI.SetActive(true);
                aR2_UI.SetActive(false);
            }
            else
            {
                pistolUI.SetActive(false);
                shottyUI.SetActive(false);
                aR_UI.SetActive(false);
                pistol2UI.SetActive(false);
                shotty2UI.SetActive(false);
                aR2_UI.SetActive(false);
            }
        }
    }
}
