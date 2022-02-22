using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelBasic : MonoBehaviour
{
    Game_UI gui = null;
    OptionsSaverScript data = null;

    private void Start()
    {
        gui = FindObjectOfType<Game_UI>();
        data = FindObjectOfType<OptionsSaverScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gui.isGameWin = true;
            gui.isGameOver = true;
        }
    }

}
