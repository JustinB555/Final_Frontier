using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [SerializeField]
    Animator Tower1 = null;
    [SerializeField]
    GameObject Target = null;



    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if (Target.activeInHierarchy == false)
        {
            Tower1.SetTrigger("TD12");
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Tower1.SetTrigger("TD1");
        }

        
    }
}
