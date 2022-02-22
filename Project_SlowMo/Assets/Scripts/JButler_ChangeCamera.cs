using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class JButler_ChangeCamera : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    [Header("Camera Views")]
    [Tooltip("Cam1 = Neutral Cam\nCam2 = Shooting Cam\nCam 3 = ADS Cam")]
    [SerializeField] private CinemachineFreeLook cam1 = null;
    [SerializeField] private CinemachineFreeLook cam2 = null;
    [SerializeField] private CinemachineFreeLook cam3 = null;

    [Space(5)]
    [Header("Camera Offsets")]
    [Tooltip("This offset deals with Cam 2's Right over-the-shoulder position.")]
    [SerializeField] Vector3 cam2_R_OtS = Vector3.zero;
    [Tooltip("This offset deals with Cam 2's Left over-the-shoulder position.")]
    [SerializeField] Vector3 cam2_L_OtS = Vector3.zero;
    [Tooltip("This offset deals with Cam 3's Right over-the-shoulder position.")]
    [SerializeField] Vector3 cam3_R_OtS = Vector3.zero;
    [Tooltip("This offset deals with Cam 3's Left over-the-shoulder position.")]
    [SerializeField] Vector3 cam3_L_OtS = Vector3.zero;
    [Tooltip("This value changes the lerp from one position to another.\nUsed with the over-the-shoulder changes.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float lerp = 0.0f;
    [Tooltip("This value changes the smoothness of the lerp.")]
    [SerializeField] float smoothing = 0.0f;
    //[Tooltip("This corrects the x_axis of the camera so that you are aiming at the same place.")]
    //[Range(0.0f, 30.0f)]
    //[SerializeField] float aimCorrections = 0.0f;

    [Space(5)]
    [Header("ADS Settings")]
    [Tooltip("This deals with how quickly you go from the shooting cam to the neutral cam.")]
    [SerializeField] private float camSpeed = 5.0f;

    [Space(5)]
    [Header("Camera Checks")]
    [Tooltip("If the player is in combat, do not switch of the Shooting Cam.\nTurn this to ture in areas that you want combat.")]
    public bool inCombat = false;

    float yAxis = 0.0f;
    float xAxis = 0.0f;
    Vector3 cam2_Offset = Vector3.zero;
    Vector3 cam3_Offset = Vector3.zero;

    int active = 100;
    int inactive = 0;
    float timer = 0.0f;
    bool inDive = false;
    bool rightShoulder = true;
    bool lerping = false;
    MovementThirdPerson mtp = null;
    OptionsSaverScript data = null;

    // Start is called before the first frame update
    void Start()
    {
        mtp = GetComponent<MovementThirdPerson>();
        data = FindObjectOfType<OptionsSaverScript>();
        cam2_Offset = cam2.GetComponent<CinemachineCameraOffset>().m_Offset;
        cam3_Offset = cam3.GetComponent<CinemachineCameraOffset>().m_Offset;

        NeutralCam();
        RecallAim();
        CheckShoulder();
    }

    // Update is called once per frame
    void Update()
    {
        StartTimer();

        //ActivateCam();
        ForceFollow();
        StoreAim();
        CheckDive();
        SwitchCombat();
        SwitchShoulders();
        ChangeShoulder();
        SwitchCams(camSpeed);
    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void NeutralCam()
    {
        try
        {
            cam1.Priority = active;
            cam2.Priority = inactive;
            cam3.Priority = inactive;
        }
        catch (System.NullReferenceException e)
        {
            CheckFields(e);
        }
    }

    public void ShootingCam()
    {
        try
        {
            cam1.Priority = inactive;
            cam2.Priority = active;
            cam3.Priority = inactive;
        }
        catch (System.NullReferenceException e)
        {
            CheckFields(e);
        }
    }

    public void ADSCam()
    {
        try
        {
            cam1.Priority = inactive;
            cam2.Priority = inactive;
            cam3.Priority = active;
        }
        catch (System.NullReferenceException e)
        {
            CheckFields(e);
        }
    }

    public int Cam1Priority()
    {
        return cam1.Priority;
    }

    public int Cam2Priority()
    {
        return cam2.Priority;
    }

    public int Cam3Priority()
    {
        return cam3.Priority;
    }

    public void ResetTimer()
    {
        timer = 0.0f;
    }

    public bool ActiveNeutral()
    {
        if (cam1.Priority > 0)
            return true;
        else
            return false;
    }

    public bool ActiveShooting()
    {
        if (cam2.Priority > 0)
            return true;
        else
            return false;
    }

    public bool ActiveADS()
    {
        if (cam3.Priority > 0)
            return true;
        else
            return false;
    }

    public void Recenter(bool debuggerActive)
    {
        if (debuggerActive)
        {
            cam1.m_YAxisRecentering.m_enabled = true;
            cam1.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            cam1.m_YAxisRecentering.m_enabled = false;
            cam1.m_RecenterToTargetHeading.m_enabled = false;
        }

    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void CheckShoulder()
    {
        if (rightShoulder)
            lerp = 1;
        else
            lerp = 0;
    }

    void ChangeShoulder()
    {
        if (lerping)
        {
            UpdateLerp();
            cam2.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.Lerp(cam2_L_OtS, cam2_R_OtS, lerp);
            cam3.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.Lerp(cam3_L_OtS, cam3_R_OtS, lerp);

            if (lerp <= 0)
            {
                lerping = false;
                lerp = 0;
            }
            else if (lerp >= 1)
            {
                lerping = false;
                lerp = 1;
            }
        }

        #region Old
        //cam3_Offset.x = new Vector3(Mathf.Lerp(cam3_L_OtS.x, cam3_R_OtS.x, 0), 0, 0).x;

        //cam2_Offset.x = Mathf.Lerp(cam2_Offset.x, cam2_L_OtS.x, smoothing);
        //cam2_Offset.y = Mathf.Lerp(cam2_Offset.y, cam2_L_OtS.y, smoothing);
        //cam2_Offset.z = Mathf.Lerp(cam2_Offset.z, cam2_L_OtS.z, smoothing);

        //cam3_Offset.x = Mathf.Lerp(cam3_Offset.x, cam3_L_OtS.x, smoothing);
        //cam3_Offset.y = Mathf.Lerp(cam3_Offset.y, cam3_L_OtS.y, smoothing);
        //cam3_Offset.z = Mathf.Lerp(cam3_Offset.z, cam3_L_OtS.z, smoothing);

        //cam2.GetComponent<CinemachineCameraOffset>().m_Offset = cam2_L_OtS;
        //cam2.m_XAxis.Value += aimCorrections;
        //cam3.GetComponent<CinemachineCameraOffset>().m_Offset = cam3_L_OtS;
        //cam3.m_XAxis.Value += aimCorrections;
        #endregion
    }

    void UpdateLerp()
    {
        if (lerping && rightShoulder)
            lerp += Time.deltaTime * smoothing;
        else if (lerping && !rightShoulder)
            lerp -= Time.deltaTime * smoothing;
    }

    void StoreAim()
    {
        yAxis = cam1.m_YAxis.Value;
        xAxis = cam1.m_XAxis.Value;

        data.yAxis = yAxis;
        data.xAxis = xAxis;
    }

    void RecallAim()
    {
        if (!data.playerValuesUsed && !data.oldPVUsed)
        {

        }
        else if (data.playerValuesUsed && !data.oldPVUsed)
        {
            cam1.m_YAxis.Value = data.yAxis;
            cam1.m_XAxis.Value = data.xAxis;
        }
        else if (!data.playerValuesUsed && data.oldPVUsed)
        {

        }
    }

    void CheckDive()
    {
        inDive = mtp.bdActive;
    }

    void SwitchCombat()
    {
        if ((inCombat || inDive) && !Input.GetKey(KeyCode.Mouse1))
        {
            ShootingCam();
        }
    }

    void SwitchCams(float time)
    {
        if (timer >= time && ActiveShooting() && !inCombat && !inDive)
            NeutralCam();
    }

    void StartTimer()
    {
        if (ActiveShooting())
            timer += Time.deltaTime;
    }

    void ForceFollow()
    {
        if (ActiveNeutral())
        {
            cam2.ForceCameraPosition(cam1.transform.position, cam1.transform.rotation);
            cam3.ForceCameraPosition(cam1.transform.position, cam1.transform.rotation);
        }
        else if (ActiveShooting())
        {
            cam1.ForceCameraPosition(cam2.transform.position, cam2.transform.rotation);
            cam3.ForceCameraPosition(cam2.transform.position, cam2.transform.rotation);
        }
        else if (ActiveADS())
        {
            cam1.ForceCameraPosition(cam3.transform.position, cam3.transform.rotation);
            cam2.ForceCameraPosition(cam3.transform.position, cam3.transform.rotation);
        }
    }

    void CheckFields(System.Exception e)
    {
        if (cam1 == null)
            throw new System.Exception(name + "'s field <b><color=red>Cam 1</color></b> is null!\t\tDid you forget to add a CinemachineFreeLook Camera?" + e.Message);

        if (cam2 == null)
            throw new System.Exception(name + "'s field <b><color=red>Cam 2</color></b> is null!\t\tDid you forget to add a CinemachineFreeLook Camera?" + e.Message);

        if (cam3 == null)
            throw new System.Exception(name + "'s field <b><color=red>Cam 3</color></b> is null!\t\tDid you forget to add a CinemachineFreeLook Camera?" + e.Message);
    }

    //////////////////////////////////////////////////
    // Input Methods
    //////////////////////////////////////////////////

    private void ActivateCam()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
            NeutralCam();
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ShootingCam();
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ADSCam();
    }

    void SwitchShoulders()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock) && !inDive)
        {
            lerping = true;
            if (rightShoulder)
                rightShoulder = false;
            else
                rightShoulder = true;
        }
    }
}
