using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBirdCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.otherCollider.name);
        GameObject effect =  YH_SingleTon.YH_ObjectPool.Instance.GetObj("ColisionEffectBird");
        effect.transform.position = gameObject.transform.position;
        effect.GetComponent<ControlParticlesInChild>().PlayParticle();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("WorldBoundary"))
        {
            YH_Helper.YH_Helper.BirdDieProcessing(gameObject.transform.parent.gameObject, gameObject);
        }
    }
}
