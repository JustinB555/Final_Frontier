using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shield : MonoBehaviour
{
    [SerializeField] 
    Image shieldReset;
    [SerializeField]
    GameObject ShieldUI = null;
    [SerializeField]
    GameObject shield;

    Sound_Manager sndmngr = null;
    Scene_Manager scnmngr = null;
    MovementThirdPerson m3p = null;

    bool isShieldActive = false;
    public bool isCoolDown = false;
    public bool isInTutorial = false;
    float waiting = 0f;
    float fillAmount = 1f;
    float cooldownTime = 10;
    int currScene = 0;


    // Start is called before the first frame update
    public void Start()
    {
        sndmngr = FindObjectOfType<Sound_Manager>();
        scnmngr = FindObjectOfType<Scene_Manager>();
        m3p = FindObjectOfType<MovementThirdPerson>();

        currScene = SceneManager.GetActiveScene().buildIndex;

        if (currScene != 3)
        {
            shieldReset = GameObject.Find("ShieldReset").GetComponent<Image>();
        }

        shield.SetActive(false);
        
    }

    // Update is called once per frame
    public void Update()
    {
        
        if (Time.time > waiting && Time.timeScale > 0)
        {
            if (shieldReset.fillAmount != 0)
            {
                shieldReset.fillAmount = 0f;
                sndmngr.Play("Shield_Recharge");
            }
            isCoolDown = false;
            if (Input.GetKeyDown(KeyCode.Alpha2) && !isShieldActive && ShieldUI.activeInHierarchy)
            {
                ShieldAbility();
            }
        }

        if (isCoolDown)
        {
            fillAmount = fillAmount - (0.1f * Time.deltaTime);
            shieldReset.fillAmount = fillAmount;
        }
    }



    private void ShieldAbility()
    {
        shield.SetActive(true);
        isShieldActive = true;
        sndmngr.Play("ShieldOpening");


        Invoke("RestartShieldAbility", 5f);
        
    }

    private void RestartShieldAbility()
    {
        sndmngr.Play("ShieldClosing");
        shield.SetActive(false);

        waiting = Time.time + cooldownTime;
        fillAmount = 1f;
        isCoolDown = true;
        isShieldActive = false;

        
    }
}
