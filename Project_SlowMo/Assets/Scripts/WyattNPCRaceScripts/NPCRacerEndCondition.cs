using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRacerEndCondition : MonoBehaviour
{
    bool isEnemyfinished = false;
    Game_UI gui = null;

    private void Start()
    {
        gui = FindObjectOfType<Game_UI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            isEnemyfinished = true;
        }
        if (other.CompareTag("Player"))
        {
            if (isEnemyfinished)
            {
                gui.isGameWin = false;
                gui.isGameOver = true;
            }
            else
            {
                gui.isGameWin = true;
                gui.isGameOver = true;
            }
        }
    }
}
