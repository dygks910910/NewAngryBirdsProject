using UnityEngine;
using System.Collections;

public class AttackHP : MonoBehaviour
{
    public float hp = 20.0f;
    public Sprite normal;
    public Sprite mid;
    public Sprite low;
    private SpriteRenderer renderer;
    private Rigidbody2D rigid;
    private Animator animator;
    private float MAX_HP;
    private AudioSource source;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        MAX_HP = hp;
        source = GetComponent<AudioSource>();
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
            source.Play();
            hp -= rigid.velocity.magnitude * rigid.mass;
            print(hp);
           // Debug.Log("땅이랑 충돌");
        }
        //돼지랑 충돌 rigidBody있음.
        if (colObject.tag == "Enermy")
        {
            source.Play();
            hp -= col.relativeVelocity.magnitude * col.rigidbody.mass;        //질량 * 속도를 곱해서 충격량을 구하고 HP를 깎는다.
            //print(hp);
           // Debug.Log("돼지랑 충돌");

        }
        //같은 오브젝트 끼리 충돌 rigidBody 있음.
        if (colObject.tag == "BuildingObject")
        {
            source.Play();
            hp -= col.relativeVelocity.magnitude * col.rigidbody.mass;        //질량 * 속도를 곱해서 충격량을 구하고 HP를 깎는다.
            //print(hp);
           // Debug.Log("오브젝트 끼리 충돌");

        }
        //플레이어 객체랑 충돌
        if (colObject.tag == "Player")
        {
            source.Play();
            hp -= col.relativeVelocity.magnitude * col.rigidbody.mass;        //질량 * 속도를 곱해서 충격량을 구하고 HP를 깎는다.
           // print(hp);
            //Debug.Log("플레이어 객체랑 충돌");
        }
       
        if (hp < MAX_HP * 0.3f)
        {
            print("들어옴");
            renderer.sprite = low;
        }
        else if (hp < MAX_HP * 0.5f)
        {
            print("들어옴");
            renderer.sprite = mid;
        }
        else if (hp <= MAX_HP * 0.8f)
        {
            print("들어옴");
            renderer.sprite = normal;
        }
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.4f);

    }

}
