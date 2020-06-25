using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH_Class;

namespace YH_Class
{
    public class CBirdCollider : MonoBehaviour
    {
        [SerializeField]
        private GameObject birdCollisionEffect;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            YH_Helper.YH_Helper.CreateCollisionEffects(birdCollisionEffect.name, gameObject.transform.position);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("WorldBoundary"))
            {
                YH_Helper.YH_Helper.BirdDieProcessing(gameObject.transform.parent.gameObject, gameObject);
            }
        }
    }

}
