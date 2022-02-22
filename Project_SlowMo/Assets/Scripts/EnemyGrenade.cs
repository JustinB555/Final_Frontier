using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
    JButler_Agent jAgent;

    public Transform throwPoint;
    public Rigidbody grenade;

    bool throwChance = false;

    AudioSource enemyAudio;
    [SerializeField] AudioClip enemyGrenade;

    float timer;
    float rand;

    [Space(10)]
    [Header("Adjustable Values")]

    [Tooltip("The chances of the enemy throwing a grenade. Higher the number = higher the chance. Max 10")]
    public float chance = 1;
    [Tooltip("The number of grenades this enemy posesses")]
    public int grenadeCount = 2;

    
    void Start()
    {
        jAgent = GetComponent<JButler_Agent>();
        enemyAudio = GetComponent<AudioSource>();

        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5)
        {

            if (jAgent.aggroLevel == JButler_Agent.AggroLevel.Level1)
            {
                if (jAgent.IsPeeking() && !jAgent.PlayerHidden())
                {
                    rand = Random.Range(0, 12);

                    if (rand <= chance)
                    {
                        if (grenadeCount > 0)
                        {
                            Rigidbody grenadeInstance;

                            grenadeInstance = Instantiate(grenade, throwPoint.position, throwPoint.rotation) as Rigidbody;
                            grenadeInstance.AddForce(Vector3.Cross(throwPoint.right, Quaternion.AngleAxis(-50, throwPoint.right) * throwPoint.up) * 125);
                            enemyAudio.clip = enemyGrenade;
                            enemyAudio.Play();
                            grenadeCount--;
                        }
                    }
                }
               
            }
            else if (jAgent.aggroLevel == JButler_Agent.AggroLevel.Level2)
            {
                if (jAgent.IsPeeking() && !jAgent.PlayerHidden())
                {
                    rand = Random.Range(0, 10);

                    if (rand <= chance)
                    {
                        if (grenadeCount > 0)
                        {
                            Rigidbody grenadeInstance;

                            grenadeInstance = Instantiate(grenade, throwPoint.position, throwPoint.rotation) as Rigidbody;
                            grenadeInstance.AddForce(Vector3.Cross(throwPoint.right, Quaternion.AngleAxis(-50, throwPoint.right) * throwPoint.up) * 125);
                            enemyAudio.clip = enemyGrenade;
                            enemyAudio.Play();
                            grenadeCount--;
                        }
                    }
                }
               
            }
            timer = 0;
        }
    }
}
