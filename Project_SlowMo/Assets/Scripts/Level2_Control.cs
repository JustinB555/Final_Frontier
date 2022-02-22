using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2_Control : MonoBehaviour
{
    bool active = false;

    Animator anim;
    Sound_Manager sm;

    [SerializeField] GameObject[] destroyObjects;
    [SerializeField] GameObject secondWaveDoor;
    //[SerializeField] GameObject[] enemies;
    [SerializeField] GameObject brokenWall;

    [SerializeField] ParticleSystem doorGroupA;
    [SerializeField] ParticleSystem doorGroupA2;
    [SerializeField] ParticleSystem doorB;

    public GameObject railingSecure;
    //public GameObject railingDamaged;
    public GameObject fallTower;
    public GameObject ShatterGlass;
    public GameObject enemySpawner;
    public GameObject wave2Spawner;

    public ParticleSystem explosion1;
    public ParticleSystem explosion2;
    public ParticleSystem explosion3;
    void Start()
    {
        anim = GetComponent<Animator>();
        sm = GameObject.Find("SoundManager").GetComponent<Sound_Manager>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!active)
            {
                active = true;

                anim.SetBool("FallStart", true);
            }
        }
    }

    public void GlassShatter()
    {
        ShatterGlass.SetActive(false);
        GetComponent<ParticleSystem>().Play();
        sm.Play("GlassShatter");
    }

    public void Balcony()
    {
        railingSecure.SetActive(false);
        //railingDamaged.SetActive(true);
    }

    public void DestroyTower()
    {
        Destroy(fallTower);
    }

    public void OpenDoors()
    {
        doorGroupA.Play();
        doorGroupA2.Play();
        sm.Play("BarrelExplosion");
        for (int i = 0; i < destroyObjects.Length; i++)
        {
            destroyObjects[i].SetActive(false);
        }
        brokenWall.SetActive(true);
    }

    public void OpenSecondWave()
    {
        doorB.Play();
        sm.Play("GrenadeExplosion");
        secondWaveDoor.SetActive(false);
    }

    public void DetonateA()
    {
        explosion1.Play();
        sm.Play("BarrelExplosion");
    }

    public void DetonateB()
    {
        explosion2.Play();
        sm.Play("GrenadeExplosion");
    }

    public void DetonateC()
    {
        explosion3.Play();
        sm.Play("GrenadeExplosion");
    }

    public void SpawnerOn()
    {
        enemySpawner.SetActive(true);
        Invoke("Wave2", 15);
    }

    void Wave2()
    {
        OpenSecondWave();
        wave2Spawner.SetActive(true);
    }
}
