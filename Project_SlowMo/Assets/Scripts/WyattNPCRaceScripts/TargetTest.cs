using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTest : MonoBehaviour
{
    [SerializeField] TextMesh text = null;
    Player_Values pv = null;
    JButler_Shooting shooty = null;

    string start = "Step into the blue to start target testing";

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
        pv = FindObjectOfType<Player_Values>();
        shooty = FindObjectOfType<JButler_Shooting>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHere = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pv.currBt < 50)
            {
                pv.currBt = pv.currBt + (pv.maxBt - pv.currBt);
            }

            if (shooty.CurAmmo() == 0 && shooty.SpareAmmo() == "0")
            {
                shooty.AddAmmo(15);
            }
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
                gameObject.GetComponent<Animator>().SetTrigger("TestStart");
                text.text = "";
                gameObject.GetComponent<TargetTest>().enabled = false;
            }
        }
    }
}
