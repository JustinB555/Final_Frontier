using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_UI : MonoBehaviour
{

    JButler_CursorManager curserMngr = null;
    Sound_Manager sndmngr = null;
    Scene_Manager scnmngr = null;
    OptionsSaverScript opsvrscr = null;

    public Slider HSlider = null;
    public Slider BtSlider = null;
    public GameObject CrouchIcon = null;
    public GameObject EndConditionUI = null;
    public Text EndText = null;
    public Text iconText = null;
    public Text standText = null;
    public Text painKillerText = null;
    public Text btPillText = null;
    public Text ammoMag = null;
    public Text ammoRes = null;
    public Text grenadeText = null;
    public GameObject ScoreUI = null;
    public Text scoreText = null;
    public GameObject ammoNotif = null;
    public Text ammoNotifText = null;
    public GameObject LH_Border = null;
    public Image lh;
    public Image injury;
    public GameObject injuryGameobject = null;
    public Image noGrenade;
    public Text noGrenadeText;

    public GameObject interact = null;
    public GameObject pickupImage = null;
    public Text pickupAmmoValue = null;

    string LevelWinStatement = "LEVEL WON!";
    string GameWinStatement = "GAME WON!";
    string LoseStatement = "YOU LOSE :(";
    string tutorialNLB = "ADMIN";
    string level1NLB = "COMMS";
    string level2NLB = "AGRICULTURE";
    string level3NLB = "END CREDITS";

    [SerializeField]
    GameObject nextLevelButton = null;
    [SerializeField]
    GameObject lastCheckpointButton = null;
    [SerializeField]
    JButler_Shooting shooting = null;
    [SerializeField]
    GameObject nextLocationText = null;

    public bool isGameWin = false;
    public bool isGameOver = false;
    public bool isUiRevealed = false;
    public bool tryFailStand = false;
    bool ammoNotifFade = false;

    float blink = 0f;
    int amountOfTimesBlinked = 0;
    bool turningRed = false;
    bool isBlinking = false;
    bool isMag = false;
    bool lhRising = true;
    bool injuryRising = true;

    Color blackColor = Color.black;
    Color tranColor = Color.clear;

    float colorSpeed = 0.01f;
    float colorSet = 0;
    float alphaValue = 0;
    float lhTran = 0;
    float injuryTran = 0;

    public int paki = 0;
    public int baki = 0;

    int currScene = 0;
    void Start()
    {
        EndConditionUI.SetActive(false);
        curserMngr = GameObject.Find("Cursor Manager").GetComponent<JButler_CursorManager>();
        sndmngr = FindObjectOfType<Sound_Manager>();
        scnmngr = FindObjectOfType<Scene_Manager>();
        opsvrscr = FindObjectOfType<OptionsSaverScript>();

        isUiRevealed = false;
        pickupImage.SetActive(false);
        lastCheckpointButton.SetActive(false);

        currScene = SceneManager.GetActiveScene().buildIndex;

        Invoke("UpdateAmmoText", 0.2f);
    }


    void Update()
    {
        if (isGameOver && !isUiRevealed)
        {
            EndConditionReveal();
        }

        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    UpdateAmmoText();
        //}
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateAmmoText();
            Invoke("UpdateAmmoText", 0.2f);
        }
        //if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
        //{
        //    UpdateAmmoText();
        //}

        painKillerText.text = paki.ToString();
        paki = Mathf.Clamp(paki, 0, 5);

        btPillText.text = baki.ToString();
        baki = Mathf.Clamp(baki, 0, 5);
    }

    public void UpdateAmmoText()
    {
        ammoMag.text = shooting.CurrentAmmo();
        ammoRes.text = shooting.SpareAmmo();
    }

    private void FixedUpdate()
    {
        if (tryFailStand)
        {
            Color textColor;
            textColor = Color.Lerp(blackColor, tranColor, colorSet);
            colorSet += colorSpeed;

            standText.color = textColor;

            if (colorSet >= 1)
            {
                colorSet = 0;
                tryFailStand = false;
            }
        }



        if (ammoNotifFade && Time.timeScale > 0)
        {
            alphaValue -= Time.deltaTime;
            //Debug.Log("Current Alpha Value = " + alphaValue);

            ammoNotif.GetComponent<Image>().color = new Color(0, 0, 0, alphaValue);
            ammoNotifText.GetComponent<Text>().color = new Color(1, 1, 1, alphaValue);

            if (alphaValue <= 0)
            {
                ammoNotif.SetActive(false);
                ammoNotifFade = false;
            }
        }

        if (isBlinking && Time.timeScale > 0)
        {
            //Thanks Jacob for this bit <3
            if (blink < 1 && turningRed)
            {
                blink += 0.04f;
            }
            else
            {
                turningRed = false;
                if (blink > 1)
                {
                    amountOfTimesBlinked += 1;
                }
            }
            if (blink > 0 && !turningRed)
            {
                blink -= 0.04f;
            }
            else
            {
                turningRed = true;
                if (amountOfTimesBlinked > 1)
                {
                    isBlinking = false;
                    amountOfTimesBlinked = 0;
                }
            }

            if (isMag)
            {
                ammoMag.color = Color.Lerp(Color.white, Color.red, blink);
            }
            else
            {
                ammoMag.color = Color.Lerp(Color.white, Color.red, blink);
                ammoRes.color = Color.Lerp(Color.white, Color.red, blink);
            }
        }

        
        if (LH_Border.activeInHierarchy == true)
        {
            lh.color = new Color(1, 1, 1, lhTran);
            if (lhRising)
            {
                lhTran += 0.01f;
                if(lhTran >= 1)
                {
                    lhRising = false;
                }
            }
            else
            {
                lhTran -= 0.01f;
                if (lhTran <= 0.25f)
                {
                    lhRising = true;
                }
            }
        }
        else
        {
            lhTran = 0;
        }

        if(injuryGameobject.activeInHierarchy == true)
        {
            injury.color = new Color(1, 1, 1, injuryTran);
            if (injuryRising)
            {
                injuryTran += 0.15f;
                if(injuryTran >= 1)
                {
                    injuryRising = false;
                }
            }
            else
            {
                injuryTran -= 0.05f;
                if(injuryTran <= 0)
                {
                    injuryGameobject.SetActive(false);
                    injuryRising = true;
                    injuryTran = 0;
                }
            }
        }
    }

    public void SetMaxHealth(int health)
    {
        HSlider.maxValue = health;
        HSlider.value = health;
    }
    public void SetHealth(int health)
    {
        HSlider.value = health;
    }
    public void SetMaxBtAmount(float BtAmount)
    {
        BtSlider.maxValue = BtAmount;
        BtSlider.value = BtAmount;
    }
    public void SetBtAmount(float BtAmount)
    {
        BtSlider.value = BtAmount;
    }
    private void EndConditionReveal()
    {
        isUiRevealed = true;
        EndConditionUI.SetActive(true);
        curserMngr.ShowCursor();
        curserMngr.UnlockCursor();
        if (isGameWin && currScene == 3)
        {
            sndmngr.Play("LevelWin");
            EndText.color = new Color(0f, 1f, 0f);
            EndText.text = LevelWinStatement;
            nextLevelButton.GetComponentInChildren<Text>().text = tutorialNLB;
        }
        if (isGameWin && currScene == 4)
        {
            sndmngr.Play("LevelWin");
            EndText.color = new Color(0f, 1f, 0f);
            EndText.text = LevelWinStatement;
            nextLevelButton.GetComponentInChildren<Text>().text = level1NLB;
        }
        if (isGameWin && currScene == 5)
        {
            sndmngr.Play("LevelWin");
            EndText.color = new Color(0f, 1f, 0f);
            EndText.text = LevelWinStatement;
            nextLevelButton.GetComponentInChildren<Text>().text = level2NLB;
        }
        else if (isGameWin && currScene >= SceneManager.sceneCountInBuildSettings - 1)
        {
            sndmngr.Play("LevelWin");
            EndText.color = new Color(0f, 1f, 0f);
            EndText.text = GameWinStatement;
            nextLevelButton.GetComponentInChildren<Text>().text = level3NLB;
        }
        else if (!isGameWin)
        {
            nextLevelButton.SetActive(false);
            lastCheckpointButton.SetActive(true);
            sndmngr.Play("LevelLost");
            EndText.color = new Color(1f, 0f, 0f);
            EndText.text = LoseStatement;
            nextLocationText.SetActive(false);
        }
        Time.timeScale = 0f;
    }

    public void PickupText(int value)
    {
        pickupAmmoValue.text = value.ToString();
        pickupImage.SetActive(true);
        Invoke("PickupTextDisable", 0.75f);
    }
    private void PickupTextDisable()
    {
        pickupImage.SetActive(false);
    }
    public void ContinueFromCheckpoint()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        opsvrscr.isContinue = true;
        opsvrscr.playerValuesUsed = true;
        opsvrscr.oldPVUsed = false;
        opsvrscr.worldValuesUsed = true;
        SceneManager.LoadScene(0);
    }

    public void EnableAmmoNotif(string reloadOrLowAmmo)
    {
        ammoNotif.SetActive(true);
        ammoNotif.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        ammoNotifText.GetComponent<Text>().color = new Color(1, 1, 1, 0.5f);
        if (reloadOrLowAmmo == "Reload")
        {
            ammoNotifText.text = "Press 'R' to Reload";
            isMag = true;
            isBlinking = true;
        }
        else if (reloadOrLowAmmo == "LowAmmo")
        {
            ammoNotifText.text = "No Ammo, Find More";
            isMag = false;
            isBlinking = true;
        }
        Invoke("DisableAmmoNotif", 1.5f);
    }

    void DisableAmmoNotif()
    {
        alphaValue = 0.5f;
        ammoNotifFade = true;
    }
}
