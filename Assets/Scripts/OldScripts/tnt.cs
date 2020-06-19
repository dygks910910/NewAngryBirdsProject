using UnityEngine;
using System.Collections;

public class tnt : MonoBehaviour {
    private CircleCollider2D collider;
    public AudioClip bombSound;
    private Animator animator;
    private AudioSource source;
    private float hp = 1;
    private int count = 0;
    private bool touch = false;
	// Use this for initialization
	void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        touch = true;
        hp -= col.relativeVelocity.magnitude;
        print("tnt충돌");
    }
    // Update is called once per frame
    void Update () {
        if (hp <= 0)
        {

            if (count < 5)
            {
                source.clip = bombSound;
                source.Play();
                collider.radius += 0.3f;
                count++;
            }
            else
            {
                animator.SetBool("Die", true);
                Destroy(gameObject,0.5f);
            }
        }

    }
}
