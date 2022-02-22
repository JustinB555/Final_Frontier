//#define FIND // Raycast hit to detect player
//#define KNOW // Raycast towards player
//#define HIDE // Distance to nearest hiding point
//#define DISTANCE // Distance to player from Agent
//#define VELOCITY // Test for movement direction
//#define CROUCH // Crouch precentage

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JButler_Agent : ProjectSlowMo.StateMachine.JButler_MachineBehaviour
{
    public enum AggroLevel { Level1, Level2, Level3, Level4, COUNT }

    public enum EnemyType { Default, Riot, COUNT }

    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public or SerializeField

    [Space(5)]
    [Header("Aggro Level")]
    [Tooltip("This will control how aggresive this enemy is when they see the player. Set their default aggro level, but this can be changed later in code.")]
    public AggroLevel aggroLevel = AggroLevel.Level1;

    [Space(5)]
    [Header("Enemy Variations")]
    [Tooltip("Select different modifications to the this enemy here.")]
    public EnemyType enemyType = EnemyType.Default;

    [Space(5)]
    [Header("Find Player's Location")]
    [Tooltip("This is the enemy's location or Head/Eyes.")]
    [SerializeField] Transform[] eyePoints = null;
    [Tooltip("This is the enemy's arms.")]
    [SerializeField] Transform[] armPoints = null;
    [Space(5)]
    [Tooltip("Use this ONLY IF YOU WANT the agent to look at something else first.\nEx. Jacob's TargetSwitchTrigger")]
    public Transform fakePoint = null;
    [Tooltip("This is the player's location.\nFollow 'Enemy_View'")]
    [SerializeField] Transform[] endPoints = null;
    [Tooltip("Put the player's SK here. This will cause the enemy to look at the player's direction.")]
    [SerializeField] Transform sK_Player = null;

    [Space(5)]
    [Header("Miss Zones")]
    [Tooltip("This is the default miss zone, or the the miss zone connected to the player.")]
    [SerializeField] JButler_MissChance playerZone = null;
    [Tooltip("This the last known position miss zone. We use this when the AI doesn't know where the player is.")]
    [SerializeField] JButler_MissChance unknownZone = null;

    [Space(5)]
    [Header("Hiding Spots")]
    [Tooltip("Put the nav points that are the hiding spots for the enemy. These are normally behind a wall or some type of cover.")]
    [SerializeField] JButler_NavPoints[] hidingPoints = null;

    [Space(5)]
    [Header("Body Positions")]
    [Tooltip("Put the Enemy_Stand here.")]
    [SerializeField] GameObject standPosition = null;
    [Tooltip("Put the Enemy_Crouch here.")]
    [SerializeField] GameObject crouchPosition = null;

    [Space(5)]
    [Header("References")]
    [Tooltip("We need to reference this Enemy's JButler_Aim script.")]
    [SerializeField] JButler_Aim aiming = null;
    [Tooltip("We need to know who's ShootingBrain we are talking too.\nPut this Enemy's Shooting Brain here.")]
    [SerializeField] JButler_Shooting shooting = null;
    [Tooltip("Put this enemy's shield here.")]
    [SerializeField] GameObject ourShield = null;
    [Tooltip("This is the animator that controls the enemy's animations.")]
    [SerializeField] Animator anim = null;

    [Space(5)]
    [Header("Movement")]
    [Tooltip("This value helps smooth out how quickly the ourPoint looks at the endPoint.")]
    [SerializeField] float turnSmoothTime = 0.1f;
    [Tooltip("Walk speed for when it approaches the player.")]
    [SerializeField] float walkSpeed = 1.5f;
    [Tooltip("Run speed for when you want the enemy to rush the player.")]
    [SerializeField] float runSpeed = 5.0f;
    [Tooltip("This is the default speed for all enemies")]
    [SerializeField] float defaultSpeed = 3.5f;
    [Tooltip("Acceleration for animation.")]
    [SerializeField] float accel = 1.0f;

    [Space(5)]
    [Header("Past Data")]
    [Tooltip("This is the last known position of the player.")]
    [SerializeField] Vector3 lastKnownPosition;
    [Tooltip("The max distance we are allowed to be away from the player.")]
    [SerializeField] float maxDistance = 30.0f;

    [Space(5)]
    [Header("Timing Values")]
    [Tooltip("If you want random times instead.")]
    [SerializeField] bool randomTime = false;
    [Tooltip("This deals with how long the enemy peeks out.")]
    [SerializeField] float peekTime = 1.0f;
    [Tooltip("This is a buffer for how long the enemy looks for the player before moving.")]
    [SerializeField] float peekBuffer = 0.5f; 
    [Tooltip("This is how frequently the enemy peeks out.")]
    [SerializeField] float peekFreq = 1.0f;
    [Tooltip("This is how many times the enemy will continue to fire after the player leaves its sight.")]
    [SerializeField] int blindFire = 3;
    [Tooltip("This bool will allow the enemy to fire in bursts")]
    public bool canFire = false;
    [Tooltip("This will allow the fire rate to be modified")]
    [SerializeField] float rateMod = 1.0f;
    [Tooltip("This value deals with how long the enemy stays active in a scene after dying before despawning.")]
    [SerializeField] float despawnTime = 1.0f;
    [Tooltip("This is how long the enemy will be surprised for.")]
    [SerializeField] float surprisedTime = 1.0f;

    [Space(5)]
    [Header("Icon UI")]
    [Tooltip("This is the canvas that will turn on when the enemy sees the player for the first time.")]
    [SerializeField] GameObject icon = null;

    // private
    Player_Values player = null;
    JButler_Enemy us = null;
    JButler_NavPoints curHiding = null;
    JButler_NavPoints lasHiding = null;
    Sound_Manager sndmngr = null;
    Transform curEye = null;
    Transform curArm = null;
    Transform curEnd = null;
    Vector3 runPosition;

    float turnSmoothVelocity;
    float peekTimer = 0.0f;
    float peekBufferTime = 0.0f;
    float timeMod = 1.0f;
    bool playerHidden = false;
    bool isCrouching = false;
    bool toggleCrouch = false;
    bool isPeeking = false;
    int noPlayer = 0;
    float aimPercent = 0.0f;
    float crouchPercent = 0.0f;
    bool startBuffer = false;


    //////////////////////////////////////////////////
    // OVERRIDE METHODS
    //////////////////////////////////////////////////

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        player = FindObjectOfType<Player_Values>();
        us = GetComponent<JButler_Enemy>();
        sndmngr = FindObjectOfType<Sound_Manager>();
        SetArms();
        SetEyes();
        SetEnds();
        TurnOnShield();

        CheckFields();
    }

    public override void AddStates()
    {
        AddState<ExampleState>();
        AddState<Default_State>();
        AddState<Engage_State>();
        AddState<Aggro1_State>();
        AddState<Aggro2_State>();
        AddState<Aggro3_State>();
        AddState<Aggro4_State>();
        AddState<Dead_State>();

        FindInitialState();
    }

    public override void Update()
    {
        base.Update();

        #region TESTING
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            Shoot();

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            Reload();

        if (Input.GetKeyDown(KeyCode.Keypad7))
            ChangeState<Engage_State>();

        if (Input.GetKeyDown(KeyCode.Keypad8))
            ToggleCrouch();

        if (Input.GetKeyDown(KeyCode.Keypad9))
            Peeking();
#else
        
#endif
        #endregion

        DeadAgent();

        if (Time.timeScale > 0 && !IsCurrentState<Dead_State>())
        {
            SetEyes();
            SetArms();
            SetEnds();
        }
        if (curEye != null && curEnd != null && curArm != null && Time.timeScale > 0 && !IsCurrentState<Dead_State>() && fakePoint == null)
        {
            ActiveLooking();
            ActiveAiming();
            CheckBodyPosition();
        }
        else if (curEye != null && curEnd != null && curArm != null && Time.timeScale > 0 && !IsCurrentState<Dead_State>() && fakePoint != null)
        {
            FakeLooking();
            FakeAiming();
            //CheckBodyPosition(); // Maybe later...
        }
        else
            CheckFields();
        CheckVisibility();

        if (Time.timeScale > 0 && canFire && shooting.GunTimer() >= (shooting.FireRate() * timeMod) && !IsCurrentState<Dead_State>() && (!IsCurrentState<Aggro3_State>() && !IsCurrentState<Aggro1_State>()))
            Shoot();
        else if (Time.timeScale > 0 && canFire && shooting.GunTimer() >= (shooting.FireRate() * timeMod) && !IsCurrentState<Dead_State>() && (IsCurrentState<Aggro3_State>() || IsCurrentState<Aggro1_State>()))
        {
            noPlayer++;
            Shoot();
        }

        if (Time.timeScale > 0 && isCrouching && !IsCurrentState<Dead_State>())
            peekTimer += Time.deltaTime;
        if (Time.timeScale > 0 && !isCrouching && !IsCurrentState<Dead_State>() && startBuffer && GetComponent<NavMeshAgent>().isStopped)
        {
            peekBufferTime += Time.deltaTime;
            Peeking();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        #region DEBUGGING
#if VELOCITY
        Debug.Log("<b><color=blue>Enemy's Velocity Z: " + GetComponent<NavMeshAgent>().velocity.z + "</color></b>");
        Debug.Log("<b><color=red>Enemy's Velocity X: " + GetComponent<NavMeshAgent>().velocity.x + "</color></b>");
#endif
        #endregion

        ChangeAnimationParameters();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    //////////////////////////////////////////////////
    // Helper Methods (Override)
    //////////////////////////////////////////////////

    public void FindInitialState()
    {
        SetInitialState<Default_State>();
    }

    void CheckFields()
    {
        if (aiming == null)
            throw new System.Exception(name + "'s field <b><color=red>Aiming</color></b> is null!\t\tDid you forget to add the ShootingBrain to Aiming?");

        if (shooting == null)
            throw new System.Exception(name + "'s field <b><color=red>Shooting</color></b> is null!\t\tDid you forget to add the ShootingBrain to Shooting?");

        if (playerZone == null)
            throw new System.Exception(name + "'s field <b><color=red>Player Zone</color></b> is null!\t\tDid you forget to add the player's miss chance here?");

        if (unknownZone == null)
            throw new System.Exception(name + "'s field <b><color=red>Unknown Zone</color></b> is null!\t\tDid you forget to add the enemy's miss chance here?");

        if (player == null)
            throw new System.Exception(name + "  could not find an object that uses <b><color=red>Player_Values</color></b>!\tMake sure that there is an active one in your scene as this is the player.");

        if (us == null)
            throw new System.Exception(name + "  could not find an object that uses <b><color=red>JButler_Enemy</color></b> attached to this agent!\tMake sure that there is an active one on this GameObject.");

        if (curEye == null && eyePoints.Length == 0)
            throw new System.Exception(name + "'s Eye Points <b><color=red>array size is set to 0</color></b>!\tMake sure to add elements to the array.");

        if (curArm == null && armPoints.Length == 0)
            throw new System.Exception(name + "'s Arm Points <b><color=red>array size is set to 0</color></b>!\tMake sure to add elements to the array.");

        if (curEnd == null && endPoints.Length == 0)
            throw new System.Exception(name + "'s End Points <b><color=red>array size is set to 0</color></b>!\tMake sure to add elements to the array.");

        if (curHiding == null && hidingPoints.Length == 0)
            throw new System.Exception(name + "'s Hiding Points <b><color=red>array size is set to 0</color></b>!\tMake sure add elements to the array.");

        if (standPosition == null)
            throw new System.Exception(name + "'s field <b><color=red>Stand Position</color></b> is null!\t\tDid you forget to add the Enemy_Stand GameObject to Stand Position?");

        if (crouchPosition == null)
            throw new System.Exception(name + "'s field <b><color=red>Crouch Position</color></b> is null!\t\tDid you forget to add the Enemy_Crouch GameObject to Crouch Position?");

        if (enemyType == EnemyType.Riot && ourShield == null)
            throw new System.Exception(name + " is missing <color=red><b>Our Shield!</b></color>\tMake sure to reference the shield that is on the enemy.");
    }

    void CheckHiding()
    {
        for (int i = 0; i < hidingPoints.Length; i++)
            if (hidingPoints[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Hiding Points Element " + i + " returned null</color></b>!\tMake sure to either add JButler_NavPoint or change the array size.");

        int active = hidingPoints.Length;

        for (int i = 0; i < hidingPoints.Length; i++)
        {
            if (hidingPoints[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                curHiding = null;
                throw new System.Exception("All <b><color=red>Hiding Points</color></b> in " + name + " are inactive!\tMake sure that there is one hiding point active at all times!");
            }
        }
    }

    void CheckEyes()
    {
        for (int i = 0; i < eyePoints.Length; i++)
            if (eyePoints[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Eye Points Element " + i + " returned null</color></b>!\tMake sure to either add the correct eyes or change the array size.");

        int active = eyePoints.Length;

        for (int i = 0; i < eyePoints.Length; i++)
        {
            if (eyePoints[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                curEye = null;
                throw new System.Exception("All <b><color=red>Eye Points</color></b> in " + name + " are inactive!\tMake sure that there is one eye active at all times!");
            }
        }
    }

    void CheckArms()
    {
        for (int i = 0; i < armPoints.Length; i++)
            if (armPoints[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>Arm Points Element " + i + " returned null</color></b>!\tMake sure to either add the correct arm or change the array size.");

        int active = armPoints.Length;

        for (int i = 0; i < armPoints.Length; i++)
        {
            if (armPoints[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                curArm = null;
                throw new System.Exception("All <b><color=red>Arm Points</color></b> in " + name + " are inactive!\tMake sure that there is one arm active at all times!");
            }
        }
    }

    void CheckEnds()
    {
        for (int i = 0; i < endPoints.Length; i++)
            if (endPoints[i] != null)
                continue;
            else
                throw new System.Exception("<b><color=red>End Points Element " + i + " returned null</color></b>!\tMake sure to either add the correct Enemy_View or change the array size.");

        int active = endPoints.Length;

        for (int i = 0; i < endPoints.Length; i++)
        {
            if (endPoints[i].gameObject.activeInHierarchy)
                active++;
            else
                active--;
            if (active == 0)
            {
                curEnd = null;
                throw new System.Exception("All <b><color=red>End Points</color></b> in " + name + " are inactive!\tMake sure that there is one arm active at all times!");
            }
        }
    }

    //////////////////////////////////////////////////
    // Helper Methods (Direction)
    //////////////////////////////////////////////////

    public void FindDestination()
    {
        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponent<NavMeshAgent>().SetDestination(lastKnownPosition);
    }

    public void SetRunDestination()
    {
        runPosition = lastKnownPosition;
    }

    public void FindRunDestination()
    {
        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponent<NavMeshAgent>().SetDestination(runPosition);
    }

    public void StopDestination()
    {
        GetComponent<NavMeshAgent>().isStopped = true;
    }

    public void SetHiding()
    {
        CheckHiding();
        float distance = 0.0f;
        lasHiding = curHiding;
        isPeeking = false;

        foreach (JButler_NavPoints point in hidingPoints)
        {
            if (distance == 0.0f && point != lasHiding && aggroLevel == AggroLevel.Level1)
            {
                distance = Vector3.Distance(point.transform.position, transform.position);
                curHiding = point;
            }
            else if (distance == 0.0f && point != lasHiding && aggroLevel == AggroLevel.Level2)
            {
                distance = Vector3.Distance(point.transform.position, curEnd.transform.position);
                curHiding = point;
            }
            if (distance > Vector3.Distance(point.transform.position, transform.position) && point != lasHiding && aggroLevel == AggroLevel.Level1)
            {
                distance = Vector3.Distance(point.transform.position, transform.position);
                curHiding = point;
            }
            else if (distance > Vector3.Distance(point.transform.position, curEnd.transform.position) && point != lasHiding && aggroLevel == AggroLevel.Level2)
            {
                distance = Vector3.Distance(point.transform.position, curEnd.transform.position);
                curHiding = point;
            }
            else
                continue;
        }
        #region DEBUGGING
#if HIDE
            Debug.Log("Current Hiding Point: " + curHiding);
#endif
        #endregion
    }

    public float VelocityX()
    {
        return GetComponent<NavMeshAgent>().velocity.x;
    }

    public float VelecityZ()
    {
        return GetComponent<NavMeshAgent>().velocity.z;
    }

    public void FindHiding()
    {
        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponent<NavMeshAgent>().SetDestination(curHiding.transform.position);
    }

    public JButler_NavPoints LasHiding()
    {
        return lasHiding;
    }

    public Transform CurHiding()
    {
        return curHiding.transform;
    }

    //////////////////////////////////////////////////
    // Helper Methods (Player related)
    //////////////////////////////////////////////////

    float DistanceFromPlayer()
    {
        float distance = Vector3.Distance(curEnd.transform.position, transform.position);
        #region DEBUGGING
#if DISTANCE
        Debug.Log("Distance to player is <b><color=lime>" + distance + "</color></b>.");
#endif
        #endregion

        return distance;
    }

    Vector3 RayDirection(Transform pointA, Transform pointB)
    {
        Vector3 ourPoint = pointA.position;
        Vector3 tarPoint = pointB.position;
        Vector3 aim = (tarPoint - ourPoint).normalized;
        return aim;
    }

    void FindPlayer(Transform pointA, Transform pointB)
    {
        Vector3 aim = RayDirection(pointA, pointB);

        float rotateY = Mathf.Atan2(aim.x, aim.z) * Mathf.Rad2Deg;
        float rotateX = Mathf.Atan(-aim.y) * Mathf.Rad2Deg;
        //float angleY = Mathf.SmoothDampAngle(pointA.eulerAngles.y, rotateY, ref turnSmoothVelocity, turnSmoothTime);
        //float angleX = Mathf.SmoothDampAngle(pointA.eulerAngles.x, rotateX, ref turnSmoothVelocity, turnSmoothTime);
        pointA.eulerAngles = new Vector3(rotateX, rotateY, 0f);

        #region DEBUGGING
#if KNOW
        Debug.DrawLine(pointA.position, pointB.position, Color.red); // Correct Direction
        Debug.DrawRay(pointA.position, pointA.forward, Color.green); // Wrong Direction
#endif
        #endregion
    }

    public void LookAtPlayer(Transform pointA, Transform pointB)
    {
        Vector3 aim = RayDirection(pointA, pointB);

        float rotateY = Mathf.Atan2(aim.x, aim.z) * Mathf.Rad2Deg;
        pointA.eulerAngles = new Vector3(0f, rotateY, 0f);
    }

    public void ActiveLooking()
    {
        FindPlayer(curEye, curEnd);
    }

    public void FakeLooking()
    {
        FindPlayer(curEye, fakePoint);
        LookAtPlayer(standPosition.transform, fakePoint);
    }

    void AimAtPlayer(Transform pointA, Transform pointB)
    {
        Vector3 aim = RayDirection(pointA, pointB);

        float rotateY = Mathf.Atan2(aim.x, aim.z) * Mathf.Rad2Deg;
        float rotateX = Mathf.Atan(-aim.y) * Mathf.Rad2Deg;
        float angleY = Mathf.SmoothDampAngle(pointA.eulerAngles.y, rotateY, ref turnSmoothVelocity, turnSmoothTime);
        float angleX = Mathf.SmoothDampAngle(pointA.eulerAngles.x, rotateX, ref turnSmoothVelocity, turnSmoothTime);
        pointA.eulerAngles = new Vector3(angleX, angleY, 0f);

        #region DEBUGGING
#if KNOW
        Debug.DrawLine(pointA.position, pointB.position, Color.red); // Correct Direction
        Debug.DrawRay(pointA.position, pointA.forward, Color.green); // Wrong Direction
#endif
        #endregion
    }

    public void ActiveAiming()
    {
        AimAtPlayer(curArm, curEnd);
        //standPosition.transform.LookAt(curEnd);
        #region DEBUGGING
#if VELOCITY
        Debug.Log("<b><color=yellow>IsStopped = " + GetComponent<NavMeshAgent>().isStopped + "</color></b>");
#endif
        #endregion
        // Aggresive guys
        if ((aggroLevel == AggroLevel.Level3 || aggroLevel == AggroLevel.Level4) && anim.GetFloat("Aim Percent") < 1)
        {
            aimPercent += Time.deltaTime * accel;
            anim.SetFloat("Aim Percent", aimPercent);
        }
        // When shooting
        else if ((aggroLevel == AggroLevel.Level1 || aggroLevel == AggroLevel.Level2) && GetComponent<NavMeshAgent>().isStopped &&anim.GetFloat("Aim Percent") < 1 && !isCrouching)
        {
            aimPercent += Time.deltaTime * accel;
            anim.SetFloat("Aim Percent", aimPercent);
        }
        // When crouching
        else if ((aggroLevel == AggroLevel.Level1 || aggroLevel == AggroLevel.Level2) && GetComponent<NavMeshAgent>().isStopped && anim.GetFloat("Aim Percent") > 0 && isCrouching)
        {
            aimPercent -= Time.deltaTime * accel;
            anim.SetFloat("Aim Percent", aimPercent);
        }
    }

    public void FakeAiming()
    {
        AimAtPlayer(curArm, fakePoint);
    }

    public bool PlayerHidden()
    {
        Vector3 rayOrigin = Vector3.zero;

        if (Time.timeScale > 0 && !IsCurrentState<Dead_State>())
        {
            SetEyes();
            SetArms();
            SetEnds();
        }

        if (eyePoints != null && endPoints != null)
            rayOrigin = curEye.position;
        else
            CheckFields();
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, RayDirection(curEye, curEnd), out hit, Mathf.Infinity, aiming.Layers()) && fakePoint == null)
        {
            if (hit.collider.GetComponent<JButler_BodyPart>())
                playerHidden = false;
            else
                playerHidden = true;
        }

        return playerHidden;
    }

    public void CheckVisibility()
    {
        Vector3 rayOrigin = Vector3.zero;
        if (eyePoints != null && endPoints != null)
            rayOrigin = curEye.position;
        else
            CheckFields();
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, RayDirection(curEye, curEnd), out hit, Mathf.Infinity, aiming.Layers()) && fakePoint == null)
        {
            if (IsCurrentState<Default_State>() && hit.collider.GetComponent<JButler_BodyPart>() && !us.IsDead())
                ChangeState<Engage_State>();

            if (hit.collider.GetComponent<JButler_BodyPart>())
            {
                playerHidden = false;
                lastKnownPosition = hit.point;
                unknownZone.transform.position = lastKnownPosition;
                aiming.targetArea = playerZone;
            }
            else
            {
                playerHidden = true;
                aiming.targetArea = unknownZone;
            }

            #region DEBUGGING
#if FIND
            if (hit.collider.GetComponent<JButler_BodyPart>())
                Debug.Log(name + " has found the player!");
            else
                Debug.Log(name + " can't find the player.\nWe hit <b><color=red>" + hit.transform.name + "</color></b> instead!");
            Debug.DrawLine(curEye.position, curEnd.position, Color.magenta);
#endif
            #endregion
        }
    }

    //////////////////////////////////////////////////
    // Helper Methods (AI related)
    //////////////////////////////////////////////////

    public void Surprised()
    {
        icon.SetActive(true);
    }

    public void UnSurprised()
    {
        icon.SetActive(false);
    }

    public float SurprisedTimer()
    {
        RandomTimes();
        return surprisedTime;
    }

    public void IconLookAt()
    {
        icon.transform.LookAt(player.transform);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }

    public float DespawnTimer()
    {
        return despawnTime;
    }

    public void SetEyes()
    {
        CheckEyes();
        foreach (Transform point in eyePoints)
        {
            if (point.gameObject.activeInHierarchy)
                curEye = point;
            else
                continue;
        }
    }

    public void SetArms()
    {
        CheckArms();
        foreach (Transform point in armPoints)
        {
            if (point.gameObject.activeInHierarchy)
                curArm = point;
            else
                continue;
        }
    }

    public void SetEnds()
    {
        CheckEnds();
        foreach (Transform point in endPoints)
        {
            if (point.gameObject.activeInHierarchy)
                curEnd = point;
            else
                continue;
        }
    }

    public void RandomTimes()
    {
        if (randomTime)
        {
            peekFreq = Random.Range(0.1f, 7.5f);
            peekTime = Random.Range(0.1f, 3.0f);
            surprisedTime = Random.Range(1.0f, 3.5f);
        }
    }

    public void Shoot()
    {
        Aim();
        shooting.Shoot();
        if (shooting.CurAmmo() <= 0)
            Reload();
    }

    void Aim()
    {
        aiming.EnemyTarget();
    }

    public void Reload()
    {
        shooting.EnemyReload();
    }

    public void ToggleCrouch()
    {
        toggleCrouch = !toggleCrouch;
        if (toggleCrouch)
            Crouch();
        else
            Stand();
    }

    public void Stand()
    {
        //standPosition.SetActive(true);
        //crouchPosition.SetActive(false);
        if (Time.timeScale > 0 && !IsCurrentState<Dead_State>())
        {
            SetEyes();
            SetArms();
            SetEnds();
        }
        CheckBodyPosition();
        //standPosition.transform.LookAt(curEnd);
        LookAtPlayer(standPosition.transform, sK_Player);
        isCrouching = false;
        peekTimer = 0.0f;
        startBuffer = true;
    }

    public void Crouch()
    {
        //standPosition.SetActive(false);
        //crouchPosition.SetActive(true);
        isCrouching = true;
        isPeeking = false;
        CheckBodyPosition();
        Hiding();
        peekBufferTime = 0.0f;
        startBuffer = false;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public void PeekFrequency()
    {
        if (peekTimer >= peekFreq && isCrouching)
        {
            StopDestination();
            Peeking();
        }
    }


    public void Peeking()
    {
        if (aggroLevel == AggroLevel.Level1)
        {
            isPeeking = true;
            Stand();
            if (!PlayerHidden() && peekBufferTime >= peekBuffer)
            {
                canFire = true;
                noPlayer = 0;
                Invoke("Crouch", peekTime);
            }
            else if (playerHidden && noPlayer < blindFire && peekBufferTime >= peekBuffer)
            {
                canFire = true;
                Invoke("Crouch", peekTime);
            }
            else if (peekBufferTime >= peekBuffer)
            {
                Invoke("Crouch", peekTime);
            }

        }
        else if (aggroLevel == AggroLevel.Level2)
        {
            isPeeking = true;
            Stand();
            #region DEBUGGING
#if FIND
            Debug.Log("<b><color=yellow>PlayerHidden before</color></b> = <b><color=lime>" + playerHidden + "</color></b>");
#endif
            #endregion
            if (!PlayerHidden() && peekBufferTime >= peekBuffer)
            {
                canFire = true;
                Invoke("Crouch", peekTime);
            }
            else if (peekBufferTime >= peekBuffer)
            {
                SetHiding();
                LookAtPlayer(standPosition.transform, curHiding.transform);
                FindHiding();
                peekBufferTime = 0.0f;
                startBuffer = false;
            }
            #region DEBUGGING
#if FIND
            Debug.LogError("<b><color=yellow>PlayerHidden after</color></b> = <b><color=cyan>" + playerHidden + "</color></b>");
#endif
            #endregion

        }
    }

    public void Hiding()
    {
        if (aggroLevel == AggroLevel.Level1)
        {
            canFire = false;
            RandomTimes();
        }
        else if (aggroLevel == AggroLevel.Level2)
        {
            canFire = false;
            RandomTimes();
        }
    }

    public bool IsPeeking()
    {
        return isPeeking;
    }

    public void Stand_n_Fire()
    {
        if (aggroLevel == AggroLevel.Level3)
        {
            canFire = true;
            if (playerHidden && noPlayer < blindFire)
            {
                SetWalk();
                FindDestination();
                timeMod = rateMod;
            }
            else if (!playerHidden)
            {
                StopDestination();
                //standPosition.transform.LookAt(curEnd);
                LookAtPlayer(standPosition.transform, sK_Player);
                noPlayer = 0;
                timeMod = 1.0f;
            }
            else
            {
                SetWalk();
                FindDestination();
                canFire = false;
            }
        }
        else if (aggroLevel == AggroLevel.Level4)
        {
            if (!playerHidden && GetComponent<NavMeshAgent>().remainingDistance <= 0 && DistanceFromPlayer() <= maxDistance)
            {
                canFire = true;
                //standPosition.transform.LookAt(curEnd);
                LookAtPlayer(standPosition.transform, sK_Player);
            }
            else if (!playerHidden && GetComponent<NavMeshAgent>().remainingDistance <= 0 && DistanceFromPlayer() > maxDistance)
            {
                canFire = false;
                SetRunDestination();
                FindRunDestination();
            }
        }
    }

    public void SetWalk()
    {
        GetComponent<NavMeshAgent>().speed = walkSpeed;
    }

    public void SetRun()
    {
        GetComponent<NavMeshAgent>().speed = runSpeed;
    }

    public void SetSpeed()
    {
        GetComponent<NavMeshAgent>().speed = defaultSpeed;
    }

    public void DeadAgent()
    {
        if (us.IsDead() && enemyType == EnemyType.Riot && !IsCurrentState<Dead_State>() && us.CurHealth() <= 0)
        {
            ToggleShield();
            sndmngr.Play("ShieldDeactivate");
        }
        else if (us.IsDead() && enemyType == EnemyType.Riot && !IsCurrentState<Dead_State>() && us.CurHealth() > 0)
            ToggleShield();
        if (us.IsDead())
        {
            us.TurnOffKinematic();
            ChangeState<Dead_State>();
        }
    }

    public void DetermineAggro()
    {
        switch (aggroLevel)
        {
            case AggroLevel.Level1:
                ChangeState<Aggro1_State>();
                break;

            case AggroLevel.Level2:
                ChangeState<Aggro2_State>();
                break;

            case AggroLevel.Level3:
                ChangeState<Aggro3_State>();
                break;

            case AggroLevel.Level4:
                ChangeState<Aggro4_State>();
                break;

            default:
                throw new System.Exception(name + " is <b><color=red>not set to an aggro level</color></b>.\tProbably is set to COUNT.");
        }
    }

    public void ToggleShield()
    {
        if (ourShield.activeInHierarchy)
            ourShield.SetActive(false);
        else
            ourShield.SetActive(true);
    }

    public void TurnOnShield()
    {
        if (enemyType == EnemyType.Riot)
            ToggleShield();
    }

    //////////////////////////////////////////////////
    // Helper Methods (Animation)
    //////////////////////////////////////////////////

    public void ChangeAnimationParameters()
    {
        anim.SetFloat("Velocity X", GetComponent<NavMeshAgent>().velocity.x);
        anim.SetFloat("Velocity Z", GetComponent<NavMeshAgent>().velocity.z);
    }

    public void CheckBodyPosition()
    {
        if (isCrouching && Time.timeScale > 0)
        {
            crouchPercent += Time.deltaTime * accel;
            if (crouchPercent > 1)
                crouchPercent = 1.0f;
            anim.SetFloat("Crouch Percent", crouchPercent);
            #region DEBUGGING
#if CROUCH
            Debug.Log("<b><color=cyan>Crouch Percent = " + crouchPercent + "</color></b>");
#endif
            #endregion
        }
        else if (!isCrouching && Time.timeScale > 0)
        {
            crouchPercent -= Time.deltaTime * accel;
            if (crouchPercent < 0)
                crouchPercent = 0.0f;
            anim.SetFloat("Crouch Percent", crouchPercent);
            #region DEBUGGING
#if CROUCH
            Debug.Log("<b><color=cyan>Crouch Percent = " + crouchPercent + "</color></b>");
#endif
            #endregion

        }
    }


    //////////////////////////////////////////////////
    // Collision Events
    //////////////////////////////////////////////////

    // See JButler_AgentCollisions for these events.

}

//////////////////////////////////////////////////
// STATES
//////////////////////////////////////////////////

public class NavAgentState : ProjectSlowMo.StateMachine.JButler_State
{
    protected JButler_Agent GetAgent()
    {
        return ((JButler_Agent)machine);
    }
}

public class ExampleState : NavAgentState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void PhysicsExecute()
    {
        base.PhysicsExecute();
    }

    public override void PostExecute()
    {
        base.PostExecute();
    }

    public override void OnAnimatorIK(int layerIndex)
    {
        base.OnAnimatorIK(layerIndex);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Default_State : NavAgentState
{
    public override void Enter()
    {
        base.Enter();
    }
}

public class Engage_State : NavAgentState
{
    float timer = 0.0f;

    public override void Enter()
    {
        base.Enter();
        GetAgent().Surprised();
        GetAgent().IconLookAt();
    }

    public override void Execute()
    {
        base.Execute();
        GetAgent().IconLookAt();
        timer += Time.deltaTime;

        if (timer >= GetAgent().SurprisedTimer())
        {
            GetAgent().UnSurprised();
            GetAgent().DetermineAggro();
        }
    }
}

// Hide-n-Shoot
public class Aggro1_State : NavAgentState
{

    public override void Enter()
    {
        base.Enter();

        GetAgent().SetSpeed();
        GetAgent().SetHiding();
        GetAgent().FindHiding();
    }

    public override void Execute()
    {
        base.Execute();

        GetAgent().PeekFrequency();
    }
}

// Hide-n-Run
public class Aggro2_State : NavAgentState
{
    public override void Enter()
    {
        base.Enter();

        GetAgent().SetSpeed();
        GetAgent().SetHiding();
        GetAgent().FindHiding();
    }

    public override void Execute()
    {
        base.Execute();

        GetAgent().PeekFrequency();
    }
}

// Stand-n-Shoot
public class Aggro3_State : NavAgentState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();

        GetAgent().Stand_n_Fire();
    }
}

// Run-n-Shoot
public class Aggro4_State : NavAgentState
{
    public override void Enter()
    {
        base.Enter();

        GetAgent().SetRun();
        GetAgent().SetRunDestination();
        GetAgent().FindRunDestination();
    }

    public override void Execute()
    {
        base.Execute();

        GetAgent().Stand_n_Fire();
    }
}

public class Dead_State : NavAgentState
{
    float timer = 0.0f;

    public override void Enter()
    {
        base.Enter();

        GetAgent().StopDestination();
    }

    public override void Execute()
    {
        base.Execute();
        timer += Time.deltaTime;
        if (timer >= GetAgent().DespawnTimer())
            GetAgent().Despawn();
    }
}