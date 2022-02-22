using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] int startHealth = 20;

    int currHealth;

    [SerializeField] bool destructable = true;
    [SerializeField] int damage = 100;

    Collider[] detections;

    [SerializeField] LayerMask detectionLayers;

    [SerializeField] LayerMask destructablePhysics;

    bool detonate = false;

    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> destructables = new List<GameObject>();

    GameObject player = null;
    public GameObject fire;
    public GameObject oil;

    ParticleSystem explosion;

    Sound_Manager sm;
    Explosives_Manager exm = null;

    void Start()
    {
        currHealth = startHealth;
        explosion = GetComponent<ParticleSystem>();
        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
        exm = FindObjectOfType<Explosives_Manager>();
    }

    void Update()
    {
#if UNITY_EDITOR
        //damage testing
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            TakeDamage(15);
        }
#else

#endif

        

        if (currHealth <= 0)
        {
            CancelInvoke("DamageOverTime");
            GatherTargets();
            if (!detonate)
            {
                detonate = true;
                exm.Store_Exp_Data();
                Invoke("Explode", 0.1f);
            }
        }

        
    }

    public bool Detonate()
    {
        return detonate;
    }

    public void NewDetonate(bool value)
    {
        detonate = value;
    }

    public void LoadExplosive()
    {
        if (detonate)
        {
            //Disappear and remove
            GetComponent<MeshRenderer>().enabled = false;
            //GetComponent<MeshCollider>().enabled = false;
            //GetComponent<SphereCollider>().enabled = false;
            fire.SetActive(false);
            oil.SetActive(false);
            Destroy(gameObject, 0.25f);
        }
    }

    public void TakeDamage(int damage)
    {
        if (destructable)
        {
            currHealth -= damage;

            sm.Play("MetalHit");
        }

        if (currHealth <= 10)
        {
            InvokeRepeating("DamageOverTime", 3, 1);
            fire.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 6);
    }

    void DamageOverTime()
    {
        currHealth -= 1;
    }

    void GatherTargets()
    {
        detections = Physics.OverlapSphere(transform.position, 6, detectionLayers);

        for (int i = 0; detections.Length > i; i++)
        {
            if (detections[i].gameObject.GetComponent<Player_Values>())
            {
                player = detections[i].gameObject;
            }

            if (detections[i].gameObject.GetComponent<JButler_EnemyTarget>() && detections[i].CompareTag("Head"))
            {
                if (!enemies.Contains(detections[i].gameObject))
                {
                    enemies.Add(detections[i].gameObject);
                }
            }

            if (detections[i].gameObject.GetComponent<ExplosiveBarrelTarget>())
            {
                if (!detections[i].transform.IsChildOf(transform))
                {
                    destructables.Add(detections[i].gameObject);
                }
            }

            if (detections[i].gameObject.GetComponent<Destructables>())
            {
                destructables.Add(detections[i].gameObject);
            }
        }
    }

    void Explode()
    {
        Debug.Log("EXPLOSIONS");
        if(enemies.Count != 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<JButler_EnemyTarget>().e.GetComponent<JButler_Enemy>().TakeDamage(damage);
            }
        }
        
        if(player != null)
        {
            player.GetComponent<Player_Values>().TakeDamage(damage - 35);
        }

        if(destructables.Count != 0)
        {
            for (int i = 0; i < destructables.Count; i++)
            {
                if(destructables[i].gameObject != null)
                {
                    if (destructables[i].GetComponentInParent<ExplosiveBarrel>())
                    {
                        destructables[i].GetComponentInParent<ExplosiveBarrel>().TakeDamage(damage);
                    }

                    if (destructables[i].GetComponent<Destructables>())
                    {
                        destructables[i].GetComponent<Destructables>().TakeDamage(damage);
                    }
                }
                else
                {
                    Debug.Log("Skipped already destroyed object");
                }
                
            }
        }

        sm.Play("BarrelExplosion");
        explosion.Play();

        Invoke("ExplosiveForce", 0.1f);

        //Disappear and remove
        GetComponent<MeshRenderer>().enabled = false;
        //GetComponent<MeshCollider>().enabled = false;
        //GetComponent<SphereCollider>().enabled = false;
        fire.SetActive(false);
        //oil.SetActive(false);
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
