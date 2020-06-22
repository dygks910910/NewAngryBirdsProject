using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnParticlesObjPool : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem[] particles;
    bool isAllAlive = true;

    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < particles.Length; ++i)
        {
            isAllAlive &= particles[i].IsAlive();
        }
        if(!isAllAlive)
        {
            YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(gameObject);
        }
    }
}
