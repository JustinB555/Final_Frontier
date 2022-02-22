using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    bool isDebugActive = false;
    bool isPlayerImmortal = false;
    bool isEnemyImmortal = false;
    bool isBulletTimeUnlimited = false;

    public bool isMouseOnDebug = false;

    int healPlayerValue = 0;
    int damagePlayerValue = 0;
    int storedCheckpointValue = 0;
    int ammoValue = 0;

    float gainBulletTimeValue = 0;
    float loseBulletTimeValue = 0;

    string on = "ON";
    string off = "OFF";

    Scene_Manager scnmngr = null;
    Player_Values plyrvls = null;
    Time_Manager timemngr = null;
    JButler_CursorManager cursermngr = null;
    JButler_EnemyManager enemyMngr = null;
    CheckpointManager cpm = null;
    JButler_ChangeCamera cc = null;
    Game_UI gui = null;
    [SerializeField]
    JButler_Shooting shooting = null;

    [SerializeField]
    GameObject debugCanvas = null;

    [SerializeField]
    Text PITEXT = null;
    [SerializeField]
    Text EITEXT = null;
    [SerializeField]
    Text UBTTEXT = null;

    [SerializeField]
    Text PHValue = null;
    [SerializeField]
    Text PDValue = null;
    [SerializeField]
    Text GBTValue = null;
    [SerializeField]
    Text LBTValue = null;
    [SerializeField]
    Text AmmoValue = null;

    [SerializeField]
    Dropdown checkpointSelector = null;

    

    // Start is called before the first frame update
    void Start()
    {

        scnmngr = FindObjectOfType<Scene_Manager>();
        plyrvls = FindObjectOfType<Player_Values>();
        timemngr = FindObjectOfType<Time_Manager>();
        cursermngr = FindObjectOfType<JButler_CursorManager>();
        enemyMngr = FindObjectOfType<JButler_EnemyManager>();
        cpm = FindObjectOfType<CheckpointManager>();
        cc = FindObjectOfType<JButler_ChangeCamera>();
        gui = FindObjectOfType<Game_UI>();

        //PITEXT = GameObject.Find("PI_Text").GetComponent<Text>();
        //EITEXT = GameObject.Find("EI_Text").GetComponent<Text>();
        //UBTTEXT = GameObject.Find("UBT_Text").GetComponent<Text>();

        //PHValue = GameObject.Find("PH_Text").GetComponent<Text>();
        //PDValue = GameObject.Find("PD_Text").GetComponent<Text>();
        //GBTValue = GameObject.Find("GBT_Text").GetComponent<Text>();
        //LBTValue = GameObject.Find("LBT_Text").GetComponent<Text>();
        //AmmoValue = GameObject.Find("Ammo_Text").GetComponent<Text>();


        //checkpointSelector = GameObject.Find("CP_Dropdown").GetComponent<Dropdown>();

        SettingLabels();

        //debugCanvas = GameObject.Find("Debug_Canvas");
        debugCanvas.SetActive(false);
        isDebugActive = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && Time.timeScale > 0)
        {
            isDebugActive = !isDebugActive;
            cc.Recenter(isDebugActive);
            if (isDebugActive)
            {
                debugCanvas.SetActive(true);
                cursermngr.UnlockCursor();
                cursermngr.ShowCursor();
            }
            else
            {
                debugCanvas.SetActive(false);
                cursermngr.LockCursor();
                cursermngr.HideCursor();
            }
        }
    }

    public bool DebugActive()
    {
        return isDebugActive;
    }

    public void MouseOnDebugToggle()
    {
        isMouseOnDebug = !isMouseOnDebug;
    }

    private void SettingLabels()
    {
        if (isPlayerImmortal)
            PITEXT.text = on;
        else
            PITEXT.text = off;

        if (isEnemyImmortal)
            EITEXT.text = on;
        else
            EITEXT.text = off;

        if (isBulletTimeUnlimited)
            UBTTEXT.text = on;
        else
            UBTTEXT.text = off;
    }

    public void PlayerImmortal()
    {
        isPlayerImmortal = !isPlayerImmortal;
        if (isPlayerImmortal)
        {
            PITEXT.text = on;
            plyrvls.isImmortal = true;
        }
        else
        {
            PITEXT.text = off;
            plyrvls.isImmortal = false;
        }
    }
    public void HealPlayer()
    {
        healPlayerValue = int.Parse(PHValue.GetComponent<Text>().text) * -1;
        plyrvls.TakeDamage(healPlayerValue);
    }
    public void DamagePlayer()
    {
        damagePlayerValue = int.Parse(PDValue.GetComponent<Text>().text);
        plyrvls.TakeDamage(damagePlayerValue);
    }
    public void EnemyImmortal()
    {
        isEnemyImmortal = !isEnemyImmortal;
        if (isEnemyImmortal)
        {
            EITEXT.text = on;
            enemyMngr.ImmortalAll();
        }
        else
        {
            EITEXT.text = off;
            enemyMngr.ImmortalAll();
        }
        //Debug.LogError("This Debug Option Currently Does Nothing. Please Try Again When Something Is Coded Here");
    }
    public void KillAllEnemies()
    {
        //Debug.LogError("This Debug Option Currently Does Nothing. Please Try Again When Something Is Coded Here");
        enemyMngr.KillAll();
    }
    public void UnlimitedBulletTime()
    {
        isBulletTimeUnlimited = !isBulletTimeUnlimited;
        if (isBulletTimeUnlimited)
        {
            UBTTEXT.text = on;
            timemngr.SlowMotion();
        }
        else
        {
            UBTTEXT.text = off;
            timemngr.NormalMotion();
        }
    }
    public void GainBulletTime()
    {
        gainBulletTimeValue = float.Parse(GBTValue.text);
        plyrvls.currBt += gainBulletTimeValue;
        plyrvls.UpdateBtBar();
    }
    public void LoseBulletTime()
    {
        loseBulletTimeValue = float.Parse(LBTValue.text);
        plyrvls.currBt -= loseBulletTimeValue;
        plyrvls.UpdateBtBar();

    }
    public void LoadCheckpoint()
    {
        cpm.SetPlayerTransformToCheckpointTransform(storedCheckpointValue);
    }
    public void UpdateCheckpoint()
    {
        storedCheckpointValue = checkpointSelector.value;
    }
    public void ResetLevel()
    {
        Time.timeScale = 1;
        timemngr.slowOn = false;
        timemngr.NormalMotion();
        scnmngr.RestartCurrentScene();
    }
    public void GainPainkiller()
    {
        gui.paki += 1;
    }
    public void LosePainkiller()
    {
        gui.paki -= 1;
    }
    public void GainAmmo()
    {
        ammoValue = int.Parse(AmmoValue.text);
        shooting.AddAmmo(ammoValue);
        gui.UpdateAmmoText();
    }

    public void PickupAmmo()
    {
        gui.PickupText(shooting.PickupAmmo());
        shooting.AddAmmo(shooting.PickupAmmo());
        gui.UpdateAmmoText();
    }

    public void LoseAmmo()
    {
        ammoValue = int.Parse(AmmoValue.text);
        shooting.SubtractAmmo(ammoValue);
        gui.UpdateAmmoText();
    }
}
