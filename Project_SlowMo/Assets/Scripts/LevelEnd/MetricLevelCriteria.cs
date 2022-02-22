using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricLevelCriteria : MonoBehaviour
{
    [SerializeField]
    EndLevelScript els = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            els.isCompletionCriteraMet = true;
        }
    }

}
