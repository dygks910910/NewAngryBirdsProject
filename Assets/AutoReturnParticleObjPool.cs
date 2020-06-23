using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnParticleObjPool : MonoBehaviour
{
    //ParticleSystem particle;
    //// Start is called before the first frame update

    //private void Awake()
    //{
    //    if(particle == null)
    //        particle = GetComponent<ParticleSystem>();
    //}
    // Update is called once per frame
    private void OnParticleSystemStopped()
    {
       YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(gameObject);
    }

}
