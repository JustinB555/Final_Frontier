using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : MonoBehaviour
{
    [Tooltip("How long should the pieces last in the world before despawning?")]
    [SerializeField] int lifeTime = 5;

    float currentTime = 0;
    void Start()
    {
        
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
