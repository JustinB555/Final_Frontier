using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSwitchTrigger : MonoBehaviour
{
    public JButler_Agent[] agents;
    //public GameObject playerEnemyView;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CharacterController>())
        {
            foreach (JButler_Agent a in agents)
                a.fakePoint = null;
            gameObject.SetActive(false);
        }
    }
}
