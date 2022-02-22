using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorOpen : MonoBehaviour
{
    bool isInTrigger = false;

    [HideInInspector] public bool wasOpen = false;

    [SerializeField] bool enemyCheck;
    [SerializeField] bool puzzleCheck = false;

    bool objectiveComplete = false;

    bool enemyCheckPass = false;
    bool puzzleCheckPass = false;

    int passCount = 0;
    int puzzleCount = 0;

    [SerializeField] GameObject[] enemies;
    [SerializeField] JButler_PowerSource[] powerSources;

    [TextArea(5, 10)]
    [SerializeField] string objText;
    [SerializeField] float width = 620.0f;

    GameObject interactUI = null;
    InteractableManager iManager = null;
    Sound_Manager sm = null;
    DoorOpen_Manager drm = null;
    ObjTextSript ots = null;
    Pause_Menu pm = null;
    PlayerAnimatorScript plyrAnim = null;

    void Start()
    {
        interactUI = GameObject.Find("InteractUI");
        iManager = FindObjectOfType<InteractableManager>();
        sm = FindObjectOfType<Sound_Manager>();
        drm = FindObjectOfType<DoorOpen_Manager>();
        ots = FindObjectOfType<ObjTextSript>();
        pm = FindObjectOfType<Pause_Menu>();
        plyrAnim = FindObjectOfType<PlayerAnimatorScript>();

        if (!enemyCheck && !puzzleCheck)
        {
            GetComponent<Animator>().SetBool("Variable", true);
        }
    }

    void Update()
    {
        if (isInTrigger)
        {
            if (enemyCheck && !puzzleCheck)
            {
                if (enemyCheckPass)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        GetComponent<Animator>().SetTrigger("isOpen");
                        plyrAnim.PickUp();
                    }
                }
                else
                {
                    //Display Requirement
                }
            }
            else if (puzzleCheck && !enemyCheck)
            {
                if (puzzleCheckPass)
                    if (Input.GetKeyDown(KeyCode.E))
                        GetComponent<Animator>().SetTrigger("isOpen");
            }
            else if (puzzleCheck && enemyCheck)
            {
                if (puzzleCheckPass && enemyCheckPass)
                    if (Input.GetKeyDown(KeyCode.E))
                        GetComponent<Animator>().SetTrigger("isOpen");
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GetComponent<Animator>().SetTrigger("isOpen");
                }
            }
        }

        if (enemyCheckPass && !puzzleCheck)
        {
            GetComponent<Animator>().SetBool("Variable", true);

            if (!objectiveComplete)
            {
                ots.objectiveText.color = Color.green;
                objectiveComplete = true;
                sm.Play("ObjectiveComplete");
                pm.CompleteObjective(Color.green);
            }

        }
        else if (puzzleCheckPass && !enemyCheck)
        {
            GetComponent<Animator>().SetBool("Variable", true);

            if (!objectiveComplete)
            {
                ots.objectiveText.color = Color.green;
                objectiveComplete = true;
                sm.Play("ObjectiveComplete");
                pm.CompleteObjective(Color.green);
            }
        }
            
        else if (puzzleCheckPass && enemyCheckPass)
        {
            GetComponent<Animator>().SetBool("Variable", true);

            if (!objectiveComplete)
            {
                ots.objectiveText.color = Color.green;
                objectiveComplete = true;
                sm.Play("ObjectiveComplete");
                pm.CompleteObjective(Color.green);
            }
        }
            
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<JButler_Enemy>().IsDead())
            {
                passCount++;
            }
            else
            {
                //Check Failed...Reset Counter
                passCount = 0;
            }

            if (passCount >= enemies.Length)
            {
                enemyCheckPass = true;
            }
        }

        for (int i = 0; i < powerSources.Length; i++)
        {
            if (!powerSources[i].On())
                puzzleCount++;
            else
                puzzleCount = 0;

            if (puzzleCount >= powerSources.Length)
                puzzleCheckPass = true;
        }
    }

    public void NewWasOpen(bool value)
    {
        wasOpen = value;
    }

    public void CheckWasOpen()
    {
        if (wasOpen)
            GetComponent<Animator>().SetTrigger("wasOpen");
    }

    public bool WasOpen()
    {
        return wasOpen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;

            if (enemyCheck && !puzzleCheck)
            {
                if (enemyCheckPass)
                {
                    interactUI.transform.GetChild(0).gameObject.SetActive(true);
                    interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "Press 'E' to interact";
                    interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 620);
                    iManager.isPlayerInTriggerManager = true;
                    iManager.isUiCurrentlyOn = true;
                }
                else
                {
                    interactUI.transform.GetChild(0).gameObject.SetActive(true);
                    interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = objText;
                    interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    iManager.isPlayerInTriggerManager = true;
                    iManager.isUiCurrentlyOn = true;
                }

            }
            else if (puzzleCheck && !enemyCheck)
            {
                if (puzzleCheckPass)
                {
                    interactUI.transform.GetChild(0).gameObject.SetActive(true);
                    interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "Press 'E' to interact";
                    interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 620);
                    iManager.isPlayerInTriggerManager = true;
                    iManager.isUiCurrentlyOn = true;
                }
                else
                {
                    interactUI.transform.GetChild(0).gameObject.SetActive(true);
                    interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = objText;
                    interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    iManager.isPlayerInTriggerManager = true;
                    iManager.isUiCurrentlyOn = true;
                }
            }
            else if (puzzleCheck && enemyCheck)
            {
                if (puzzleCheckPass && enemyCheckPass)
                {
                    interactUI.transform.GetChild(0).gameObject.SetActive(true);
                    interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "Press 'E' to interact";
                    interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 620);
                    iManager.isPlayerInTriggerManager = true;
                    iManager.isUiCurrentlyOn = true;
                }
                else
                {
                    interactUI.transform.GetChild(0).gameObject.SetActive(true);
                    interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = objText;
                    interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    iManager.isPlayerInTriggerManager = true;
                    iManager.isUiCurrentlyOn = true;
                }
            }
            else
            {
                interactUI.transform.GetChild(0).gameObject.SetActive(true);
                iManager.isPlayerInTriggerManager = true;
                iManager.isUiCurrentlyOn = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;

            interactUI.transform.GetChild(0).gameObject.SetActive(false);
            interactUI.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "Press 'E' to interact";
            interactUI.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 620);
            iManager.isPlayerInTriggerManager = false;
            iManager.isUiCurrentlyOn = false;
        }
    }

    public void OpenSound()
    {
        sm.Play("DoorOpenNew");
    }

    public void WasClosed()
    {
        wasOpen = false;
    }

    public void WasOpened()
    {
        wasOpen = true;
    }

    public void StoreData()
    {
        drm.Store_Door_Data();
    }
}
