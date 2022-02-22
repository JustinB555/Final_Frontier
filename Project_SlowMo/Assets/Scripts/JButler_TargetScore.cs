using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JButler_TargetScore : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Space(5)]
    [Header("Our Targets")]
    [Tooltip("Keep track of which targets this board is going to be tracking")]
    [SerializeField] JButler_Target[] targets = null;

    [Space(5)]
    [Header("Scoreboard UI")]
    [Tooltip("Put the text for the scoreboard here.")]
    [SerializeField] TextMesh scoreboard = null;
    [Tooltip("Put this test's #.")]
    [SerializeField] int testNum = 0;

    // private
    List<int> scores = new List<int>();
    JButler_ScoreManager sm = null;

    // Start is called before the first frame update
    void Start()
    {
        sm = FindObjectOfType<JButler_ScoreManager>();

        if (scoreboard == null)
            throw new System.Exception(name + " <b><color=red>Scoreboard is missing the Text reference!</color></b>\tMake sure to add the Text from Blackboard as a reference.");
        if (sm == null)
            throw new System.Exception(name + "<b><color=red>could not find Sm!</color></b>\tMake sure that there is a Score Mangager inside the scene.");

        Scoreboard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////

    public void AddScore()
    {
        foreach (JButler_Target tar in targets)
        {
            if (!tar.checkedHit && (tar.point1 || tar.point2 || tar.point3 || tar.point4 || tar.point5))
            {
                sm.score += tar.PointsValue();
                sm.ui.scoreText.text = sm.score.ToString();
                scores.Add(tar.PointsValue());
                Scoreboard();
                tar.checkedHit = true;
            }
        }
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void Scoreboard()
    {
        // no count
        if (scores.Count == 0)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + "\n\n- Total Score: " + sm.score.ToString());
        // start
        else if (scores.Count == 1)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + " = " + scores[0] + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 2)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + " = " + (scores[0] + scores[1]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 3)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + " = " + (scores[0] + scores[1] + scores[2]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 4)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + " = " + (scores[0] + scores[1] + scores[2] + scores[3]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 5)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 6)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5]) + "\n\n- Total Score: " + sm.score.ToString());
        // new line
        else if (scores.Count == 7)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 8)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 9)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 10)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 11)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 12)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11]) + "\n\n- Total Score: " + sm.score.ToString());
        // new line
        else if (scores.Count == 13)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + "+\n" + scores[12] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11] + scores[12]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 14)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + "+\n" + scores[12] + "+" + scores[13] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11] + scores[12] + scores[13]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 15)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + "+\n" + scores[12] + "+" + scores[13] + "+" + scores[14] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11] + scores[12] + scores[13] + scores[14]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 16)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + "+\n" + scores[12] + "+" + scores[13] + "+" + scores[14] + "+" + scores[15] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11] + scores[12] + scores[13] + scores[14] + scores[15]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 17)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + "+\n" + scores[12] + "+" + scores[13] + "+" + scores[14] + "+" + scores[15] + "+" + scores[16] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11] + scores[12] + scores[13] + scores[14] + scores[15] + scores[16]) + "\n\n- Total Score: " + sm.score.ToString());
        else if (scores.Count == 18)
            scoreboard.text = ("- Test " + testNum + " Score:\n" + scores[0] + "+" + scores[1] + "+" + scores[2] + "+" + scores[3] + "+" + scores[4] + "+" + scores[5] + "+\n" + scores[6] + "+" + scores[7] + "+" + scores[8] + "+" + scores[9] + "+" + scores[10] + "+" + scores[11] + "+\n" + scores[12] + "+" + scores[13] + "+" + scores[14] + "+" + scores[15] + "+" + scores[16] + "+" + scores[17] + " = " + (scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9] + scores[10] + scores[11] + scores[12] + scores[13] + scores[14] + scores[15] + scores[16] + scores[17]) + "\n\n- Total Score: " + sm.score.ToString());
        else
            scoreboard.text = ("<color=red>Error, adjust code to \nallow more targets.</color>");

        //+ "+" + scores[1]
        //+ "+\n" + scores[1]

        //+scores[1]
    }

}
