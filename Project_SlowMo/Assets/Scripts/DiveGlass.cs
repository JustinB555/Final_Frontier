using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveGlass : MonoBehaviour
{
    [SerializeField] GameObject explosionPoint;

    [SerializeField] LayerMask destructablePhysics;

    Collider playerCollider;

    [SerializeField] GameObject bulletCollider;

    bool diveDestroyed = false;
    Sound_Manager sm;
    void Start()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();

        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
    }

    void Update()
    {
        if(bulletCollider == null && !diveDestroyed)
        {
            SendMessage("TakeDamage", 20);

            //Debug.Log("OOPS");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            diveDestroyed = true;
            Destroy(bulletCollider);

            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].isKinematic = false;
            }

            Collider[] colliders = GetComponentsInChildren<Collider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                Physics.IgnoreCollision(colliders[i], playerCollider);
            }

            sm.Play("GlassShatter");

            Collider[] physicObjects = Physics.OverlapSphere(explosionPoint.transform.position, 5, destructablePhysics);
            //Debug.Log(physicObjects);
            for (int i = 0; i < physicObjects.Length; i++)
            {
                Rigidbody rb = physicObjects[i].GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(500, explosionPoint.transform.position, 15);
                }
            }

            Invoke("DestroyAfterEffect", 5);
        }
    }

    void DestroyAfterEffect()
    {
        Destroy(gameObject);
    }
}
