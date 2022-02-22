using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MovementThirdPerson : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    Player_Values pv;
    Game_UI gui;
    JButler_Crosshair jbCrossHair;
    [SerializeField] JButler_Shooting jshoot;

    public GameObject offset;

    Sound_Manager sndmngr = null;
    Time_Manager timeManager = null;

    public Animator anim;

    Vector3 moveDirection;
    public Vector3 direction;

    public float MoveSpeed = 250;


    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 bruhy;
    float anotherFLoat = -9.81f;

    [SerializeField]
    float timer;

    float jumpHeight = 1.5f;

    int thrice = 0;

    public bool crouching = false;
    public bool rolling = false;
    public bool sprinting = false;
    public bool prone = false;
    public bool dive = false;
    bool held;
    bool footstepSoundIsPlaying = false;
    bool ableToStand = true;
    bool tryingToStand = false;
    bool display = false;
    bool groundCheck = false;
    bool onSlope = false;
    bool upSlope = false;
    bool downSlope = false;
    bool isPressing = false;
    bool isFirstCall = true;
    bool transtart = false;
    bool grenadeEmpty = false;
    bool sprintBegin = false;
    bool momsSpaghetti = false;
    bool exhausted = false;
    bool sprintSfx = false;
    bool landing = false;
    public bool airTime = false;
    public bool bdActive = false;
    public bool isAcceptingInput = true;
    public bool isNotTutorialBT = true;
    public bool isNotTutorialCRS = true;
    public GameObject gfxFull;
    public GameObject gfxCrouch;
    public GameObject gfxProne;
    public GameObject gfxDive;

    public Transform throwPoint;
    public Transform throwPointCrouched;
    public Transform throwPointProned;
    public Rigidbody grenade;

    public GameObject speedLines;

    [SerializeField]
    LayerMask ignoreLayers;

    [SerializeField]
    LayerMask transparentIgnore;

    [SerializeField]
    LayerMask slopeMask;

    Scene currScene;

    float rollSpeedTest = 0;

    float sprintTimer = 0;

    float movedist;

    float landingF;

    float textTimer = 0;
    int textBoxSize = 0;

    Text MagText = null;

    RaycastHit crouchHit;
    GameObject obstruction = null;
    Color obColor;
    Color obColorTransparent;
    float interval = 0;

    float intervalG = 0;
    bool blink = false;

    Vector3 compareLocation;
    Vector3 playerLocation;
    Vector3 neutralViewPos;

    [SerializeField] GameObject mainCamera;
    PlayerAnimatorScript animatorScript;
    [SerializeField]
    GameObject ShieldUI = null;

    private void Start()
    {
        //anim = GetComponent<Animator>();
        sndmngr = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        timeManager = GameObject.Find("TimeManager").GetComponent<Time_Manager>();
        gui = GameObject.Find("Game_UI").GetComponent<Game_UI>();
        pv = GetComponent<Player_Values>();
        jbCrossHair = GameObject.Find("Crosshair").GetComponentInChildren<JButler_Crosshair>();
        anim = GetComponent<Animator>();
        animatorScript = FindObjectOfType<PlayerAnimatorScript>();

        currScene = SceneManager.GetActiveScene();

        //jshoot = GetComponent<JButler_Shooting>();

        crouching = false;
        rolling = false;
        footstepSoundIsPlaying = false;
        isAllowedToBD = true;

        ignoreLayers = ~ignoreLayers;
        transparentIgnore = ~transparentIgnore;

        if (currScene.name == "Metric")
        {
            MagText = GameObject.Find("MagText").GetComponent<Text>();
        }

        neutralViewPos = offset.transform.localPosition;
    }
    private void Update()
    {
        if (isAcceptingInput)
        {
            groundCheck = controller.isGrounded;

            if (!groundCheck)
            {
                if (!airTime)
                {
                    landingF += Time.deltaTime;
                    landing = true;
                }
            }

            if (landing)
            {
                if (groundCheck)
                {
                    if(landingF >= 0.75f)
                    {
                        sndmngr.Play("Landing");
                        landing = false;
                        landingF = 0;
                    }
                    else
                    {
                        landingF = 0;
                    }
                }
            }

            if (direction != Vector3.zero)
            {
                movedist = Vector3.Distance(controller.velocity, Vector3.zero);
            }
            else
            {
                movedist = 0;
            }

            //=====Camera Adjustment=====

            if (!prone)
            {
                offset.transform.localPosition = neutralViewPos;
            }
            else
            {
                offset.transform.localPosition = Vector3.zero;
            }

            //=====CROUCH & PRONE=====

            if (Input.GetKeyDown(KeyCode.C) && !airTime == isNotTutorialCRS == true)
            {
                if (!jshoot.isReloading)
                {
                    timer = 0;
                    held = false;
                }
            }


            if (Input.GetKey(KeyCode.C) && !rolling && !airTime && isNotTutorialCRS == true)
            {
                if (!jshoot.isReloading)
                {
                    isPressing = true;

                    if (timer >= 0.5f && held == false)
                    {
                        if (!crouching && !prone && !rolling)
                        {
                            sndmngr.Play("Crouch_1");
                            Prone();
                            held = true;
                        }
                        else if (crouching && !rolling)
                        {
                            sndmngr.Play("Crouch_1");
                            Prone();
                            held = true;
                        }
                        else if (prone && ableToStand)
                        {
                            sndmngr.Play("Crouch_2");
                            Stand();
                            held = true;
                        }
                        else if (prone && !ableToStand)
                        {
                            sndmngr.Play("Crouch_2");
                            Crouch();
                            held = true;
                        }
                    }
                }
                
            }

            if (Input.GetKeyUp(KeyCode.C) && held == false && !airTime && isNotTutorialCRS == true)
            {
                if (!jshoot.isReloading)
                {
                    if (!crouching && !prone && !rolling)
                    {
                        if (direction.magnitude >= 0.1f) //ROLLING
                        {
                            rolling = true;
                            controller.height = 0.5f;
                            controller.center = new Vector3(0, -0.5f, 0);
                            controller.skinWidth = 0.01f;
                            gui.iconText.text = "Rolling";
                            gui.CrouchIcon.SetActive(true);
                            InvokeRepeating("Roll", 0.1f, 0.007f);
                            Invoke("RollTimer", 0.5f);
                            sndmngr.Play("Rolling");

                            //anim.Play("Base Layer.CombatRoll");
                        }
                        else
                        {
                            sndmngr.Play("Crouch_1");
                            Crouch();
                            timer = 0;
                            isPressing = false;
                        }

                    }
                    else if (crouching && !rolling && ableToStand)
                    {
                        if (direction.magnitude >= 0.1f) //ROLLING
                        {
                            rolling = true;
                            controller.height = 0.5f;
                            controller.center = new Vector3(0, -0.5f, 0);
                            controller.skinWidth = 0.01f;
                            gui.iconText.text = "Rolling";
                            gui.CrouchIcon.SetActive(true);
                            InvokeRepeating("Roll", 0.1f, 0.005f);
                            Invoke("RollTimer", 0.5f);
                            sndmngr.Play("Rolling");

                            //anim.Play("Base Layer.CombatRoll");
                        }
                        else
                        {
                            sndmngr.Play("Crouch_2");
                            Stand();
                            timer = 0;
                            isPressing = false;
                        }
                    }
                    else if (prone)
                    {
                        sndmngr.Play("Crouch_1");
                        Crouch();
                        timer = 0;
                        isPressing = false;
                    }
                    else if (crouching && !ableToStand)
                    {
                        gui.tryFailStand = true;
                        tryingToStand = true;
                    }
                }
                
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                if (!jshoot.isReloading)
                {
                    isPressing = false;
                }
            }

            if (prone && movedist > 0)
            {
                if (gfxProne)
                {
                    sndmngr.Play("Crouch_1");
                    Crouch();
                    Invoke("Stand", 0.33f);
                }
            }

            if (crouching && ableToStand && tryingToStand)
            {
                sndmngr.Play("Crouch_2");
                Stand();
                tryingToStand = false;
            }

            //=====SPRINTING=====

            
            if (Input.GetKeyDown(KeyCode.LeftControl) && movedist > 1.4f && !sprinting && !airTime && ShieldUI.activeInHierarchy)
            {
                if (crouching && ableToStand)
                {
                    /*sprintTimer += Time.deltaTime;
                    if(sprintTimer > 3 && !exhausted)
                    {
                        sndmngr.Play("Exhausted");
                        exhausted = true;
                    }*/
                    sprintSfx = true;
                    sprintBegin = true;
                    Invoke("sprintChange", 0.15f);
                    Stand();
                    SprintOnOff(400);
                    gui.CrouchIcon.SetActive(true);
                    gui.iconText.text = "Sprinting";
                    display = true;
                    sprinting = true;
                }
                else if (!crouching && !prone)
                {
                    /*sprintTimer += Time.deltaTime;
                    if (sprintTimer > 3 && !exhausted)
                    {
                        sndmngr.Play("Exhausted");
                        exhausted = true;
                    }*/
                    sprintSfx = true;
                    SprintOnOff(400);
                    gui.CrouchIcon.SetActive(true);
                    gui.iconText.text = "Sprinting";
                    display = true;
                    sprinting = true;
                }
                else if (crouching && !ableToStand)
                {
                    gui.tryFailStand = true;
                    tryingToStand = true;
                    sprinting = true;
                }

            }
            else if (movedist <= 2.5f && !airTime)
            {
                if (crouching)
                {
                    SprintOnOff(200);
                    sprinting = false;
                    sprintSfx = false;
                    sprintTimer = 0;
                    exhausted = false;
                }
                else if (!crouching && !prone && !sprintBegin)
                {
                    SprintOnOff(250);
                    sprinting = false;
                    if (!rolling)
                    {
                        gui.CrouchIcon.SetActive(false);
                    }
                    display = false;
                    sprintSfx = false;
                    sprintTimer = 0;
                    exhausted = false;
                }
                else if (prone)
                {
                    SprintOnOff(75);
                    sprinting = false;
                    sprintSfx = false;
                    sprintTimer = 0;
                    exhausted = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && prone)
            {
                Crouch();
            }

            //Exhausted SFX Handling

            if (sprintSfx)
            {
                sprintTimer += Time.deltaTime;
                if (sprintTimer > 3 && !exhausted)
                {
                    sndmngr.Play("Exhausted");
                    exhausted = true;
                }
            }

            if (sprintTimer > 6)
            {
                exhausted = false;
                sprintTimer = 0;
            }

            //======SHOOT DODGE=======
            if (Input.GetKeyDown(KeyCode.Space) && !airTime && Time.timeScale > 0 && isAllowedToBD && isNotTutorialBT == true)
            {
                if (!jshoot.isReloading)
                {
                    if (!rolling && !prone)
                    {
                        MoveSpeed = 5f;
                        timeManager.SlowMotion();
                        timeManager.ToggleSlowOn();
                        sndmngr.Play("Jump");
                        speedLines.SetActive(true);
                        airTime = true;
                        bdActive = true;
                        isFirstCall = true;
                        pv.bulletTime = true;
                    }
                }
            }


            if (airTime)
            {

                Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 1.15f, transform.position.z), Color.red);
                if (Physics.Raycast(transform.position, Vector3.down, 1.15f, ignoreLayers))
                {

                    if (bdToggle)
                    {
                        BDStop();
                    }
                }
            }


            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            direction = new Vector3(horizontal, 0f, vertical).normalized;

            //GRENADES

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (pv.grenadeCount > 0)
                {
                    Rigidbody grenadeInstance;

                    if (gfxFull.activeInHierarchy)
                    {
                        grenadeInstance = Instantiate(grenade, throwPoint.position, throwPoint.rotation) as Rigidbody;
                        grenadeInstance.AddForce(Vector3.Cross(throwPoint.right, Quaternion.AngleAxis(-50, throwPoint.right) * throwPoint.up) * 125);
                        sndmngr.Play("GrenadeThrow");
                        pv.grenadeCount -= 1;
                    }
                    else if (gfxCrouch.activeInHierarchy)
                    {
                        grenadeInstance = Instantiate(grenade, throwPointCrouched.position, throwPointCrouched.rotation) as Rigidbody;
                        grenadeInstance.AddForce(Vector3.Cross(throwPointCrouched.right, Quaternion.AngleAxis(-50, throwPointCrouched.right) * throwPointCrouched.up) * 100);
                        sndmngr.Play("GrenadeThrow");
                        pv.grenadeCount -= 1;
                    }
                    else if (gfxProne.activeInHierarchy)
                    {
                        grenadeInstance = Instantiate(grenade, throwPointProned.position, throwPointProned.rotation) as Rigidbody;
                        grenadeInstance.AddForce(Vector3.Cross(throwPointProned.right, Quaternion.AngleAxis(65, throwPointProned.right) * throwPointProned.up) * 100);
                        sndmngr.Play("GrenadeThrow");
                        pv.grenadeCount -= 1;
                    }
                }
                else
                {
                    Debug.Log("Out of Grenades");
                    sndmngr.Play("Fail");
                    grenadeEmpty = true;
                    thrice = 0;
                }

            }

            if (grenadeEmpty)
            {
                //Icon
                gui.grenadeText.color = Color.Lerp(Color.white, Color.red, intervalG);

                if (intervalG < 1 && blink == false)
                {
                    intervalG += 0.04f;
                }
                else
                {
                    blink = true;
                    if (intervalG > 1)
                    {
                        thrice += 1;
                    }
                }

                if (intervalG > 0 && blink == true)
                {
                    intervalG -= 0.04f;
                }
                else
                {
                    blink = false;
                    if (thrice >= 3)
                    {
                        thrice = 0;
                    }
                }

                //Text
                textTimer += Time.deltaTime;
                
                if(textTimer >= 3)
                {
                    if(textBoxSize > 0)
                    {
                        textBoxSize -= 25;
                        gui.noGrenade.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textBoxSize);
                        if (textBoxSize <= 280)
                        {
                            gui.noGrenadeText.text = "";
                        }

                        if (textBoxSize <= 0)
                        {
                            grenadeEmpty = false;
                            textTimer = 0;
                            gui.grenadeText.color = Color.white;
                        }
                    }
                    //Mathf.Clamp(textBoxSize, 0, 525);
                    
                }
                else
                {
                    if(textBoxSize <= 300)
                    {
                        textBoxSize += 25;
                        gui.noGrenade.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textBoxSize);
                        if (textBoxSize >= 280)
                        {
                            gui.noGrenadeText.text = "NO GRENADES";
                        }
                    }
                    //Mathf.Clamp(textBoxSize, 0, 525);
                    
                }
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, slopeMask))
        {
            //Debug.Log("Floor Angle: " + hit.normal);
            if (hit.normal != Vector3.up)
            {
                onSlope = true;
            }
            else
            {
                onSlope = false;
                upSlope = false;
                downSlope = false;
            }
        }
        else
        {
            onSlope = false;
            upSlope = false;
            downSlope = false;
        }
    }
    void FixedUpdate()
    {
        if (isPressing)
        {
            timer += 0.01f;
        }

        //locking rotation of the entire model to when either the player's aiming or not
        if (animatorScript != null && animatorScript.curAimPercent > 0.0f || direction.magnitude > 0)
        {
            transform.rotation = Quaternion.Euler(0f, mainCamera.transform.rotation.eulerAngles.y, 0f);
        }

        if (direction.magnitude > 0 && !airTime)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (!rolling)
            {
                if (!upSlope && !downSlope)
                {
                    controller.SimpleMove(moveDirection.normalized * MoveSpeed * Time.fixedDeltaTime);
                }
                else if (upSlope)
                {
                    controller.SimpleMove(moveDirection.normalized * (MoveSpeed * 0.5f) * Time.fixedDeltaTime);
                }
                else if (downSlope)
                {
                    if (controller.isGrounded)
                    {
                        controller.SimpleMove(moveDirection.normalized * MoveSpeed * Time.fixedDeltaTime);
                        controller.Move(Vector3.down * MoveSpeed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        controller.SimpleMove(moveDirection.normalized * MoveSpeed * Time.fixedDeltaTime);
                    }

                }

                if (!footstepSoundIsPlaying)
                {
                    sndmngr.Play("Walking");
                    footstepSoundIsPlaying = true;
                    Invoke("ToggleFootStep", 0.578f);
                }
            }
        }
        else if (airTime)
        {
            if (momsSpaghetti)
            {
                controller.Move((Physics.gravity * Time.fixedDeltaTime) * 0.5f);
            }
        }
        else
        {
            controller.Move(Physics.gravity * Time.deltaTime);
        }



        Debug.DrawRay(transform.position, new Vector3(moveDirection.x, -0.75f, moveDirection.z), Color.blue);
        RaycastHit floorCheck;
        if (Physics.Raycast(transform.position, new Vector3(moveDirection.x, -0.75f, moveDirection.z), out floorCheck, 3, slopeMask))
        {
            //Debug.Log("Distance: " + Vector3.Distance(transform.position, floorCheck.point));
            if (movedist > 0)
            {
                if (floorCheck.distance < 1.15f)
                {
                    //Debug.Log("Check");
                    upSlope = true;
                    downSlope = false;
                }
            }

        }
        else
        {
            if (movedist > 0)
            {
                downSlope = true;
                upSlope = false;
            }
        }


        if (currScene.name == "Metric")
        {
            //Debug.Log("Magnitude: " + moveDirection.magnitude);
            MagText.text = ("Current Magnitude = " + moveDirection.magnitude);
        }


        #region //Crouching Transparency
        /*
        if (crouching || rolling || prone || dive)
        {

            if (Physics.Raycast(transform.position, Vector3.up, out crouchHit, 0.5f, transparentIgnore))
            {
                //Debug.Log("Cannot Stand Here");
                ableToStand = false;


                if (!transtart)
                {
                    obstruction = crouchHit.transform.gameObject;
                    if (obstruction.name != "Icon" && obstruction.GetComponent<Renderer>())
                    {
                        obColor = obstruction.GetComponent<Renderer>().material.color;
                        obColorTransparent = new Color(obColor.r, obColor.g, obColor.b, 0.15f);
                        transtart = true;
                    }
                }

                if (obstruction.name != "Icon" && obstruction.GetComponent<Renderer>())
                {
                    obstruction.GetComponent<Renderer>().material.color = Color.Lerp(obColor, obColorTransparent, interval);
                }

                if (interval < 1)
                {
                    interval += 0.01f;
                }
            }
            else
            {
                ableToStand = true;
                if (obstruction != null && obstruction.name != "Icon" && obstruction.GetComponent<Renderer>())
                {
                    obstruction.GetComponent<Renderer>().material.color = Color.Lerp(obColor, obColorTransparent, interval);
                    if (interval > 0)
                    {
                        interval -= 0.015f;
                    }
                    else
                    {
                        transtart = false;
                        obstruction = null;
                    }
                }

            }


            Debug.DrawRay(transform.position, Vector3.up, Color.cyan);
        }
        else
        {
            if (obstruction != null && obstruction.GetComponent<Renderer>())
            {
                obstruction.GetComponent<Renderer>().material.color = Color.Lerp(obColor, obColorTransparent, interval);
                if (interval > 0)
                {
                    interval -= 0.015f;
                }
                else
                {
                    transtart = false;
                    obstruction = null;
                }
            }
        }*/
        #endregion

        if (rolling)
        {
            controller.Move(Vector3.down * MoveSpeed * Time.fixedDeltaTime);
        }

        //Bullet Dodge Cont.

        if (airTime)
        {
            if (isFirstCall)
            {
                Invoke("BulletDodgeOff", 0.45f);
                Invoke("bdToggleMethod", 0.25f);
                InvokeRepeating("BulletDodgeOn", 0.1f, 0.001f);
                InvokeRepeating("BDMoveDirection", 0.1f, 0.001f);
                isFirstCall = false;
                Dive();
                momsSpaghetti = false;
            }
        }
    }


    void BDStop()
    {
        CancelInvoke("BulletDodgeOn");
        CancelInvoke("BDMoveDirection");
        CancelInvoke("BulletDodgeOff");
        CancelInvoke("bdToggleMethod");
        speedLines.SetActive(false);
        airTime = false;
        bdToggle = false;
        bdActive = false;
        pv.bulletTime = false;
        isAllowedToBD = false;
        Invoke("ToggleAllowedToBD", 1f);
        MoveSpeed = 250;
        timeManager.NormalMotion();
        Prone();
        sndmngr.Play("Land");
    }

    bool isAllowedToBD = false;
    void ToggleAllowedToBD()
    {
        isAllowedToBD = true;
    }

    bool bdToggle = false;
    void bdToggleMethod()
    {
        bdToggle = true;
    }

    void BulletDodgeOn()
    {
        if (timeManager.slowOn)
        {
            controller.Move(Vector3.up * jumpHeight * Time.fixedDeltaTime);
        }
        else
        {
            controller.Move(Vector3.up * (jumpHeight * 0.1f) * Time.fixedDeltaTime);
        }
    }
    void BDMoveDirection()
    {
        if (timeManager.slowOn)
        {
            controller.Move(moveDirection * MoveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if (momsSpaghetti)
            {
                controller.Move(moveDirection * (MoveSpeed * 0.45f) * Time.fixedDeltaTime);
            }
            else
            {
                controller.Move(moveDirection * (MoveSpeed * 0.15f) * Time.fixedDeltaTime);
            }
        }

        if (controller.collisionFlags != CollisionFlags.None)
        {
            Debug.Log("Ht Ground");
            BDStop();
        }
    }


    void BulletDodgeOff()
    {
        if (bdToggle)
        {
            CancelInvoke("BulletDodgeOn");
            gfxFull.SetActive(false);
            gfxDive.SetActive(false);
            gfxProne.SetActive(true);
            controller.skinWidth = 0.01f;
            momsSpaghetti = true;
        }
    }

    private void ToggleFootStep()
    {
        footstepSoundIsPlaying = !footstepSoundIsPlaying;
    }

    private void SprintOnOff(int speed)
    {
        MoveSpeed = speed;
    }
    public void Crouch()
    {
        gfxFull.SetActive(false);
        gfxCrouch.SetActive(true);
        gfxProne.SetActive(false);
        gfxDive.SetActive(false);
        gui.iconText.text = "Crouching";
        gui.CrouchIcon.SetActive(true);
        crouching = true;
        prone = false;
        MoveSpeed = 200;
        controller.skinWidth = 0.01f;
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 0.5f;
    }

    public void Stand()
    {
        gfxFull.SetActive(true);
        gfxCrouch.SetActive(false);
        gfxProne.SetActive(false);
        gfxDive.SetActive(false);
        gui.CrouchIcon.SetActive(false);
        prone = false;
        crouching = false;
        MoveSpeed = 250;
        controller.skinWidth = 0.15f;
        controller.center = Vector3.zero;
        controller.height = 1.7f;
    }

    public void Prone()
    {
        gfxProne.SetActive(true);
        gfxCrouch.SetActive(false);
        gfxFull.SetActive(false);
        gfxDive.SetActive(false);
        gui.CrouchIcon.SetActive(true);
        gui.iconText.text = "Prone";
        crouching = false;
        prone = true;
        MoveSpeed = 75;
    }

    public void Dive()
    {
        gfxDive.SetActive(true);
        gfxFull.SetActive(false);
        gfxProne.SetActive(false);
        gfxCrouch.SetActive(false);
        gui.CrouchIcon.SetActive(true);
        gui.iconText.text = "Diving";
        dive = true;
    }

    public void Roll()
    {
        if (timeManager.slowOn)
        {
            if (!crouching)
            {
                rollSpeedTest = 18;
            }
            else
            {
                rollSpeedTest = 10;
            }
        }
        else
        {
            if (!crouching)
            {
                rollSpeedTest = 3;
            }
            else
            {
                rollSpeedTest = 1.5f;
            }
        }

        MoveSpeed = rollSpeedTest;
        gfxFull.SetActive(false);
        gfxCrouch.SetActive(true);
        controller.Move(moveDirection.normalized * MoveSpeed * Time.fixedDeltaTime);
    }

    public bool Rolling()
    {
        return rolling;
    }

    void sprintChange()
    {
        sprintBegin = false;
    }

    public void RollTimer()
    {
        CancelInvoke("Roll");
        rolling = false;

        if (ableToStand)
        {
            Stand();
        }
        else
        {
            Crouch();
        }
    }

    public void startMeleeAnim()
    {
        if (!airTime)
        {
            if (gfxFull.activeInHierarchy)
            {
                anim.SetBool("isMelee", true);
            }

            if (gfxCrouch.activeInHierarchy)
            {
                anim.SetBool("isMeleeCrouched", true);
            }
        }

    }
    public void resetMeleeAnim()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isMeleeCrouched", false);
    }

    public void MeleeSFX()
    {
        sndmngr.Play("MeleeWhoosh");
    }
}
