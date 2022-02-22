using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHealth : MonoBehaviour
{
    

    [SerializeField]
    ParticleSystem ps = null;



    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PkParticle()
    {
        ps.Play();
    }
}
