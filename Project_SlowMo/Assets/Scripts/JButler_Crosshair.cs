using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JButler_Crosshair : MonoBehaviour
{
    [ColorUsage(true, false)]
    [SerializeField] Color orange;
    [ColorUsage(true, false)]
    [SerializeField] Color grey;

    public void ColorWhite()
    {
        GetComponent<Image>().color = Color.white;
    }

    public void ColorRed()
    {
        GetComponent<Image>().color = Color.red;
    }

    public void ColorOrange()
    {
        GetComponent<Image>().color = orange;
    }

    public void ColorGrey()
    {
        GetComponent<Image>().color = grey;
    }

    public void ColorYellow()
    {
        GetComponent<Image>().color = Color.yellow;
    }

    public void NotActive()
    {
        gameObject.SetActive(false);
    }

    public void Active()
    {
        gameObject.SetActive(true);
    }


    
    public void BtChActivate()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector3(30, 30, 1);       
    }

    public void BtChDeactivate()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector3(15, 15, 1);
    }
}
