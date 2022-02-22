using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPillParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem ps = null;
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    public void BTPillParticlePlay()
    {
        ps.Play();
    }
}
