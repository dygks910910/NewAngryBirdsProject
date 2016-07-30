using UnityEngine;
using System.Collections;
public class PigHP : MonoBehaviour {

    public float hp;
    private Animator animator;
    private Rigidbody2D rigid;
    private float MAX_HP;
    private AnimationClip dieClip;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        MAX_HP = hp;
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject colObject = col.gameObject;
        //벽이랑 충돌 rigidBody 없음 아무일도 안일어남.
        if (colObject.tag == "Wall")
            return;
        //땅이랑 충돌 rigidBody없음
        if (colObject.tag == "Ground")
        {
            //땅은 움직이지 않으므로 객체 자신의 질량과속도를 구해서 hp를 깍아준다.
            hp -= rigid.velocity.magnitude * rigid.mass;
            print(hp);
            Debug.Log("땅이랑 충돌");
        }
        //돼지랑 충돌 rigidBody있음.
        if (colObject.tag == "Enermy")
        {
            hp -= col.relativeVelocity.magnitude * col.rigidbody.mass;        //질량 * 속도를 곱해서 충격량을 구하고 HP를 깎는다.
            print(hp);
            Debug.Log("돼지랑 충돌");

        }
        //같은 오브젝트 끼리 충돌 rigidBody 있음.
        if (colObject.tag == "BuildingObject")
        {
            hp -= col.relativeVelocity.magnitude * col.rigidbody.mass;        //질량 * 속도를 곱해서 충격량을 구하고 HP를 깎는다.
            print(hp);
            Debug.Log("오브젝트랑 충돌");

        }
        //플레이어 객체랑 충돌
        if (colObject.tag == "Player")
        {
            hp -= col.relativeVelocity.magnitude * col.rigidbody.mass;        //질량 * 속도를 곱해서 충격량을 구하고 HP를 깎는다.
            print(hp);
            Debug.Log("플레이어 객체랑 충돌");
        }
        print(hp);

        if (hp < MAX_HP * 0.3f)
        {
            animator.SetBool("LowHP", true);
        }
        else if (hp < MAX_HP * 0.7f)
        {
            animator.SetBool("MidHP", true);

        }
        if (hp <= 0)
        {
            StartCoroutine(Die());

        }
    }

    IEnumerator Die()
    {
        animator.SetBool("Die", true);
        yield return new WaitForSeconds (0.3f);
        Destroy(gameObject);

    }

}
