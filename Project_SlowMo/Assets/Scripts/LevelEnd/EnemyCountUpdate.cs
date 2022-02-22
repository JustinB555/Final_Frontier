using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCountUpdate : MonoBehaviour
{
    public Text objectiveText;
    JButler_EnemyManager jem;
    // Start is called before the first frame update
    void Start()
    {
        jem = FindObjectOfType<JButler_EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        objectiveText.text = "Objective: Defeat All Enemies.\nEnemies Remaining: " + jem.NumberOfEnemies();
    }
}
