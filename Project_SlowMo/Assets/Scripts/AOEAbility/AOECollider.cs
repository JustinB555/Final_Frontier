using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOECollider : MonoBehaviour
{
    [SerializeField] AOEAbility papa = null;
    public bool isActive = false;
    [SerializeField] public List<GameObject> enemiesInTrigger = new List<GameObject>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body") && other.gameObject.GetComponent<JButler_EnemyTarget>())
        {
            enemiesInTrigger.Add(other.gameObject.GetComponent<JButler_EnemyTarget>().e.gameObject);
        }
        papa.EpicDamage();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Body") && other.gameObject.GetComponent<JButler_EnemyTarget>())
        {
            enemiesInTrigger.Remove(other.gameObject.GetComponent<JButler_EnemyTarget>().e.gameObject);
        }
    }

}
