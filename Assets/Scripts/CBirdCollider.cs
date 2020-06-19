using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBirdCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        YH_Effects.Effects.CreateBirdCollisionEffect(gameObject.transform.position);
    }
}
