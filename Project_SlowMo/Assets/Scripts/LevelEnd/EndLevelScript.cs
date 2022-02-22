using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelScript : MonoBehaviour
{
    GameObject TriggerVolume = null;
    Game_UI Gameui = null;
    OptionsSaverScript data = null;

    public bool isCompletionCriteraMet = false;
    bool hasVolumeBeenToggled = false;

    // Start is called before the first frame update
    void Start()
    {
        Gameui = GameObject.Find("Game_UI").GetComponent<Game_UI>();
        TriggerVolume = GameObject.Find("Trigger Volume");
        data = FindObjectOfType<OptionsSaverScript>();
        TriggerVolume.SetActive(false);
        isCompletionCriteraMet = false;
        hasVolumeBeenToggled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCompletionCriteraMet && !hasVolumeBeenToggled)
            TriggerVolumeToggle();
    }

    private void TriggerVolumeToggle()
    {
        hasVolumeBeenToggled = true;
        TriggerVolume.SetActive(true);
    }

    public void EndLevelTriggered()
    {
        Gameui.isGameWin = true;
        Gameui.isGameOver = true;
    }
}
