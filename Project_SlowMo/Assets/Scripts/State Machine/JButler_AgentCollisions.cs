using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_AgentCollisions : MonoBehaviour
{
    [Space(5)]
    [Header("Our Agent")]
    [Tooltip("Put AI_Enemy or whoever holds the JButler_Agent script here.")]
    [SerializeField] JButler_Agent ourAgent = null;

    private void Start()
    {
        if (ourAgent == null)
            throw new System.Exception(name + "'s field <b><color=red>Our Agent</color></b> is null!\t\tDid you forget to add the AI_Enemy to Our Agent?");
    }

    //////////////////////////////////////////////////
    // Collisions Events
    //////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<JButler_NavPoints>() && (ourAgent.aggroLevel == JButler_Agent.AggroLevel.Level1 || ourAgent.aggroLevel == JButler_Agent.AggroLevel.Level2) && !ourAgent.IsCrouching() && !ourAgent.IsPeeking() && ourAgent.LasHiding() != other.GetComponent<JButler_NavPoints>() && other.transform.position == ourAgent.CurHiding().position)
        {
            ourAgent.ToggleCrouch();
            //Debug.Log(ourAgent.name + " reached " + other.name);
        }
    }
}
