using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Values : MonoBehaviour
{
    Game_UI gui;
    MovementThirdPerson M3P;
    

    public int maxHealth = 100;
    public int currHealth;

    public bool isImmortal = false;

    int grenadeMax = 3;
    public int grenadeCount;


    public float maxBt = 100.0f;
    public float currBt;
    public bool bulletTime = false;
    public Time_Manager timeManager;

    Sound_Manager sndmngr = null;

    public PainKiller pk = null;
    public BulletTimePicky btp = null;
    OptionsSaverScript data = null;
    CheckpointManager cm = null;


    void Start()
    {
        gui = FindObjectOfType<Game_UI>();
        currHealth = maxHealth;
        gui.SetMaxHealth(maxHealth);

        M3P = GetComponent<MovementThirdPerson>();
        cm = FindObjectOfType<CheckpointManager>();

        currBt = maxBt;
        gui.SetMaxBtAmount(maxBt);

        isImmortal = false;

        sndmngr = FindObjectOfType<Sound_Manager>();
        data = FindObjectOfType<OptionsSaverScript>();

        StartingValues();
    }

    void Update()
    {
#if UNITY_EDITOR
        //damage testing
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            TakeDamage(8);
        }
#else

#endif

        if (currHealth <= 0)
        {
            gui.isGameOver = true;
            sndmngr.Play("Death");
        }
        else if (currHealth > maxHealth)
        {
            currHealth = maxHealth;
        }


        if (currBt > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.timeScale > 0 && M3P.isAcceptingInput && M3P.isNotTutorialBT == true)
            {
                if (bulletTime == false)
                {
                    timeManager.SlowMotion();
                    timeManager.ToggleSlowOn();
                    bulletTime = true;
                    //Debug.Log("SlowMo");
                }
                else if (bulletTime == true)
                {
                    timeManager.NormalMotion();
                    bulletTime = false;
                    //Debug.Log("NormMo");
                }
            }

            if (timeManager.slowOn && Time.timeScale > 0)
            {
                ReduceBt(12f);
            }
        }
        else
        {
            if (timeManager.slowOn && !M3P.bdActive)
            {
                timeManager.NormalMotion();
                bulletTime = false;
                //Debug.Log("NormMo");
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.timeScale > 0)
            {
                if (bulletTime == false)
                {
                    timeManager.SlowMotion();
                    timeManager.ToggleSlowOn();
                    bulletTime = true;
                    //Debug.Log("SlowMo");
                }
                else if (bulletTime == true)
                {
                    timeManager.NormalMotion();
                    bulletTime = false;
                    //Debug.Log("NormMo");
                }
            }
        }
        currBt = Mathf.Clamp(currBt, 0, 100);


        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currHealth > 0)
            {
                pk.PainKillerActivate();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (currBt < 100)
            {
                btp.BTPillActivate();
            }
            
        }

        grenadeCount = Mathf.Clamp(grenadeCount, 0, grenadeMax);
        gui.grenadeText.text = grenadeCount.ToString();
        StoreValues();
        
    }

    void StartingValues()
    {
        // Default
        if (!data.playerValuesUsed && !data.oldPVUsed)
        {
            
        }
        else if (data.playerValuesUsed && !data.oldPVUsed)
        {
            grenadeCount = data.grenadeCount;
        }
        else if (!data.playerValuesUsed && data.oldPVUsed)
        {
            grenadeCount = data.old_grenadeCount;
        }
    }

    void StoreValues()
    {
        data.grenadeCount = grenadeCount;

        if (cm.StartLevel())
            data.old_grenadeCount = grenadeCount;
    }

    public void TakeDamage(int damage)
    {
        if (!isImmortal)
        {
            if (damage >= 0)
            {
                int rand = Random.Range(0, 5);

                if(rand <= 1)
                {
                    sndmngr.Play("TakeDamage");
                    gui.injuryGameobject.SetActive(true);
                }
                else if(rand == 2 || rand == 3)
                {
                    sndmngr.Play("TakeDamage2");
                    gui.injuryGameobject.SetActive(true);
                }
                else if (rand >= 4)
                {
                    sndmngr.Play("TakeDamage3");
                    gui.injuryGameobject.SetActive(true);
                }
            }
            else
            {
                sndmngr.Play("UsedPainkiller");
            }

            currHealth -= damage;

            gui.SetHealth(currHealth);

            if (currHealth <= 25)
            {
                gui.LH_Border.SetActive(true);
            }
            else
            {
                gui.LH_Border.SetActive(false);
            }
        }
    }

    public void ReduceBt(float bulletTime)
    {
        if (Time.timeScale > 0)
        {
            currBt -= bulletTime * Time.unscaledDeltaTime;
            UpdateBtBar();
        }
    }

    public void UpdateBtBar()
    {
        gui.SetBtAmount(currBt);
    }

    public void IncreaseBt(float bulletTime)
    {
        currBt += bulletTime;
        UpdateBtBar();
    }
}

