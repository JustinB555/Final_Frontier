using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTutorial : MonoBehaviour
{
    [SerializeField] GameObject TutorialText = null;

    Game_UI gui;
    MovementThirdPerson m3p = null;
    Pause_Menu pm = null;
    UITutorial uit = null;

    bool inGamePause = false;

    // Start is called before the first frame update
    void Start()
    {
        gui = FindObjectOfType<Game_UI>();
        m3p = GameObject.FindObjectOfType<MovementThirdPerson>();
        pm = GameObject.Find("PAUSE").GetComponent<Pause_Menu>();
        uit = GameObject.FindObjectOfType<UITutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gui.paki > 0)
        {
            uit.PainkillerUI.SetActive(true);
            uit.HealthUI.SetActive(true);
            TutorialText.SetActive(true);
            Time.timeScale = 0.0f;
            inGamePause = true;
            m3p.isAcceptingInput = false;
        }

        if (inGamePause == true && pm.isGamePaused == false && Input.GetKeyDown(KeyCode.Space))
        {
            TutorialText.SetActive(false);
            Time.timeScale = 1.0f;
            Invoke("AcceptInput", 0.001f);
        }
    }

    void AcceptInput()
    {
        m3p.isAcceptingInput = true;
        gameObject.SetActive(false);
    }
}
