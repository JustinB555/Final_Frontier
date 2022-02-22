using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reminder : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Text reminderText;

    [TextArea(5, 10)]
    [SerializeField] string reminder = null;

    int textBoxSize = 0;
    bool isInTrigger = false;
    void Start()
    {

    }

    void Update()
    {
        if (isInTrigger) //up
        {
            
            if(textBoxSize < 500)
            {
                textBoxSize += 25;

                background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textBoxSize);

                if(textBoxSize > 450)
                {
                    reminderText.text = "Reminder: " + reminder;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
            //Debug.Log("Yep");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
            InvokeRepeating("Exit", 0.01f, Time.deltaTime);
            //Debug.Log("Nope");
        }
    }

    void Exit()
    {
        if (textBoxSize > 0)
        {
            textBoxSize -= 20;

            background.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textBoxSize);

            if (textBoxSize < 700)
            {
                reminderText.text = "";
            }

            if(textBoxSize <= 0)
            {
                CancelInvoke();
            }
        }
    }
}
