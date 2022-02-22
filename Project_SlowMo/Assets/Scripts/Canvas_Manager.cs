using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Canvas_Manager : MonoBehaviour
{
    [SerializeField]
    Button continueButton = null;
    [SerializeField]
    Button PlayButton = null;

    bool isContinueOn = false;
    string play = "PLAY";
    string newGame = "NEW GAME";

    OptionsSaverScript opsvrscr = null;

    // Start is called before the first frame update
    void Start()
    {
        opsvrscr = FindObjectOfType<OptionsSaverScript>();
        opsvrscr.Load();
        if (opsvrscr.sceneValue != 0)
        {
            isContinueOn = true;
        }

        if (isContinueOn)
        {
            continueButton.interactable = true;
            continueButton.GetComponent<ButtonLogic>().isValid = true;
            continueButton.onClick.AddListener(Continue);
            PlayButton.GetComponentInChildren<Text>().text = newGame;
        }
        else
        {
            continueButton.interactable = true;
            continueButton.GetComponent<ButtonLogic>().isValid = false;
            PlayButton.GetComponentInChildren<Text>().text = play;
        }
    }

    void Continue()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        opsvrscr.isContinue = true;
        opsvrscr.playerValuesUsed = true;
        opsvrscr.oldPVUsed = false;
        opsvrscr.worldValuesUsed = true;
        SceneManager.LoadScene(opsvrscr.sceneValue);
    }
}
