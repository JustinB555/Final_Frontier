using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeTutorial : MonoBehaviour
{
    [SerializeField] GameObject TutorialText = null;
    [SerializeField] GameObject Marker = null;

    MovementThirdPerson m3p;
    Collider tutCol;
    UITutorial uit = null;

    bool inGamePause = false;

    // Start is called before the first frame update
    void Start()
    {
        m3p = GameObject.FindObjectOfType<MovementThirdPerson>();
        tutCol = GetComponent<Collider>();
        uit = GameObject.FindObjectOfType<UITutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0 && inGamePause == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TutorialText.SetActive(false);
                tutCol.enabled = !tutCol.enabled;
                Time.timeScale = 1;
                Invoke("AcceptInput", 0.1f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialText.SetActive(true);
            Time.timeScale = 0;
            Marker.SetActive(true);
            m3p.isAcceptingInput = false;
            inGamePause = true;
        }

    }

    void AcceptInput()
    {
        m3p.isAcceptingInput = true;
    }

}
