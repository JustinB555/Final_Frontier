using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool IsPlayerInCheckpoint = false;
    public bool hasPlayerLeftCheckpoint = false;
    CheckpointManager cpm = null;
    public int objectIdentifier = 0;
    // Start is called before the first frame update
    void Start()
    {
        cpm = FindObjectOfType<CheckpointManager>();
        hasPlayerLeftCheckpoint = false;

        objectIdentifier = int.Parse(gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasPlayerLeftCheckpoint == false)
        {
            IsPlayerInCheckpoint = true;
            cpm.UpdateCurrentCheckpointValue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayerLeftCheckpoint = true;
        }
    }
}
