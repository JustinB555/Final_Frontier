using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_ScoreManager : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("UI")]
    [Tooltip("Put Game_UI here.")]
    public Game_UI ui = null;
    [Tooltip("This is the total score in this scene.")]
    public int score = 0;

    // private


    // Start is called before the first frame update
    void Start()
    {
        if (ui == null)
            throw new System.Exception(name + " <b><color=red>ui is missing the Game_UI reference!</color></b>\tMake sure to add the Game_UI as a reference.");

        if (!ui.ScoreUI.activeInHierarchy)
        {
            //ui.ScoreUI.SetActive(true);
            //ui.scoreText.text = "0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
