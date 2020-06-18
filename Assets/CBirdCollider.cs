using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBirdCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetPrefab("ColisionEffectBird");
        Instantiate(tmp, transform.position, collision.transform.rotation);
    }
}
