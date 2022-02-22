using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> destructables = new List<GameObject>();

    [SerializeField] LayerMask destructablePhysics;

    Collider[] detections;

    GameObject player = null;

    [SerializeField] LayerMask detectionLayers;

    [SerializeField] float fuseTimer = 3;
    float currentTimer;

    [SerializeField] int damage = 100;

    ParticleSystem explosion;
    Sound_Manager sm;

    [Tooltip("This is the Detection Trigger for the Grenade")]
    [SerializeField] SphereCollider detectionSphere;

    bool detonate = false;

    [SerializeField] GameObject pin;

    void Start()
    {
        explosion = GetComponent<ParticleSystem>();
        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
    }

    void Update()
    {
        currentTimer += Time.deltaTime;

        if(currentTimer >= fuseTimer)
        {
            GatherTargets();
            if (!detonate)
            {
                detonate = true;
                Invoke("Explode", 0.2f);
            }
        }
    }

    void GatherTargets()
    {
        //detectionSphere.radius = 20;

        

        detections = Physics.OverlapSphere(transform.position, 5, detectionLayers);

        for (int i = 0; detections.Length > i; i++)
        {
            if(detections[i].gameObject.GetComponent<Player_Values>())
            {
                player = detections[i].gameObject;
            }

            if(detections[i].gameObject.GetComponent<JButler_EnemyTarget>() && detections[i].CompareTag("Head"))
            {
                if (!enemies.Contains(detections[i].gameObject))
                {
                    enemies.Add(detections[i].gameObject);
                }
            }

            if (detections[i].gameObject.GetComponent<ExplosiveBarrelTarget>())
            {
                destructables.Add(detections[i].gameObject);
            }

            if(detections[i].gameObject.GetComponent<Destructables>())
            {
                destructables.Add(detections[i].gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Values>())
        {
            player = other.gameObject;
        }

        if (other.GetComponent<JButler_EnemyTarget>() && other.CompareTag("Body"))
        {
            enemies.Add(other.gameObject);
        }

        if (other.GetComponent<ExplosiveBarrelTarget>())
        {
            destructables.Add(other.gameObject);
        }

        if (other.GetComponent<Destructables>())
        {
            destructables.Add(other.gameObject);
        }
    }*/

    void Explode()
    {
        Debug.Log("EXPLOSIONS");
        if (enemies.Count != 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<JButler_EnemyTarget>().e.GetComponent<JButler_Enemy>().TakeDamage(damage);
                Debug.Log("Ouchie");
            }
        }

        if (player != null)
        {
            player.GetComponent<Player_Values>().TakeDamage(damage - 40);
        }

        if (destructables.Count != 0)
        {
            
            for (int i = 0; i < destructables.Count; i++)
            {
                if (destructables[i].gameObject != null)
                {
                    if (destructables[i].GetComponentInParent<ExplosiveBarrel>())
                    {
                        destructables[i].GetComponentInParent<ExplosiveBarrel>().TakeDamage(damage);
                    }

                    if (destructables[i].GetComponentInParent<Destructables>())
                    {
                        destructables[i].GetComponentInParent<Destructables>().TakeDamage(damage);
                    }
                }
            }
        }

        sm.Play("GrenadeExplosion");
        explosion.Play();

        Invoke("ExplosiveForce", 0.1f);
        

        //Disappear and remove
        GetComponent<MeshRenderer>().enabled = false;
        pin.GetComponent<MeshRenderer>().enabled = false;
       
    }

    
    void ExplosiveForce()
    {
        Collider[] physicObjects = Physics.OverlapSphere(transform.position, 5, destructablePhysics);
        //Debug.Log(physicObjects);
        for (int i = 0; i < physicObjects.Length; i++)
        {
            Rigidbody rb = physicObjects[i].GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(500, transform.position, 15);
            }
        }

        Destroy(gameObject, 0.25f);
    }
}
