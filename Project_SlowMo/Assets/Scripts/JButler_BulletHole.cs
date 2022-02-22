using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JButler_BulletHole : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("This value sets how long a bullet hole will stay active before destroying itself.")]
    [SerializeField] float despawnTime = 0.1f;

    float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= despawnTime)
            Destroy(gameObject);
    }
}
