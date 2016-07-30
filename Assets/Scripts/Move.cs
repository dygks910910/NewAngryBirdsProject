using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
    public float MoveSpeed;
    private Rigidbody2D rigid;
	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        
    }
   
    // Update is called once per frame
    void Update () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rigid.AddForce(new Vector2(x*10, y*10));
        //print(x);
	}
    void OnCollision2DEnter(Collider2D col)
    {

    }
}
