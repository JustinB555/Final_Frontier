using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoTutorial : MonoBehaviour
{

    [SerializeField] GameObject TutorialText = null;
    [SerializeField] GameObject Marker = null;
    [SerializeField] GameObject HandGun = null;
    [SerializeField] GameObject NextTutorial = null;
    [SerializeField] GameObject NextTutorial1 = null;


    MovementThirdPerson m3p = null;
    Pause_Menu pm = null;
    UITutorial uit = null;
    

    Collider tutCol;

    bool inGamePause = false;

    private void Start()
    {
        m3p = GameObject.FindObjectOfType<MovementThirdPerson>();
        pm = GameObject.Find("PAUSE").GetComponent<Pause_Menu>();
        tutCol = GetComponent<Collider>();
        uit = GameObject.FindObjectOfType<UITutorial>();
    }


    void Update()
    {
        if (!HandGun.activeInHierarchy)
        {
            uit.WeaponUI.SetActive(true);
            Marker.SetActive(false);
            NextTutorial.SetActive(true);
            NextTutorial1.SetActive(true);
            Destroy(gameObject);          
        }
        


        if (Time.timeScale == 0 && inGamePause == true && pm.isGamePaused == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tutCol.enabled = !tutCol.enabled;
                TutorialText.SetActive(false);
                Time.timeScale = 1.0f;
                Invoke("AcceptInput", 0.1f);
            }
        }

        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Marker.SetActive(true);
            TutorialText.SetActive(true);
            m3p.isAcceptingInput = false;
            inGamePause = true;
            Time.timeScale = 0.0f;
        }

    }

    void AcceptInput()
    {
        m3p.isAcceptingInput = true;
    }

}
