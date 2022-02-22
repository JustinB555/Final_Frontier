using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    //[SerializeField] GameObject exampleButton = null;
    //[SerializeField] GameObject label = null;
    [SerializeField] ButtonManager btnmngr = null;
    public bool isActivated = false;
    public bool isValid = true;
    //public bool isNotAndActualButton = false;

    //string off = "OFF";
    //string on = "ON";
    private void Start()
    {
        btnmngr = FindObjectOfType<ButtonManager>();
        //if (isNotAndActualButton && isValid)
        //{
        //    gameObject.GetComponentInChildren<Text>().text = on;
        //}
        //else if (isNotAndActualButton && !isValid)
        //{
        //    gameObject.GetComponentInChildren<Text>().text = on;
        //}
        if (!isValid)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void ToggleValidity()
    {
        isValid = !isValid;
        //exampleButton.GetComponentInChildren<ButtonLogic>().isValid = isValid;
        //if (isValid)
        //{
        //    label.GetComponentInChildren<Text>().text = on;
        //}
        //else
        //{
        //    label.GetComponentInChildren<Text>().text = off;
        //}
    }
    public void PlayNoise()
    {
        isActivated = true;
        btnmngr.PlayNoise();
    }
}
