using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAnimatorScript : MonoBehaviour
{
    Animator animator;
    MovementThirdPerson movementthirdperson;
    Player_Values pv;
    AOEAbility aoe;
    Shield shield;
    Game_UI gui;
    [SerializeField]
    JButler_Shooting jbutler_ShootingBrain;
    JButler_ChangeCamera cameraChange;
    [SerializeField] JButler_Aim aim;
    [SerializeField] GameObject leftHandGripPistol = null;
    [SerializeField] GameObject leftHandGripShotty = null;
    [SerializeField] GameObject leftHandGripAR = null;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    float acceleration = 8.0f;
    float deceleration = 8.0f;
    public float maximumJogVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;
    public float weponAimSpeed = 8.0f;
    public bool isSprinting = false;
    public bool canPlayPickUp = false;
    bool forwardPressed;
    bool leftPressed;
    bool rightPressed;
    bool backwardPressed;
    bool isAiming;
    bool isCurrentlyPlayingAnim = false;
    public float pistolOrRifle;
    public float curAimPercent;
    public float aimWeight;
    public float crouchPercent;
    public string anim = "PickUp";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        movementthirdperson = FindObjectOfType<MovementThirdPerson>();
        cameraChange = FindObjectOfType<JButler_ChangeCamera>();
        pv = FindObjectOfType<Player_Values>();
        aoe = FindObjectOfType<AOEAbility>();
        shield = FindObjectOfType<Shield>();
        gui = FindObjectOfType<Game_UI>();
    }

    void changeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backwardPressed, bool isSprinting, float currentMaxVelocity)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    void lockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backwardPressed, bool isSprinting, float currentMaxVelocity)
    {
        if (!forwardPressed && !backwardPressed && velocityZ != 0.0f && (velocityZ > -0.1f && velocityZ < 0.1f))
        {
            velocityZ = 0.0f;
        }

        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.1f && velocityX < 0.1f))
        {
            velocityX = 0.0f;
        }

        //lock forward
        if (forwardPressed && isSprinting && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }
        //lock backward
        if (backwardPressed && isSprinting && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        else if (backwardPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityZ < currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        else if (backwardPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }
        //lock Left
        if (leftPressed && isSprinting && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityX < currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }
        //lock right
        if (rightPressed && isSprinting && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    void AimWeight()
    {
        if (jbutler_ShootingBrain.OurCurrentWeapon() != JButler_Shooting.WeaponType.Handgun && jbutler_ShootingBrain.OurCurrentWeapon() != JButler_Shooting.WeaponType.AR && jbutler_ShootingBrain.OurCurrentWeapon() != JButler_Shooting.WeaponType.Shotgun && aimWeight > 0)
        {
            aimWeight -= Time.deltaTime * weponAimSpeed;
        }
        if (jbutler_ShootingBrain.OurCurrentWeapon() == JButler_Shooting.WeaponType.Handgun || jbutler_ShootingBrain.OurCurrentWeapon() == JButler_Shooting.WeaponType.AR || jbutler_ShootingBrain.OurCurrentWeapon() == JButler_Shooting.WeaponType.Shotgun)
        {
            if (aimWeight < 1)
            {
                aimWeight += Time.deltaTime * weponAimSpeed;
            }
        }
        animator.SetLayerWeight(animator.GetLayerIndex("Aim"), aimWeight);
    }

    void AimPercentage()
    {
        if (cameraChange.Cam1Priority() == 100 && curAimPercent > 0.0f)
        {
            curAimPercent -= Time.deltaTime * weponAimSpeed;
        }

        if (cameraChange.Cam2Priority() == 100 && curAimPercent < 0.5f)
        {
            curAimPercent += Time.deltaTime * weponAimSpeed;
            if (curAimPercent > 0.5f && curAimPercent < (0.5f + 0.05))
            {
                curAimPercent = 0.5f;
            }
        }
        else if (cameraChange.Cam2Priority() == 100 && curAimPercent > 0.5f)
        {
            curAimPercent -= Time.deltaTime * weponAimSpeed;
            if (curAimPercent > 0.5f && curAimPercent < (0.5f - 0.05))
            {
                curAimPercent = 0.5f;
            }
        }

        if (cameraChange.Cam3Priority() == 100 && curAimPercent < 1.0f)
        {
            curAimPercent += Time.deltaTime * weponAimSpeed;
        }

        animator.SetFloat("curAimProgress", curAimPercent);
    }

    void PistolOrRifle()
    {
        if (jbutler_ShootingBrain.OurCurrentWeapon() == JButler_Shooting.WeaponType.Handgun && pistolOrRifle > 0.0f)
        {
            pistolOrRifle = 0.0f;
        }
        if (jbutler_ShootingBrain.OurCurrentWeapon() != JButler_Shooting.WeaponType.Handgun && pistolOrRifle < 1.0f)
        {
            pistolOrRifle = 1.0f;
        }
        animator.SetFloat("pistolToRifle", pistolOrRifle);
    }

    private void OnAnimatorIK()
    {
        //determining if the arms should follow the target point
        if (curAimPercent > 0.0f)
        {
            // Weapon Aim at  Target IK
            //Settign up the grip of the left hand while aiming
            if (pistolOrRifle == 0)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGripPistol.transform.position);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGripShotty.transform.position);
            }
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, aim.target.position);

            //Look at atrget IK
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(aim.target.position);
        }
    }

    void CrouchPercent()
    {
        if (movementthirdperson.crouching && crouchPercent < 1)
        {
            crouchPercent += Time.deltaTime * acceleration;
            animator.SetFloat("crouchPercent", crouchPercent);
        }
        else if (!movementthirdperson.crouching && crouchPercent > 0)
        {
            crouchPercent -= Time.deltaTime * acceleration;
            animator.SetFloat("crouchPercent", crouchPercent);
        }
    }

    void CheckForAction()
    {
        if (!isCurrentlyPlayingAnim)
        {
            if (Input.GetKey(KeyCode.G) && pv.grenadeCount > 0)
            {
                Shield();
            }
            if (Input.GetKey(KeyCode.Alpha1) && !aoe.isInCooldown)
            {
                AOE();
            }
            if (Input.GetKey(KeyCode.Alpha2) && !shield.isCoolDown)
            {
                Shield();
            }
            if (Input.GetKey(KeyCode.Alpha3) && gui.painKillerText.text != "0")
            {
                Heal();
            }
            if (Input.GetKey(KeyCode.Alpha4) && pv.currBt < 100 && gui.btPillText.text != "0")
            {
                Heal();
            }
            if (canPlayPickUp)
            {
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        anim = "PickUp";
        PlayActionAnimation();
    }
    public void Heal()
    {
        anim = "Heal";
        PlayActionAnimation();
    }
    public void Shield()
    {
        anim = "Shield";
        PlayActionAnimation();
    }
    public void AOE()
    {
        anim = "AOE";
        PlayActionAnimation();
    }

    void PlayActionAnimation()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Action Layer"), 1);
        isCurrentlyPlayingAnim = true;
        animator.SetTrigger(anim);
    }
    void AnimationEnd()
    {
        canPlayPickUp = false;
        isCurrentlyPlayingAnim = false;
        animator.SetLayerWeight(animator.GetLayerIndex("Action Layer"), 0);
    }

    // Update is called once per frame
    void Update()
    {
        forwardPressed = Input.GetKey(KeyCode.W);
        leftPressed = Input.GetKey(KeyCode.A);
        rightPressed = Input.GetKey(KeyCode.D);
        backwardPressed = Input.GetKey(KeyCode.S);
        isSprinting = movementthirdperson.sprinting;
        float currentMaxVelocity = isSprinting ? maximumRunVelocity : maximumJogVelocity;
        changeVelocity(forwardPressed, leftPressed, rightPressed, backwardPressed, isSprinting, currentMaxVelocity);
        lockOrResetVelocity(forwardPressed, leftPressed, rightPressed, backwardPressed, isSprinting, currentMaxVelocity);
        CrouchPercent();
        AimWeight();
        AimPercentage();
        PistolOrRifle();
        CheckForAction();

        animator.SetFloat("VelocityZ", velocityZ);
        animator.SetFloat("VelocityX", velocityX);
    }
}
