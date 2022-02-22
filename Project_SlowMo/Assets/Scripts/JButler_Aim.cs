#define TEST
//#define TEST2

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Project SlowMo Scripts/JButler_Aim")]
public class JButler_Aim : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Space(5)]
    [Header("User")]
    [Tooltip("Who is using this script, an enemy or the player?\nPlayer = true\nEnemy = false")]
    [SerializeField] bool isPlayer = false;

    [Space(5)]
    [Header("Location Points")]
    [Tooltip("These points are who is looking.\nThis can be left empty if isPlayer = false.")]
    [SerializeField] private Transform[] pivots = null;

    [Space(10)]
    [Tooltip("This point is where they are looking too.\nFor the character's aim, DO NOT CHANGE!\nTHIS IS VERY IMPORTANT!!!")]
    public Transform target = null;
    [Tooltip("This the miss zone area.\nOnly used by enemies.")]
    public JButler_MissChance targetArea = null;
    [Tooltip("This is the MissChance (really spread) for the shotgun.\nPut the Shotgun Spread here.")]
    [SerializeField] JButler_MissChance[] spreadArea = null;
    [Tooltip("These are the 5 points to create a shotgun spread.")]
    public Transform[] spread = null;
    [Tooltip("Set how far away from the point of origin the MissChance will sit.")]
    [SerializeField] float ShotgunSpreadDistance = 0.0f;
    [Tooltip("Set how far away from the point of origin the MissChance will sit.")]
    [SerializeField] float aRSpreadDistance = 0.0f;


    [Space(5)]
    [Header("Raycast Layers")]
    [Tooltip("Choose which layers the raycast can hit.")]
    [SerializeField] LayerMask layers;

    [Space(5)]
    [Header("Smooth Turning")]
    [Tooltip("As the name implies, this is to help how smoothly the origin point looks at the target point.")]
    [SerializeField] private float turnSmoothTime = 0.1f;

    CharacterController player = null;
    private Transform curPoint = null;
    private float turnSmoothVelocity;
    private Camera mainCam = null;
    private JButler_Crosshair cross = null;
    DebugScript ds = null;
    bool once = false;
    JButler_MissChance curSpread = null;
    JButler_Shooting shooting = null;
    JButler_ChangeCamera cam = null;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = FindObjectOfType<Camera>();
        cross = FindObjectOfType<JButler_CrosshairManager>().Crosshair();
        ds = FindObjectOfType<DebugScript>();
        player = FindObjectOfType<CharacterController>();
        cam = FindObjectOfType<JButler_ChangeCamera>();
        if (GetComponent<JButler_Shooting>().IsPlayer())
            shooting = GetComponent<JButler_Shooting>();

        SetOurPoint();

        CheckFields();
    }

    // Update is called once per frame
    void Update()
    {
        ChooseSpread();

        if (isPlayer && Time.timeScale > 0)
        {
            SetOurPoint();
            SetTarget();
            MoveSpreadArea();
        }
        //else if (Time.timeScale > 0)
        //    EnemyTarget();

        if (curPoint != null && !ds.DebugActive() && isPlayer && Time.timeScale > 0 && !cam.ActiveNeutral())
        {
            AimAt(curPoint, target);
            once = false;
        }
        else if (!once && isPlayer && Time.timeScale > 0)
        {
            ResetRotation();
            once = true;
        }
    }

    private void FixedUpdate()
    {

    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public Vector3 RayDirection(Transform pointA, Transform pointB)
    {
        Vector3 ourPoint = pointA.position;
        Vector3 tarPoint = pointB.position;
        Vector3 aim = (tarPoint - ourPoint).normalized;
        return aim;
    }

    public LayerMask Layers()
    {
        return layers;
    }

    public void RandomSpread()
    {
        foreach (Transform spread in spread)
            spread.position = curSpread.AIDirection();
    }

    public Transform RandomShoot()
    {
        target.position = curSpread.AIDirection();
        return target;
    }

    public void AimAt(Transform pointA, Transform pointB)
    {
        Vector3 aim = RayDirection(pointA, pointB);

        float rotateY = Mathf.Atan2(aim.x, aim.z) * Mathf.Rad2Deg;
        float rotateX = Mathf.Atan(-aim.y) * Mathf.Rad2Deg;
        float angleY = Mathf.SmoothDampAngle(pointA.eulerAngles.y, rotateY, ref turnSmoothVelocity, turnSmoothTime);
        float angleX = Mathf.SmoothDampAngle(pointA.eulerAngles.x, rotateX, ref turnSmoothVelocity, turnSmoothTime);
        pointA.eulerAngles = new Vector3(angleX, angleY, 0f);

        #region DEBUGGING
#if TEST
        Debug.DrawLine(pointA.position, pointB.position, Color.red); // Correct Direction
        //Debug.DrawRay(pointA.position, pointA.forward, Color.green); // Wrong Direction
#endif
        #endregion
    }

    public void EnemyTarget()
    {
        target.position = targetArea.AIDirection();
    }

    public Transform OurPoint()
    {
        return curPoint;
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void MoveSpreadArea()
    {
        Vector3 rayOrigin = shooting.Here().position;
        Vector3 dir = target.position - rayOrigin;
        float direction = dir.magnitude;
        dir /= direction;
        Ray ray = new Ray(rayOrigin, dir);
        #region DEBUGGING
#if TEST2
        Debug.DrawRay(rayOrigin, dir, Color.white);
#endif
        #endregion

        if (Physics.Raycast(ray, Mathf.Infinity))
        {
            if (shooting.OurCurrentWeapon() == JButler_Shooting.WeaponType.Shotgun)
                curSpread.transform.position = ray.GetPoint(ShotgunSpreadDistance);
            else if (shooting.OurCurrentWeapon() == JButler_Shooting.WeaponType.AR)
                curSpread.transform.position = ray.GetPoint(aRSpreadDistance);
        }
    }

    void ChooseSpread()
    {
        foreach (JButler_MissChance spread in spreadArea)
            if (spread.gameObject.activeInHierarchy)
                curSpread = spread;
    }

    void ResetRotation()
    {
        curPoint.rotation = Quaternion.LookRotation(player.transform.forward);
    }

    private void SetTarget()
    {
        Vector3 rayOrigin = mainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, mainCam.transform.forward, out hit, Mathf.Infinity, layers))
        {
            target.position = hit.point;
            JButler_EnemyTarget enemy = null;
            JButler_Target tar = null;
            if (hit.collider.GetComponent<JButler_EnemyTarget>())
                enemy = hit.collider.GetComponent<JButler_EnemyTarget>();
            else
                enemy = null;
            if (hit.collider.GetComponent<JButler_Target>())
                tar = hit.collider.GetComponent<JButler_Target>();
            else
                tar = null;

            RayCheck();
            ShootingRay();

            if (enemy && !RayCheck() && cross != null && !enemy.e.IsDead())
                cross.ColorWhite();
            else if (enemy && RayCheck() && !ShootingRay() && cross != null && !enemy.e.IsDead())
                cross.ColorGrey();
            else if (enemy && RayCheck() && ShootingRay() && cross != null && !enemy.e.IsDead())
                cross.ColorRed();
            else if (tar && !RayCheck() && cross != null)
                cross.ColorWhite();
            else if (tar && RayCheck() && !ShootingRay() && cross != null)
                cross.ColorGrey();
            else if ((hit.collider.CompareTag("1 Point") || hit.collider.CompareTag("2 Point")) && tar && RayCheck() && ShootingRay() && cross != null)
                cross.ColorYellow();
            else if ((hit.collider.CompareTag("3 Point") || hit.collider.CompareTag("4 Point")) && tar && RayCheck() && ShootingRay() && cross != null)
                cross.ColorOrange();
            else if (hit.collider.CompareTag("5 Point") && tar && RayCheck() && ShootingRay() && cross != null)
                cross.ColorRed();
            else if (cross != null)
                cross.ColorWhite();
        }
        else
        {
            target.position = (rayOrigin + (mainCam.transform.forward * 100.0f));
            if (cross != null)
                cross.ColorWhite();
        }
    }

    bool ShootingRay()
    {
        return shooting.ShootingRay();
    }

    bool RayCheck()
    {
        return shooting.RayCheck();
    }

    private void SetOurPoint()
    {
        CheckArray();
        foreach (Transform ourPoint in pivots)
        {
            if (ourPoint.gameObject.activeInHierarchy)
            {
                curPoint = ourPoint;
            }
            else
                continue;
        }
    }

    private void CheckArray()
    {
        for (int i = 0; i < pivots.Length; i++)
            if (pivots[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Our Points Element " + i + " returned null</color></b>!\tMake sure to add the correct Transform so that the object can pivot properly.");

        int active = pivots.Length;

        for (int i = 0; i < pivots.Length; i++)
        {
            if (pivots[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                curPoint = null;
                throw new System.Exception("All <b><color=red>Pivots</color></b> in " + name + " are inactive!\tMake sure that there is one location active at all times!");
            }
        }
    }

    private void CheckFields()
    {
        if (mainCam == null)
            throw new System.Exception("I don't know how, but " + name + " can't seem to find the <b><color=red>Main Camera</color></b>!\tI am sure you noticed, but turn the camera on.");

        if (cross == null)
            throw new System.Exception(name + " <b><color=red>could not find JButler_Crosshair</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (isPlayer && shooting == null)
            throw new System.Exception(name + " <b><color=red>could not find JButler_Shooting</color></b>!\tThere should be a JButler_Shooting attached to the ShootingBrain along with this script.");

        if (targetArea == null && !isPlayer)
            throw new System.Exception(name + "'s field <b><color=red>Target Area</color></b> is null!\t\tDid you forget to add the player's miss chance here?");

        if (ds == null)
            throw new System.Exception(name + " <b><color=red>could not find DebugScript</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (player == null)
            throw new System.Exception(name + " <b><color=red>could not find a CharacterController</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (targetArea == null && !isPlayer)
            throw new System.Exception(name + " <b><color=red>could not find a JButler_MissChance</color></b>!\tMake sure that there is an active one in the scene at all times!");

        if (curPoint == null && pivots.Length == 0 && isPlayer)
            throw new System.Exception(name + "'s Pivots <b><color=red>array size is set to 0</color></b>!\tMake sure to add elements to the array.");

        if (target == null)
            throw new System.Exception("<b><color=red>Target is null!</color> </b> \t<i><color=lime>This is one of the MOST important items, make sure that it is set to <b><color=red>ADS_EndPoint</color></b>.</color></i>\nIf you are not messing with the ShootingBrain's aim script, but this somewhere else instead, just know that the 'Target' is the most important part.");
    }
}
