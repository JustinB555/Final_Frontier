using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Menu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu = null;

    public bool isGamePaused = false;
    float storedTimescale = 0.0f;

    Options_Menu optMnu = null;
    JButler_CursorManager cursorMngr = null;
    Sound_Manager sndmngr = null;
    Game_UI gui = null;

    [Header("ObjectiveSlots")]

    public Text objectiveSlot_1;
    public Text objectiveSlot_2;
    public Text objectiveSlot_3;
    public Text objectiveSlot_4;
    public Text objectiveSlot_5;
    public Text objectiveSlot_6;
    public Text objectiveSlot_7;
    public Text objectiveSlot_8;

    int lastSet = 0;

    void Start()
    {
        isGamePaused = false;
        pauseMenu.SetActive(false);
        optMnu = GameObject.Find("OPTIONS").GetComponent<Options_Menu>();
        cursorMngr = GameObject.Find("Cursor Manager").GetComponent<JButler_CursorManager>();
        sndmngr = FindObjectOfType<Sound_Manager>();
        gui = FindObjectOfType<Game_UI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !optMnu.isOptionOpen && !gui.isUiRevealed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            sndmngr.Play("MenuOpen");
            storedTimescale = Time.timeScale;
            pauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            cursorMngr.ShowCursor();
            cursorMngr.UnlockCursor();
            //Debug.Log("Current Stored Timescale = " + storedTimescale);
        }
        else
        {
            sndmngr.Play("MenuClose");
            pauseMenu.SetActive(false);
            Time.timeScale = storedTimescale;
            cursorMngr.LockCursor();
            cursorMngr.HideCursor();
        }
    }

    public void NewObjective(string objectiveText)
    {
        if (string.IsNullOrEmpty(objectiveSlot_1.text))
        {
            objectiveSlot_1.text = "Objective 01: \n" + objectiveText;
            lastSet = 1;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_2.text))
        {
            objectiveSlot_2.text = "Objective 02: \n" + objectiveText;
            lastSet = 2;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_3.text))
        {
            objectiveSlot_3.text = "Objective 03: \n" + objectiveText;
            lastSet = 3;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_4.text))
        {
            objectiveSlot_4.text = "Objective 04: \n" + objectiveText;
            lastSet = 4;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_5.text))
        {
            objectiveSlot_5.text = "Objective 05: \n" + objectiveText;
            lastSet = 5;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_6.text))
        {
            objectiveSlot_6.text = "Objective 06: \n" + objectiveText;
            lastSet = 6;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_7.text))
        {
            objectiveSlot_7.text = "Objective 07: \n" + objectiveText;
            lastSet = 7;
        }
        else if (string.IsNullOrEmpty(objectiveSlot_8.text))
        {
            objectiveSlot_8.text = "Objective 08: \n" + objectiveText;
            lastSet = 8;
        }
    }

    public void CompleteObjective(Color objColor)
    {
        if(lastSet == 1)
        {
            objectiveSlot_1.color = objColor;
        }
        else if(lastSet == 2)
        {
            objectiveSlot_2.color = objColor;
        }
        else if (lastSet == 3)
        {
            objectiveSlot_3.color = objColor;
        }
        else if (lastSet == 4)
        {
            objectiveSlot_4.color = objColor;
        }
        else if (lastSet == 5)
        {
            objectiveSlot_5.color = objColor;
        }
        else if (lastSet == 6)
        {
            objectiveSlot_6.color = objColor;
        }
        else if (lastSet == 7)
        {
            objectiveSlot_7.color = objColor;
        }
        else if (lastSet == 8)
        {
            objectiveSlot_8.color = objColor;
        }
    }
}
