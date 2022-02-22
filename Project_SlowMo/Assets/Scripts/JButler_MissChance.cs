#define TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_MissChance : MonoBehaviour
{
    public float missRadius = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //AIDirection();
    }

    public Vector3 AIDirection()
    {
        Vector3 aim = transform.position + Random.insideUnitSphere * missRadius;

        #region DEBUGGING
#if TEST
        Debug.DrawLine(aim - Vector3.forward * 0.01f, aim + Vector3.forward * 0.01f, Color.green, 50.0f);
        Debug.DrawLine(aim - Vector3.right * 0.01f, aim + Vector3.right * 0.01f, Color.green, 50.0f);
        Debug.DrawLine(aim - Vector3.up * 0.01f, aim + Vector3.up * 0.01f, Color.green, 50.0f);
#endif
        #endregion
        return aim;
    }
}
