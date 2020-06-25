using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggCollisionBoomb : MonoBehaviour
{
    public float bombRadious = 0;
    public float bombPower = 0;
    public GameObject bombEffect;
    private Rigidbody2D rbody;
    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect = YH_SingleTon.YH_ObjectPool.Instance.GetObj(bombEffect.name, gameObject.transform.position);
        effect.GetComponent<ParticleSystem>().Play();
        YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(gameObject);
        YH_Helper.YH_Helper.ExplosionObjects(gameObject, bombRadious, bombPower);
    }
}
