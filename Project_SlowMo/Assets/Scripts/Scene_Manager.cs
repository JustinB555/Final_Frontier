//#define ORDER

using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public Scene currScene;
    public int sceneIndex;

    OptionsSaverScript opsvrscr = null;

    private void Start()
    {
        Time.timeScale = 1;
        currScene = SceneManager.GetActiveScene();
        sceneIndex = currScene.buildIndex;
        opsvrscr = FindObjectOfType<OptionsSaverScript>();
        Invoke("LoadingFromCheckpoint", 0.2f);
    }

    void LoadingFromCheckpoint()
    {
        if (opsvrscr.isContinue)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale = 1;
            }
            opsvrscr.Load();
            opsvrscr.playerValuesUsed = true;
            opsvrscr.oldPVUsed = false;
            opsvrscr.worldValuesUsed = true;
            #region DEBUGGING
#if ORDER
            Debug.LogError("<i><b><color=teal>LoadingFromCheckpoint</color></b></i>");
#endif
            #endregion
            SceneManager.LoadScene(opsvrscr.sceneValue);
        }
    }

    public void StartGame()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        opsvrscr.playerValuesUsed = false;
        opsvrscr.oldPVUsed = false;
        opsvrscr.worldValuesUsed = false;
        #region DEBUGGING
#if ORDER
        Debug.LogError("<i><b><color=teal>StartGame</color></b></i>");
#endif
        #endregion
        SceneManager.LoadScene("Tutorial");
    }

    public void ContinueGame()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        opsvrscr.isContinue = true;
        #region DEBUGGING
#if ORDER
        Debug.LogError("<i><b><color=teal>ContinueGame</color></b></i>");
#endif
        #endregion
        SceneManager.LoadScene(opsvrscr.sceneValue);
    }

    public void NextLevel()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        opsvrscr.playerValuesUsed = true;
        opsvrscr.oldPVUsed = false;
        opsvrscr.worldValuesUsed = false;
        #region DEBUGGING
#if ORDER
        Debug.LogError("<i><b><color=teal>NextLevel</color></b></i>");
#endif
        #endregion
        int nextLevelIndex = sceneIndex + 1;
        if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene("CreditsEnd");
        else
            SceneManager.LoadScene(nextLevelIndex);
    }

    public void MainMenu()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        #region DEBUGGING
#if ORDER
        Debug.LogError("<i><b><color=teal>MainMenu</color></b></i>");
#endif
        #endregion
        SceneManager.LoadScene(0);
    }

    public void Credits()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        #region DEBUGGING
#if ORDER
        Debug.LogError("<i><b><color=teal>Credits</color></b></i>");
#endif
        #endregion
        SceneManager.LoadScene("CreditsEnd");
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1;
        opsvrscr.playerValuesUsed = false;
        opsvrscr.oldPVUsed = true;
        opsvrscr.worldValuesUsed = false;
        #region DEBUGGING
#if ORDER
        Debug.LogError("<i><b><color=teal>RestartCurrentScene</color></b></i>");
#endif
        #endregion
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadMetric()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Metric");
    }
    public void LoadActionBlock()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("ActionBlock");
    }
    public void LoadProto1()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto1");
    }
    public void LoadProto2()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto2");
    }
    public void LoadProto3()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto3");
    }
    public void LoadProto4()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto4");
    }
    public void LoadProto5()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto5");
    }
    public void LoadProto6()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto6");
    }
    public void LoadProto7()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto7");
    }
    public void LoadProto8()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Proto8");
    }

    public void LoadTut()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadLvl1()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("level1");
    }

    public void LoadLvl2()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level_2");
    }
    public void LoadLvl3()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level3");
    }


    public void ApplicationQuit()
    {
        //save data info here
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
