using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceRelease : MonoBehaviour
{
    [SerializeField] TextMesh text = null;
    [SerializeField] Animator npcRacer = null;

    string start = "Please come closer to start the race";

    bool playerHere = false;

    private float waitTime1 = 1f;
    private float waitTime2 = 2f;
    private float waitTime3 = 3f;
    private float waitTime4 = 4f;
    float currentTimerText = 3;
    private float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        text.text = start;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHere = true;
        }
    }

    private void Update()
    {
        if (playerHere)
        {
            timer += Time.deltaTime;

            if (timer < waitTime1)
            {
                text.text = currentTimerText.ToString();
            }

            if (timer > waitTime1 && timer < waitTime2)
            {
                currentTimerText = 2;
                text.text = currentTimerText.ToString();
            }

            if (timer > waitTime2 && timer < waitTime3)
            {
                currentTimerText = 1;
                text.text = currentTimerText.ToString();
            }

            if (timer > waitTime3 && timer < waitTime4)
            {
                currentTimerText = 0;
                text.text = currentTimerText.ToString();
            }

            if (timer > waitTime4)
            {
                playerHere = false;
                gameObject.GetComponent<Animator>().SetBool("PlayerInTrigger", true);
                npcRacer.SetTrigger("RaceStart");
                Invoke("DDay", 1);
            }
        }
    }

    void DDay()
    {
        Destroy(gameObject);
    }
}
