using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelRequirement : MonoBehaviour
{
    Game_UI gui = null;
    EndLevelScript els = null;
    OptionsSaverScript opsvrscr = null;

    [SerializeField] JButler_Enemy[] enemies = null;

    int amountDead = 0;

    // Start is called before the first frame update
    void Start()
    {
        gui = FindObjectOfType<Game_UI>();
        els = FindObjectOfType<EndLevelScript>();
        opsvrscr = FindObjectOfType<OptionsSaverScript>();
    }

    private void Update()
    {
        if (opsvrscr.enemyCount <= 0)
        {
            allEnemiesDead();
        }
    }

    void allEnemiesDead()
    {
        els.isCompletionCriteraMet = true;
    }
}
