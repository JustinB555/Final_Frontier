using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targets : MonoBehaviour
{
    [SerializeField]
    GameObject GoodTarget = null;
    [SerializeField]
    GameObject GoodTarget2 = null;

    [SerializeField]
    Animator Animator = null;
    [SerializeField]
    Animator Animator2 = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GoodTarget.activeInHierarchy == false)
        {
            Animator.SetBool("FirstPickUp", true);
        }
        if (GoodTarget2.activeInHierarchy == false)
        {
            Animator2.SetBool("SecondPickUp", true);
        }
    }
}
