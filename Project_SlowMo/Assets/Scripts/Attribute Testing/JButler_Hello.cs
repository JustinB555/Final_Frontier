using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_Hello : MonoBehaviour
{
    string stealSecrets;

    // Start is called before the first frame update
    void Start()
    {
        stealSecrets = GetComponent<JButler_AttributeExamples>().mySecrets;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
