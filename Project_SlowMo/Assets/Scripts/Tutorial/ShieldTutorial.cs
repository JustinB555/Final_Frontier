using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTutorial : MonoBehaviour
{
    [SerializeField] GameObject TutorialText = null;
    
    
    Collider tutCol;
    MovementThirdPerson m3p = null;
    Pause_Menu pm = null;
    UITutorial uit = null;
    Shield shield = null;

    bool inGamePause = false;

    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.Find("PAUSE").GetComponent<Pause_Menu>();
        tutCol = GetComponent<Collider>();
        m3p = GameObject.FindObjectOfType<MovementThirdPerson>();
        uit = GameObject.FindObjectOfType<UITutorial>();
        shield = GameObject.FindObjectOfType<Shield>();

    }

    // Update is called once per frame
    void Update()
    {
        if (inGamePause == true && pm.isGamePaused == false && Input.GetKeyDown(KeyCode.Space))
        {
            tutCol.enabled = !tutCol.enabled;
            uit.ShieldAbilityUI.SetActive(true);
            shield.isInTutorial = false;
            TutorialText.SetActive(false);
            Time.timeScale = 1.0f;
            Invoke("AcceptInput", 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialText.SetActive(true);
            Time.timeScale = 0.0f;
            inGamePause = true;
            m3p.isAcceptingInput = false;
        }
    }
    void AcceptInput()
    {
        m3p.isAcceptingInput = true;
        Destroy(gameObject);
    }
}
