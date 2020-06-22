using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlParticlesInChild : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem[] particles;
    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }
    public void PlayParticle()
    {
        for(int i = 0; i < particles.Length; ++i)
        {
            particles[i].Play();
        }
    }
}
