using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_EnemySpawner : MonoBehaviour
{
    //////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////

    // public
    [Header("Agents to Spawn")]
    [Tooltip("Put what agents you want to spawn here.")]
    [SerializeField] JButler_Agent[] agents;

    [Header("Checks")]
    [Tooltip("Checks to see if this agent has already spawned, preventing from triggering again.")]
    [SerializeField] bool spawned = false;

    // private

    // Start is called before the first frame update
    void Start()
    {
        if (agents.Length == 0)
            throw new System.Exception(name + " <color=red><b>has no agents to spawn!</color></b>\tDid you make sure to add agents to the spawner?");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //////////////////////////////////////////////////
    // Private Methods
    //////////////////////////////////////////////////

    void SpawnEnemy()
    {
        if (!spawned)
        {
            for (int i = 0; i < agents.Length; i++)
                agents[i].gameObject.SetActive(true);
            spawned = true;
        }
    }

    //////////////////////////////////////////////////
    // Collision Events
    //////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SpawnEnemy();
    }
}
