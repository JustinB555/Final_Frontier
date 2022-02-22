//#define DEBUGGING_WP
//#define DEBUGGING_PS
//#define OLD

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSaverScript : MonoBehaviour
{
    Options_Menu options = null;

    /// <summary>
    /// Continue to use player values when you a) Move to Next Level b) Restart at Last Checkpoint c) Continue your game
    /// </summary>
    [Space(5)]
    [Header("Checks")]
    public bool playerValuesUsed = false;
    /// <summary>
    /// Use old player values. Used only for Restart Level.
    /// </summary>
    public bool oldPVUsed = false;
    /// <summary>
    /// Use the world values when you a) Restart at Last Checkpoint b) Continue your game
    /// </summary>
    public bool worldValuesUsed = false;
    public bool isContinue = false;

    [Space(5)]
    [Header("Options")]
    public float musicStore = 1f;
    public float sfxStore = 1f;
    public int qualitySetting = 0;

    [Space(5)]
    [Header("Checkpoints")]
    public int checkpointValue = 0;
    public int sceneValue = 0;

    [Space(5)]
    [Header("Player Values")]
    public int painkillerValue = 0;
    public int bulletTimeVaule = 0;

    [Space(5)]
    [Header("Old Player Values")]
    public int old_painkillerValue = 0;
    public int old_bulletTimeVaule = 0;

    [Space(5)]
    [Header("Weapons")]
    public int weapon1 = 0;
    public int weapon2 = 0;
    public int currentWeapon = 0;

    [Space(5)]
    [Header("Old Weapons")]
    public int old_weapon1 = 0;
    public int old_weapon2 = 0;
    public int old_currentWeapon = 0;

    [Space(5)]
    [Header("Ammos")]
    public int w1_curAmmo = 0;
    public int w1_sprAmmo = 0;
    public int w2_curAmmo = 0;
    public int w2_sprAmmo = 0;
    public int w3_curAmmo = 0;
    public int w3_sprAmmo = 0;
    public int grenadeCount = 0;

    [Space(5)]
    [Header("Old Ammos")]
    public int old_w1_curAmmo = 0;
    public int old_w1_sprAmmo = 0;
    public int old_w2_curAmmo = 0;
    public int old_w2_sprAmmo = 0;
    public int old_w3_curAmmo = 0;
    public int old_w3_sprAmmo = 0;
    public int old_grenadeCount = 0;

    [Space(5)]
    [Header("Enemies")]
    public int enemyCount = 0;
    public Dictionary<int, bool> enemiesAlive = new Dictionary<int, bool>();

    [Space(5)]
    [Header("Weapon Pickups")]
    public int totalWeaponPickups = 0;
    public Dictionary<int, int> wP_WT = new Dictionary<int, int>();
    public Dictionary<int, bool> wP_Fresh = new Dictionary<int, bool>();
    public Dictionary<int, int> wP_CurAmmo = new Dictionary<int, int>();
    public Dictionary<int, int> wP_SprAmmo = new Dictionary<int, int>();

    [Space(5)]
    [Header("Power Sources")]
    public int totalPowerSources = 0;
    public Dictionary<int, bool> ps_On = new Dictionary<int, bool>();

    [Space(5)]
    [Header("Doors")]
    public int totalDoors = 0;
    public Dictionary<int, bool> dr_WasOpen = new Dictionary<int, bool>();

    [Space(5)]
    [Header("Destructibles")]
    public int totalDestructibles = 0;
    public Dictionary<int, bool> des_Destroyed = new Dictionary<int, bool>();

    [Space(5)]
    [Header("Explosives")]
    public int totalExplosiveBarrels = 0;
    public Dictionary<int, bool> exp_Boom = new Dictionary<int, bool>();

    [Space(5)]
    [Header("Camera")]
    public float yAxis = 0.0f;
    public float xAxis = 0.0f;


    private static OptionsSaverScript instance;
    private void Awake()
    {
        Load();
        if (instance == null)
            instance = this;
        else
        {
            isContinue = instance.isContinue;
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    public void TheOlSwitcharoo()
    {
        options = GameObject.Find("OPTIONS").GetComponent<Options_Menu>();
        musicStore = options.musicValue;
        sfxStore = options.sfxValue;
        qualitySetting = options.qsNumber;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        //creates a file in C:// user roaming data. Opens the files to save stuff.
        FileStream file = File.Create(Application.persistentDataPath + "/FS202012_Undesireables.dat");

        PlayerData data = new PlayerData();


        data.checkpointValue = checkpointValue;
        data.sceneValue = sceneValue;

        data.painkillerValue = painkillerValue;
        data.bulletTimeVaule = bulletTimeVaule;

        data.old_painkillerValue = old_painkillerValue;
        data.old_bulletTimeVaule = old_bulletTimeVaule;

        data.weapon1 = weapon1;
        data.weapon2 = weapon2;
        data.currentWeapon = currentWeapon;

        data.old_weapon1 = old_weapon1;
        data.old_weapon2 = old_weapon2;
        data.old_currentWeapon = old_currentWeapon;

        data.w1_curAmmo = w1_curAmmo;
        data.w1_sprAmmo = w1_sprAmmo;
        data.w2_curAmmo = w2_curAmmo;
        data.w2_sprAmmo = w2_sprAmmo;
        data.w3_curAmmo = w3_curAmmo;
        data.w3_sprAmmo = w3_sprAmmo;
        data.grenadeCount = grenadeCount;

        #region DEBUGGING
#if OLD
        Debug.LogError("<b><color=white>Before Save Old Data</color></b>:\n<b><color=orange>Old Handgun</color></b>: <b><color=cyan>[" + old_w1_curAmmo + "/" + old_w1_sprAmmo + "]</color></b>\n<b><color=orange>Old Shotgun</color></b>: <b><color=cyan>[" + old_w2_curAmmo + "/" + old_w2_sprAmmo + "]</color></b>\n<b><color=orange>Old AR</color></b>: <b><color=cyan>[" + old_w3_curAmmo + "/" + old_w3_sprAmmo + "]</color></b>");
#endif
        #endregion
        data.old_w1_curAmmo = old_w1_curAmmo;
        data.old_w1_sprAmmo = old_w1_sprAmmo;
        data.old_w2_curAmmo = old_w2_curAmmo;
        data.old_w2_sprAmmo = old_w2_sprAmmo;
        data.old_w3_curAmmo = old_w3_curAmmo;
        data.old_w3_sprAmmo = old_w3_sprAmmo;
        data.old_grenadeCount = old_grenadeCount;
        #region DEBUGGING
#if OLD
        Debug.LogError("<b><color=teal>After Save Old Data</color></b>:\n<b><color=orange>Old Handgun</color></b>: <b><color=cyan>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>\n<b><color=orange>Old Shotgun</color></b>: <b><color=cyan>[" + data.old_w2_curAmmo + "/" + data.old_w2_sprAmmo + "]</color></b>\n<b><color=orange>Old AR</color></b>: <b><color=cyan>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
        #endregion

        data.enemyCount = enemyCount;
        data.enemiesAlive = enemiesAlive;

        data.totalWeaponPickups = totalWeaponPickups;

        #region DEBUGGING
#if DEBUGGING_WP
        foreach (KeyValuePair<int, int> wt in wP_WT)
        {
            if (wP_Fresh.ContainsKey(wt.Key) && wP_CurAmmo.ContainsKey(wt.Key) && wP_SprAmmo.ContainsKey(wt.Key))
            {
                bool fresh = false;
                wP_Fresh.TryGetValue(wt.Key, out fresh);
                int cur = 0;
                wP_CurAmmo.TryGetValue(wt.Key, out cur);
                int spr = 0;
                wP_SprAmmo.TryGetValue(wt.Key, out spr);
                Debug.Log("<b><color=maroon>Data before Save (#3)</color></b>: <b><color=lime>WT [" + wt.Key + "," + wt.Value + "]</color></b>, <b><color=cyan>Fresh [" + wt.Key + "," + fresh + "]</color></b>, <b><color=orange>Cur [" + wt.Key + "," + cur + "]</color></b>, and <b><color=yellow>Spr [" + wt.Key + "," + spr + "]</color></b>\t\t" + "New values start here");
            }
        }
#endif
        #endregion

        data.wP_WT = wP_WT;
        data.wP_Fresh = wP_Fresh;
        data.wP_CurAmmo = wP_CurAmmo;
        data.wP_SprAmmo = wP_SprAmmo;
        #region DEBUGGING
#if DEBUGGING_WP
        foreach (KeyValuePair<int, int> wt in data.wP_WT)
        {
            if (data.wP_Fresh.ContainsKey(wt.Key) && data.wP_CurAmmo.ContainsKey(wt.Key) && data.wP_SprAmmo.ContainsKey(wt.Key))
            {
                bool fresh = false;
                data.wP_Fresh.TryGetValue(wt.Key, out fresh);
                int cur = 0;
                data.wP_CurAmmo.TryGetValue(wt.Key, out cur);
                int spr = 0;
                data.wP_SprAmmo.TryGetValue(wt.Key, out spr);
                Debug.Log("<b><color=lightblue>Data before Save (#4)</color></b>: <b><color=lime>WT [" + wt.Key + "," + wt.Value + "]</color></b>, <b><color=cyan>Fresh [" + wt.Key + "," + fresh + "]</color></b>, <b><color=orange>Cur [" + wt.Key + "," + cur + "]</color></b>, and <b><color=yellow>Spr [" + wt.Key + "," + spr + "]</color></b>\t\t" + "Should match #3");
            }
        }
#endif
        #endregion

        #region DEBUGGING
#if DEBUGGING_PS
        foreach (KeyValuePair<int, bool> ps in ps_On)
            Debug.Log("<b><color=yellow>PS Data</color></b> <b><color=grey>before Save</color></b>: <b><color=cyan>[" + ps.Key + "," + ps.Value + "]</color></b>");
#endif
        #endregion
        data.totalPowerSources = totalPowerSources;
        data.ps_On = ps_On;
        #region DEBUGGING
#if DEBUGGING_PS
        foreach (KeyValuePair<int, bool> ps in data.ps_On)
            Debug.Log("<b><color=yellow>PS Data</color></b> <b><color=lightblue>after Save</color></b>: <b><color=cyan>[" + ps.Key + "," + ps.Value + "]</color></b>");
#endif
        #endregion

        data.totalDoors = totalDoors;
        data.dr_WasOpen = dr_WasOpen;

        data.totalDestructibles = totalDestructibles;
        data.des_Destroyed = des_Destroyed;

        data.totalExplosiveBarrels = totalExplosiveBarrels;
        data.exp_Boom = exp_Boom;

        data.yAxis = yAxis;
        data.xAxis = xAxis;


        bf.Serialize(file, data);
        file.Close();

        Debug.Log("Save data folder created in: " + Application.persistentDataPath + System.Environment.NewLine + "The file is called FS202012_Undesireables.dat");
    }
    public void SaveAudio()
    {
        BinaryFormatter bf = new BinaryFormatter();
        //creates a file in C:// user roaming data. Opens the files to save stuff.
        FileStream file = File.Create(Application.persistentDataPath + "/FS202012_Undesireables_Audio.dat");

        PlayerData data = new PlayerData();
        data.musicStore = musicStore;
        data.sfxStore = sfxStore;

        data.qualitSetting = qualitySetting;


        bf.Serialize(file, data);
        file.Close();

        Debug.Log("Save data folder created in: " + Application.persistentDataPath + System.Environment.NewLine + "The file is called FS202012_Undesireables_Audio.dat");
    }
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/FS202012_Undesireables.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/FS202012_Undesireables.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            Debug.Log("Loaded data folder from: " + Application.persistentDataPath + System.Environment.NewLine + "The file is called FS202012_Undesireables.dat");



            sceneValue = data.sceneValue;
            checkpointValue = data.checkpointValue;

            painkillerValue = data.painkillerValue;
            bulletTimeVaule = data.bulletTimeVaule;

            old_painkillerValue = data.old_painkillerValue;
            old_bulletTimeVaule = data.old_bulletTimeVaule;

            weapon1 = data.weapon1;
            weapon2 = data.weapon2;
            currentWeapon = data.currentWeapon;

            old_weapon1 = data.old_weapon1;
            old_weapon2 = data.old_weapon2;
            old_currentWeapon = data.old_currentWeapon;

            w1_curAmmo = data.w1_curAmmo;
            w1_sprAmmo = data.w1_sprAmmo;
            w2_curAmmo = data.w2_curAmmo;
            w2_sprAmmo = data.w2_sprAmmo;
            w3_curAmmo = data.w3_curAmmo;
            w3_sprAmmo = data.w3_sprAmmo;
            grenadeCount = data.grenadeCount;

            #region DEBUGGING
#if OLD
            Debug.LogError("<b><color=yellow>Before Load Old Data</color></b>:\n<b><color=orange>Old Handgun</color></b>: <b><color=cyan>[" + data.old_w1_curAmmo + "/" + data.old_w1_sprAmmo + "]</color></b>\n<b><color=orange>Old Shotgun</color></b>: <b><color=cyan>[" + data.old_w2_curAmmo + "/" + data.old_w2_sprAmmo + "]</color></b>\n<b><color=orange>Old AR</color></b>: <b><color=cyan>[" + data.old_w3_curAmmo + "/" + data.old_w3_sprAmmo + "]</color></b>");
#endif
            #endregion
            old_w1_curAmmo = data.old_w1_curAmmo;
            old_w1_sprAmmo = data.old_w1_sprAmmo;
            old_w2_curAmmo = data.old_w2_curAmmo;
            old_w2_sprAmmo = data.old_w2_sprAmmo;
            old_w3_curAmmo = data.old_w3_curAmmo;
            old_w3_sprAmmo = data.old_w3_sprAmmo;
            old_grenadeCount = data.old_grenadeCount;
            #region DEBUGGING
#if OLD
            Debug.LogError("<b><color=silver>After Load Old Data</color></b>:\n<b><color=orange>Old Handgun</color></b>: <b><color=cyan>[" + old_w1_curAmmo + "/" + old_w1_sprAmmo + "]</color></b>\n<b><color=orange>Old Shotgun</color></b>: <b><color=cyan>[" + old_w2_curAmmo + "/" + old_w2_sprAmmo + "]</color></b>\n<b><color=orange>Old AR</color></b>: <b><color=cyan>[" + old_w3_curAmmo + "/" + old_w3_sprAmmo + "]</color></b>");
#endif
            #endregion

            enemyCount = data.enemyCount;
            if (data.enemiesAlive != null)
                enemiesAlive = data.enemiesAlive;

            totalWeaponPickups = data.totalWeaponPickups;

#region DEBUGGING
#if DEBUGGING_WP
            foreach (KeyValuePair<int, int> wt in data.wP_WT)
            {
                if (data.wP_Fresh.ContainsKey(wt.Key) && data.wP_CurAmmo.ContainsKey(wt.Key) && data.wP_SprAmmo.ContainsKey(wt.Key))
                {
                    bool fresh = false;
                    data.wP_Fresh.TryGetValue(wt.Key, out fresh);
                    int cur = 0;
                    data.wP_CurAmmo.TryGetValue(wt.Key, out cur);
                    int spr = 0;
                    data.wP_SprAmmo.TryGetValue(wt.Key, out spr);
                    Debug.Log("<b><color=red>Data before Load (#1)</color></b>: <b><color=lime>WT [" + wt.Key + "," + wt.Value + "]</color></b>, <b><color=cyan>Fresh [" + wt.Key + "," + fresh + "]</color></b>, <b><color=orange>Cur [" + wt.Key + "," + cur + "]</color></b>, and <b><color=yellow>Spr [" + wt.Key + "," + spr + "]</color></b>\t\t" + "Should match #4");
                }
            }
#endif
#endregion

            if (data.wP_WT != null && data.wP_Fresh != null && data.wP_CurAmmo != null && data.wP_SprAmmo != null)
            {
                wP_WT = data.wP_WT;
                wP_Fresh = data.wP_Fresh;
                wP_CurAmmo = data.wP_CurAmmo;
                wP_SprAmmo = data.wP_SprAmmo;
#region DEBUGGING
#if DEBUGGING_WP
                foreach (KeyValuePair<int, int> wt in wP_WT)
                {
                    if (wP_Fresh.ContainsKey(wt.Key) && wP_CurAmmo.ContainsKey(wt.Key) && wP_SprAmmo.ContainsKey(wt.Key))
                    {
                        bool fresh = false;
                        wP_Fresh.TryGetValue(wt.Key, out fresh);
                        int cur = 0;
                        wP_CurAmmo.TryGetValue(wt.Key, out cur);
                        int spr = 0;
                        wP_SprAmmo.TryGetValue(wt.Key, out spr);
                        Debug.Log("<b><color=teal>Data after Load (#2)</color></b>: <b><color=lime>WT [" + wt.Key + "," + wt.Value + "]</color></b>, <b><color=cyan>Fresh [" + wt.Key + "," + fresh + "]</color></b>, <b><color=orange>Cur [" + wt.Key + "," + cur + "]</color></b>, and <b><color=yellow>Spr [" + wt.Key + "," + spr + "]</color></b>\t\t" + "Should match #1");
                    }
                }
#endif
#endregion
            }

            totalPowerSources = data.totalPowerSources;
#region DEBUGGING
#if DEBUGGING_PS
            foreach (KeyValuePair<int, bool> ps in data.ps_On)
                Debug.Log("<b><color=yellow>PS Data</color></b> <b><color=brown>before Load</color></b>: <b><color=cyan>[" + ps.Key + "," + ps.Value + "]</color></b>");
#endif
#endregion
            if (data.ps_On != null)
            {
                ps_On = data.ps_On;
#region DEBUGGING
#if DEBUGGING_PS
                foreach (KeyValuePair<int, bool> ps in ps_On)
                    Debug.Log("<b><color=yellow>PS Data</color></b> <b><color=maroon>after Load</color></b>: <b><color=cyan>[" + ps.Key + "," + ps.Value + "]</color></b>");
#endif
#endregion
            }

            totalDoors = data.totalDoors;
            if (data.dr_WasOpen != null)
                dr_WasOpen = data.dr_WasOpen;

            totalDestructibles = data.totalDestructibles;
            if (data.des_Destroyed != null)
                des_Destroyed = data.des_Destroyed;

            totalExplosiveBarrels = data.totalExplosiveBarrels;
            if (data.exp_Boom != null)
                exp_Boom = data.exp_Boom;

            yAxis = data.yAxis;
            xAxis = data.xAxis;

        }
    }
    public void LoadAudio()
    {
        if (File.Exists(Application.persistentDataPath + "/FS202012_Undesireables_Audio.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/FS202012_Undesireables_Audio.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            musicStore = data.musicStore;
            sfxStore = data.sfxStore;
            qualitySetting = data.qualitSetting;
        }
    }
}

[Serializable]
class PlayerData
{
    public float musicStore;
    public float sfxStore;
    public int qualitSetting;

    public int checkpointValue;
    public int sceneValue;

    public int painkillerValue;
    public int bulletTimeVaule;

    public int old_painkillerValue;
    public int old_bulletTimeVaule;

    public int weapon1;
    public int weapon2;
    public int currentWeapon;

    public int old_weapon1;
    public int old_weapon2;
    public int old_currentWeapon;

    public int w1_curAmmo;
    public int w1_sprAmmo;
    public int w2_curAmmo;
    public int w2_sprAmmo;
    public int w3_curAmmo;
    public int w3_sprAmmo;
    public int grenadeCount;

    public int old_w1_curAmmo;
    public int old_w1_sprAmmo;
    public int old_w2_curAmmo;
    public int old_w2_sprAmmo;
    public int old_w3_curAmmo;
    public int old_w3_sprAmmo;
    public int old_grenadeCount;

    public int enemyCount;
    public Dictionary<int, bool> enemiesAlive;

    public int totalWeaponPickups;
    public Dictionary<int, int> wP_WT;
    public Dictionary<int, bool> wP_Fresh;
    public Dictionary<int, int> wP_CurAmmo;
    public Dictionary<int, int> wP_SprAmmo;

    public int totalPowerSources;
    public Dictionary<int, bool> ps_On;

    public int totalDoors;
    public Dictionary<int, bool> dr_WasOpen;

    public int totalDestructibles;
    public Dictionary<int, bool> des_Destroyed;

    public int totalExplosiveBarrels;
    public Dictionary<int, bool> exp_Boom;

    public float yAxis;
    public float xAxis;
}
