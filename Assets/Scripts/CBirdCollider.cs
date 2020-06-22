using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBirdCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect =  YH_SingleTon.YH_ObjectPool.Instance.GetObj("ColisionEffectBird");
        effect.transform.position = gameObject.transform.position;
        effect.GetComponent<ControlParticlesInChild>().PlayParticle();
    }
}
