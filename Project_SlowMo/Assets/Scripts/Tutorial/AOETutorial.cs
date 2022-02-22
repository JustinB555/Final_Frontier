using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOETutorial : MonoBehaviour
{
    [SerializeField] GameObject AOEText = null;

    Pause_Menu pm = null;
    Collider tutCol;
    UITutorial uit = null;

    bool inGamePause = false;

    // Start is called before the first frame update
    void Start()
    {
        tutCol = GetComponent<Collider>();
        pm = GameObject.Find("PAUSE").GetComponent<Pause_Menu>();
        uit = GameObject.FindObjectOfType<UITutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inGamePause == true && pm.isGamePaused == false && Input.GetKeyDown(KeyCode.Alpha1))
        {
            AOEText.SetActive(false);
            uit.AOEAbilityUI.SetActive(true);
            Time.timeScale = 1.0f;
            inGamePause = false;
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        AOEText.SetActive(true);
        Time.timeScale = 0.0f;
        inGamePause = true;
        
    }

    void Textoff()
    {
        AOEText.SetActive(false);
    }
}
