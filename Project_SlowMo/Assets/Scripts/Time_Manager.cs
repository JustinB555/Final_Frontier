using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Time_Manager : MonoBehaviour
{

    public float slowFactor = 0.1f;
    public float normFactor = 1.0f;
    //public float TickTime;

    public bool slowOn = false;
    public bool target = false;

    Sound_Manager sndmngr = null;
    Player_Values pv;
    JButler_Crosshair jbCrossHair;
    GameObject invTar;

    [SerializeField] CinemachineFreeLook neutralView = null;
    [SerializeField] CinemachineFreeLook shootingView = null;
    [SerializeField] CinemachineFreeLook adsView = null;

    public void Start()
    {
        sndmngr = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        jbCrossHair = FindObjectOfType<JButler_Crosshair>();
        invTar = GameObject.Find("InvTarget");

        neutralView = GameObject.Find("Third Person Camera (Neutral View)").GetComponent<CinemachineFreeLook>();
        shootingView = GameObject.Find("Third Person Camera (Shooting View)").GetComponent<CinemachineFreeLook>();
        adsView = GameObject.Find("Third Person Camera (ADS View)").GetComponent<CinemachineFreeLook>();

        slowOn = false;

        //Debug.Log(Time.deltaTime);
    }


    public void SlowMotion()
    {
        jbCrossHair.BtChActivate();
        sndmngr.SlowMotionSounds();
        sndmngr.audioSources[4].pitch = 1.9f;
        sndmngr.Play("SlowDown");
        //sndmngr.audioSources[1].pitch = .7f;
        target = true;

        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StartCoroutine(Crosshair());
        invTar.SetActive(false);

        if (neutralView.m_XAxis.m_MaxSpeed == 450)
        {
            //Adjusting sensitivity for Neutral View
            neutralView.m_XAxis.m_MaxSpeed = neutralView.m_XAxis.m_MaxSpeed * 5f;
            neutralView.m_YAxis.m_MaxSpeed = neutralView.m_YAxis.m_MaxSpeed * 4f;
            neutralView.m_XAxis.m_AccelTime = neutralView.m_XAxis.m_AccelTime / 5;
            neutralView.m_XAxis.m_DecelTime = neutralView.m_XAxis.m_DecelTime / 5;
            neutralView.m_YAxis.m_AccelTime = neutralView.m_YAxis.m_AccelTime / 5;
            neutralView.m_YAxis.m_DecelTime = neutralView.m_YAxis.m_DecelTime / 5;

            //Adjusting sensitivity for Shooting View
            shootingView.m_XAxis.m_MaxSpeed = shootingView.m_XAxis.m_MaxSpeed * 5f;
            shootingView.m_YAxis.m_MaxSpeed = shootingView.m_YAxis.m_MaxSpeed * 4f;
            shootingView.m_XAxis.m_AccelTime = shootingView.m_XAxis.m_AccelTime / 5;
            shootingView.m_XAxis.m_DecelTime = shootingView.m_XAxis.m_DecelTime / 5;
            shootingView.m_YAxis.m_AccelTime = shootingView.m_YAxis.m_AccelTime / 5;
            shootingView.m_YAxis.m_DecelTime = shootingView.m_YAxis.m_DecelTime / 5;

            //Adjusting Sensitivity for ADS View
            adsView.m_XAxis.m_MaxSpeed = adsView.m_XAxis.m_MaxSpeed * 5f;
            adsView.m_YAxis.m_MaxSpeed = adsView.m_YAxis.m_MaxSpeed * 4f;
            adsView.m_XAxis.m_AccelTime = adsView.m_XAxis.m_AccelTime / 5;
            adsView.m_XAxis.m_DecelTime = adsView.m_XAxis.m_DecelTime / 5;
            adsView.m_YAxis.m_AccelTime = adsView.m_YAxis.m_AccelTime / 5;
            adsView.m_YAxis.m_DecelTime = adsView.m_YAxis.m_DecelTime / 5;
        }
    }
    public void ToggleSlowOn()
    {
        slowOn = true;
    }

    public void NormalMotion()
    {
        sndmngr.NormMotionSounds();
        slowOn = false;
        sndmngr.audioSources[3].pitch = 1.3f;
        sndmngr.Play("SpeedUp");
        sndmngr.audioSources[1].pitch = 1f;
        invTar.SetActive(true);
        target = false;

        Time.timeScale = normFactor;
        Time.fixedDeltaTime = 0.02f;

        //simple check to see if the player is already at the normal speed before slowing it down
        if (neutralView.m_XAxis.m_MaxSpeed != 450)
        {
            //Adjusting sensitivity for Neutral View
            neutralView.m_XAxis.m_MaxSpeed = neutralView.m_XAxis.m_MaxSpeed / 5f;
            neutralView.m_YAxis.m_MaxSpeed = neutralView.m_YAxis.m_MaxSpeed / 4f;
            neutralView.m_XAxis.m_AccelTime = neutralView.m_XAxis.m_AccelTime * 5;
            neutralView.m_XAxis.m_DecelTime = neutralView.m_XAxis.m_DecelTime * 5;
            neutralView.m_YAxis.m_AccelTime = neutralView.m_YAxis.m_AccelTime * 5;
            neutralView.m_YAxis.m_DecelTime = neutralView.m_YAxis.m_DecelTime * 5;

            //Adjusting sensitivity for Shooting View
            shootingView.m_XAxis.m_MaxSpeed = shootingView.m_XAxis.m_MaxSpeed / 5f;
            shootingView.m_YAxis.m_MaxSpeed = shootingView.m_YAxis.m_MaxSpeed / 4f;
            shootingView.m_XAxis.m_AccelTime = shootingView.m_XAxis.m_AccelTime * 5;
            shootingView.m_XAxis.m_DecelTime = shootingView.m_XAxis.m_DecelTime * 5;
            shootingView.m_YAxis.m_AccelTime = shootingView.m_YAxis.m_AccelTime * 5;
            shootingView.m_YAxis.m_DecelTime = shootingView.m_YAxis.m_DecelTime * 5;

            //Adjusting Sensitivity for ADS View
            adsView.m_XAxis.m_MaxSpeed = adsView.m_XAxis.m_MaxSpeed / 5f;
            adsView.m_YAxis.m_MaxSpeed = adsView.m_YAxis.m_MaxSpeed / 4f;
            adsView.m_XAxis.m_AccelTime = adsView.m_XAxis.m_AccelTime * 5;
            adsView.m_XAxis.m_DecelTime = adsView.m_XAxis.m_DecelTime * 5;
            adsView.m_YAxis.m_AccelTime = adsView.m_YAxis.m_AccelTime * 5;
            adsView.m_YAxis.m_DecelTime = adsView.m_YAxis.m_DecelTime * 5;
        }
    }

    IEnumerator Crosshair()
    {
        yield return new WaitForSeconds(.1f);
        jbCrossHair.BtChDeactivate();
    }
}
