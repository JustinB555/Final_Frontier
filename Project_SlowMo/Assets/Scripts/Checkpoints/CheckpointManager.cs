//#define PILLS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Checkpoint[] checkpoints = null;
    MovementThirdPerson player = null;
    Sound_Manager sndmngr = null;
    GameObject checkpointUI = null;
    public int currentCheckpointValue = 0;
    public int sceneValue = 0;
    public int optionsSaverScriptSceneValue = -1;
    bool toggleBool = false;


    OptionsSaverScript opsavscr = null;
    Game_UI gui = null;
    JButler_EnemyManager nmemngr = null;
    // Start is called before the first frame update
    void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();
        Array.Sort(checkpoints, new CheckpointComparer());
        player = FindObjectOfType<MovementThirdPerson>();
        checkpointUI = GameObject.Find("CheckpointUI");
        sndmngr = FindObjectOfType<Sound_Manager>();
        gui = FindObjectOfType<Game_UI>();
        opsavscr = FindObjectOfType<OptionsSaverScript>();
        nmemngr = FindObjectOfType<JButler_EnemyManager>();

        sceneValue = SceneManager.GetActiveScene().buildIndex;
        optionsSaverScriptSceneValue = opsavscr.sceneValue;
        //Debug.Log("Scene Value = " + sceneValue + "\nOPSVRSCR Scene Value = " + optionsSaverScriptSceneValue + "\nisContinueStatus = " + opsavscr.isContinue);
        currentCheckpointValue = -1;

        if (sceneValue == opsavscr.sceneValue && opsavscr.isContinue)
        {
            currentCheckpointValue = opsavscr.checkpointValue;
            SetPlayerTransformToCheckpointTransform(opsavscr.checkpointValue);
        }
        checkpointUI.SetActive(false);
        RecallPills();
    }

    public void UpdateCurrentCheckpointValue()
    {
        opsavscr.isContinue = false;
        opsavscr.playerValuesUsed = true;
        opsavscr.oldPVUsed = false;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i].IsPlayerInCheckpoint && !checkpoints[i].hasPlayerLeftCheckpoint && checkpoints[i].objectIdentifier > currentCheckpointValue)
            {
                ToggleUI();
                sndmngr.Play("Checkpoint");
                opsavscr.checkpointValue = checkpoints[i].objectIdentifier;
                currentCheckpointValue = checkpoints[i].objectIdentifier;
                StorePills();
                opsavscr.sceneValue = SceneManager.GetActiveScene().buildIndex;

                //Ammo and enemy bullshit here

                opsavscr.Save();
                Invoke("ToggleUI", 1.5f);
            }
            else
            {
                continue;
            }
        }
    }

    public bool StartLevel()
    {
        if (checkpoints[0].IsPlayerInCheckpoint && !checkpoints[0].hasPlayerLeftCheckpoint)
            return true;
        else
            return false;
    }

    public void StorePills()
    {
        #region DEBUGGING
#if PILLS
        Debug.LogError("<b><color=cyan>Painkiller Value Before Store</color></b> = <b><color=lime>" + gui.paki + "</color></b>\n<b><color=cyan>Bullet Time Value Before Store</color></b> = <b><color=lightblue>" + gui.baki + "</color></b>");
#endif
        #endregion
        opsavscr.painkillerValue = gui.paki;
        opsavscr.bulletTimeVaule = gui.baki;
        #region DEBUGGING
#if PILLS
        Debug.LogError("<b><color=silver>Painkiller Value After Store</color></b> = <b><color=lime>" + opsavscr.painkillerValue + "</color></b>\n<b><color=silver>Bullet Time Value After Store</color></b> = <b><color=lightblue>" + opsavscr.bulletTimeVaule + "</color></b>");
#endif
        #endregion


        if (checkpoints[0].IsPlayerInCheckpoint && !checkpoints[0].hasPlayerLeftCheckpoint)
        {
            opsavscr.old_painkillerValue = gui.paki;
            opsavscr.old_bulletTimeVaule = gui.baki;
        }
    }

    void RecallPills()
    {
        if (!opsavscr.playerValuesUsed && !opsavscr.oldPVUsed)
        {

        }
        else if (opsavscr.playerValuesUsed && !opsavscr.oldPVUsed)
        {
            #region DEBUGGING
#if PILLS
            Debug.LogError("<b><color=orange>Painkiller Value Before Recall</color></b> = <b><color=lime>" + opsavscr.painkillerValue+"</color></b>\n<b><color=orange>Bullet Time Value Before Recall</color></b> = <b><color=lightblue>"+opsavscr.bulletTimeVaule+"</color></b>");
#endif
            #endregion
            gui.paki = opsavscr.painkillerValue;
            gui.baki = opsavscr.bulletTimeVaule;
            #region DEBUGGING
#if PILLS
            Debug.LogError("<b><color=yellow>Painkiller Value After Recall</color></b> = <b><color=lime>" + gui.paki + "</color></b>\n<b><color=yellow>Bullet Time Value After Recall</color></b> = <b><color=lightblue>" + gui.baki + "</color></b>");
#endif
            #endregion
        }
        else if (!opsavscr.playerValuesUsed && opsavscr.oldPVUsed)
        {
            gui.paki = opsavscr.old_painkillerValue;
            gui.baki = opsavscr.old_bulletTimeVaule;
        }
    }

    void ToggleUI()
    {
        toggleBool = !toggleBool;
        if (!toggleBool)
            checkpointUI.SetActive(false);
        else
            checkpointUI.SetActive(true);
    }

    public void SetPlayerTransformToCheckpointTransform(int checkpointValue)
    {
        try
        {
            player.controller.enabled = false;
            player.transform.position = checkpoints[checkpointValue].transform.position;
            //Debug.Log("Ceckpoint value: " + checkpointValue);
            player.controller.enabled = true;
        }
        catch (System.NullReferenceException e)
        {
            if (player == null)
                throw new System.Exception(name + "<color=red><b> could not find the PLAYER!</color></b>\n" + e.Message);
            if (!player.GetComponent<MovementThirdPerson>())
                throw new System.Exception(name + "<color=orange><b> could not find the MovementThirdPerson script!</color></b>\n" + e.Message);
            if (player.GetComponent<MovementThirdPerson>().controller == null)
                throw new System.Exception(name + "<color=magenta><b> could not find the Controller!</color></b>\n" + e.Message);
        }
    }
}


public class CheckpointComparer : IComparer<Checkpoint>
{
    public int Compare(Checkpoint a, Checkpoint b)
    {
        return a.name.CompareTo(b.name);
    }
}