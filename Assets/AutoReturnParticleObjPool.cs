using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnParticleObjPool : MonoBehaviour
{
    ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!particle.IsAlive())
        {
            YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(gameObject);
        }
    }
}
