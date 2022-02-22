using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjTextSript : MonoBehaviour
{
    public Image backgroundImage;

    public Text objectiveText;

    public GameObject Header = null;

    string theObjective;

    float horizontalSize = 61.5f;
    float verticalSize = 64.5f;

    bool whiteOnce = false;
    private void Update()
    {
        if (string.IsNullOrEmpty(objectiveText.text)) //going down
        {
            if(horizontalSize > 61.5f)
            {
                horizontalSize -= 450 * Time.deltaTime;

                backgroundImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalSize);
            }

            if(verticalSize > 64.5)
            {
                verticalSize -= 200 * Time.deltaTime;

                backgroundImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalSize);
            }

            Header.SetActive(false);
            objectiveText.color = Color.clear;
            whiteOnce = false;
        }
        else //going up
        {
            if(horizontalSize < 375)
            {
                horizontalSize += 450 * Time.deltaTime;

                backgroundImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalSize);
            }

            if(verticalSize < 200)
            {
                verticalSize += 200 * Time.deltaTime;

                backgroundImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalSize);
            }

            if(horizontalSize >= 375)
            {
                if (!whiteOnce)
                {
                    Header.SetActive(true);
                    objectiveText.color = Color.white;
                    whiteOnce = true;
                }
            }
        }
    }

    /*public void InsertText(string text)
    {
        Invoke("PlaceText", 1.5f);

        theObjective = text;
    }

    void PlaceText()
    {
        objectiveText.text = theObjective;
    }*/
}
