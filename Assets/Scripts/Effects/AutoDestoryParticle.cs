using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestoryParticle : MonoBehaviour
{
    ParticleSystem[] particles;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        bool bAllParticleStoped = true;
        for(int i =0; i < particles.Length; ++i)
        {
            if(particles[i] != null)
            bAllParticleStoped &= !particles[i].IsAlive();
        }
        if (bAllParticleStoped)
            Destroy(gameObject);
    }
}
