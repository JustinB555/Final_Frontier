using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_RiotLevelEnd : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public


    // private
    EndLevelScript end = null;
    JButler_EnemyManager eManager = null;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            end = FindObjectOfType<EndLevelScript>();
            eManager = FindObjectOfType<JButler_EnemyManager>();
        }
        catch (System.NullReferenceException e)
        {
            if (end = null)
                throw new System.Exception(name + " could not find an object with the EndLevelScript.\tPlease make sure that there is one present inside this level.\n" + e.Message);

            else if (eManager = null)
                throw new System.Exception(name + " could not find an Enemy Manager.\tPlease make sure that there is one present inside this level.\n" + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        EndRequirements();
    }

    //////////////////////////////////////////////////
    // Collision Events
    //////////////////////////////////////////////////



    //////////////////////////////////////////////////
    // Public Methods
    //////////////////////////////////////////////////



    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void EndRequirements()
    {
        if (eManager.NumberOfEnemies() <= 0)
            end.isCompletionCriteraMet = true;
    }
}
