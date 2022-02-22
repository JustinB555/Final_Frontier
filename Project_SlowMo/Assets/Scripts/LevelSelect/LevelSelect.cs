using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelSelect : MonoBehaviour
{
    Dropdown lvlSelect = null;
    Scene_Manager scnmngr = null;
    OptionsSaverScript data = null;

    int levelSelection = 0;

    private void Start()
    {
        lvlSelect = gameObject.GetComponent<Dropdown>();
        scnmngr = FindObjectOfType<Scene_Manager>();
        data = FindObjectOfType<OptionsSaverScript>();
    }

    public void UpdateLevelSelection()
    {
        levelSelection = lvlSelect.value;
        Debug.Log("Current Level Select Value: " + levelSelection);
    }

    public void LoadLevel()
    {
        if (levelSelection == 0)
        {
            data.playerValuesUsed = false;
            data.oldPVUsed = false;
            data.worldValuesUsed = false;
            scnmngr.LoadTut();
        }
        else if (levelSelection == 1)
        {
            data.playerValuesUsed = false;
            data.oldPVUsed = false;
            data.worldValuesUsed = false;
            scnmngr.LoadLvl1();
        }
        else if (levelSelection == 2)
        {
            data.playerValuesUsed = false;
            data.oldPVUsed = false;
            data.worldValuesUsed = false;
            scnmngr.LoadLvl2();
        }
        else if (levelSelection == 3)
        {
            data.playerValuesUsed = false;
            data.oldPVUsed = false;
            data.worldValuesUsed = false;
            scnmngr.LoadLvl3();
        }
        else if (levelSelection == 4)
        {
            data.playerValuesUsed = false;
            data.oldPVUsed = false;
            data.worldValuesUsed = false;
            scnmngr.LoadMetric();
        }
    }

}
