using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] ButtonLogic[] buttons = null;
    [SerializeField] Sound_Manager sndmngr = null;

    private void Start()
    {
        sndmngr = FindObjectOfType<Sound_Manager>();
    }

    public void PlayNoise()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].isActivated && buttons[i].isValid)
            {
                sndmngr.Play("Button Confirm");
                buttons[i].isActivated = false;
            }
            else if (buttons[i].isActivated && !buttons[i].isValid)
            {
                sndmngr.Play("Fail");
                buttons[i].isActivated = false;
            }
        }
    }
}
