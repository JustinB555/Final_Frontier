using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options_Menu : MonoBehaviour
{
    [SerializeField]
    GameObject optionsMenu = null;
    [SerializeField]
    GameObject otherCanvas = null;

    public bool isOptionOpen = false;
    public bool isControlsOpen = false;

    [SerializeField]
    Slider musicSlider = null;
    [SerializeField]
    Slider SFXSlider = null;
    [SerializeField]
    GameObject lowTextureImage = null;
    [SerializeField]
    GameObject highTextureImage = null;
    [SerializeField]
    GameObject restartLevelText = null;

    public float musicValue = 0f;
    public float sfxValue = 0f;

    public int qsNumber = 0;

    [SerializeField]
    Sound_Manager sndmngr = null;
    [SerializeField]
    GameObject controls = null;

    OptionsSaverScript opSavScript = null;

    // Start is called before the first frame update
    void Start()
    {
        isOptionOpen = false;
        isControlsOpen = false;
        optionsMenu.SetActive(false);
        controls.SetActive(false);
        restartLevelText.SetActive(false);


        musicSlider.value = sndmngr.audioSources[0].volume;
        SFXSlider.value = sndmngr.audioSources[2].volume;

        // Attempting to reach the OptionsSaverScript. This is what makes your life a living hell during devleopment.
        try
        {
            opSavScript = GameObject.Find("OptionsSaver").GetComponent<OptionsSaverScript>();

            SettingValuesFromOptionsSaver();
        }
        catch (System.NullReferenceException e)
        {
            throw new System.NullReferenceException(name + " Options_Menu couldn't find the <color=red>OptionsSaverScript</color>. Recommend action is to start the project from the <color=lime>Main Menu</color>.\n" + e.Message + "\nBruh");
        }


        //displaying the highlighted selection upon opening the options
        if (qsNumber == 0)
        {
            lowTextureImage.SetActive(true);
            highTextureImage.SetActive(false);
        }
        else
        {
            lowTextureImage.SetActive(false);
            highTextureImage.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            QualitySettingsShow();
        }
    }
    public void QualitySettingsShow()
    {
        string[] names;
        names = QualitySettings.names;
        Debug.LogError("Current quality settings is set to: " + names[QualitySettings.GetQualityLevel()]);
    }
    public void ToggleOptions()
    {
        isOptionOpen = !isOptionOpen;

        if (isOptionOpen)
        {
            optionsMenu.SetActive(true);
            otherCanvas.SetActive(false);
        }
        else
        {
            otherCanvas.SetActive(true);
            optionsMenu.SetActive(false);
        }

    }

    public void MusicChange()
    {

        musicValue = musicSlider.value;

        try
        {
            int howmanysongs = 2;
            int howmanysfx = sndmngr.audioSources.Length;
            int audiodifference = howmanysfx - howmanysongs;
            for (int i = 0; i < sndmngr.audioSources.Length - audiodifference; i++)
            {
                sndmngr.audioSources[i].volume = musicSlider.value;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            return;
        }
        catch (System.NullReferenceException)
        {
            return;
        }
        //sndmngr.audioSources[0].volume = musicSlider.value;
        //sndmngr.audioSources[1].volume = musicSlider.value;
    }

    public void SFXChange()
    {
        sfxValue = SFXSlider.value;

        for (int i = 2; i < sndmngr.audioSources.Length; i++)
        {
            sndmngr.audioSources[i].volume = SFXSlider.value;
        }
    }

    public void SaveOptions()
    {
        opSavScript.TheOlSwitcharoo();
        opSavScript.SaveAudio();
    }
    public void SettingValuesFromOptionsSaver()
    {
        opSavScript.LoadAudio();
        musicSlider.value = opSavScript.musicStore;
        SFXSlider.value = opSavScript.sfxStore;
        qsNumber = opSavScript.qualitySetting;
        MusicChange();
        SFXChange();
        SetQualityLevel();
        //Debug.Log("I Set the options from OPtions Saver");
    }

    void SetQualityLevel()
    {
        qsNumber = opSavScript.qualitySetting;
        QualitySettings.SetQualityLevel(qsNumber);
    }

    public void ToggleControls()
    {
        isControlsOpen = !isControlsOpen;
        if (isControlsOpen)
        {
            optionsMenu.SetActive(false);
            controls.SetActive(true);
        }
        else
        {
            optionsMenu.SetActive(true);
            controls.SetActive(false);
        }
    }

    public void ChangeQualityValue(int settingNumber)
    {

        restartLevelText.SetActive(true);
        if (settingNumber == 0)
        {
            qsNumber = settingNumber;
        }
        else
        {
            qsNumber = settingNumber;
        }

        if (qsNumber == 0)
        {
            lowTextureImage.SetActive(true);
            highTextureImage.SetActive(false);
        }
        else
        {
            lowTextureImage.SetActive(false);
            highTextureImage.SetActive(true);
        }
    }
}
