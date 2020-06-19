using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimationChanger : MonoBehaviour
{
    Rigidbody2D birdRigidBody;
    Animator birdAnimator;
    // Start is called before the first frame update
    void Start()
    {
        birdRigidBody = GetComponent<Rigidbody2D>();
        birdAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        birdAnimator.SetFloat("velocity", birdRigidBody.velocity.magnitude);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        birdAnimator.SetBool("IsCollision",true);

    }
}
