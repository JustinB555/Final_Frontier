using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AOEAbility : MonoBehaviour
{
    [SerializeField] GameObject aoeCollider = null;
    [SerializeField] AOECollider ac = null;
    [SerializeField] Image aoeCooldownMask;
    [SerializeField] GameObject AOEIcon = null;
    
    MovementThirdPerson mvmt3p = null;
    Sound_Manager sndmngr = null;
    Scene_Manager scnmngr = null;

    bool isAoeActive = false;
    public bool isInCooldown = false;
    float cooldownTime = 10;
    float nextFireTime = 0;
    float fillAmount = 1;
    int currScene = 0;

    Renderer colliderRenderer = null;

    public Scene Tutorial { get; private set; }

    private void Start()
    {
        mvmt3p = FindObjectOfType<MovementThirdPerson>();
        sndmngr = FindObjectOfType<Sound_Manager>();
        scnmngr = FindObjectOfType<Scene_Manager>();

        currScene = SceneManager.GetActiveScene().buildIndex;

        if (currScene != 3)
        {
            aoeCooldownMask = GameObject.Find("AOECooldownMask").GetComponent<Image>();
        }
        

        aoeCollider.SetActive(false);
    }
    private void Update()
    {
        if (Time.time > nextFireTime && Time.timeScale > 0)
        {
            if (aoeCooldownMask.fillAmount != 0)
            {
                aoeCooldownMask.fillAmount = 0f;
                sndmngr.Play("AOERecharge");
            }
            isInCooldown = false;
            if (Input.GetKeyDown(KeyCode.Alpha1) && !isAoeActive && !mvmt3p.airTime && AOEIcon.activeInHierarchy)
            {
                AOEFunction();
            }
        }

        if (isInCooldown)
        {
            fillAmount = fillAmount - (0.1f * Time.deltaTime);
            aoeCooldownMask.fillAmount = fillAmount;
        }
    }

    public void EpicDamage()
    {
        //a foreach/for loop leading to a list in AOECollider to damage those specific enemies
        foreach (GameObject go in ac.enemiesInTrigger)
        {
            go.GetComponent<JButler_Enemy>().TakeDamage(100);
        }
    }

    private void AOEFunction()
    {
        aoeCollider.SetActive(true);
        isAoeActive = true;
        mvmt3p.isAcceptingInput = false;
        mvmt3p.direction = new Vector3(0, 0, 0);
        sndmngr.Play("AOEAction");

        Invoke("LeavingAOE", 1.5f);
    }

    private void LeavingAOE()
    {
        ac.enemiesInTrigger.Clear();
        aoeCollider.SetActive(false);
        mvmt3p.isAcceptingInput = true;

        nextFireTime = Time.time + cooldownTime;
        aoeCooldownMask.fillAmount = 1f;
        fillAmount = 1f;
        isInCooldown = true;

        isAoeActive = false;
    }
}
