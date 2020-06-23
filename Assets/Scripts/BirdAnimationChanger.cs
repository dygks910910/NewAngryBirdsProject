using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Class
{
    public enum eBirdState{IDLE,FLY,COLLIDED }
    public class BirdAnimationChanger : MonoBehaviour
    {
        Rigidbody2D birdRigidBody;
        Animator birdAnimator;
        public eBirdState birdState = eBirdState.IDLE;

        private static WaitForSeconds wait1Sec = new WaitForSeconds(0.1f); 
        // Start is called before the first frame update
        void Awake()
        {
            birdRigidBody = GetComponent<Rigidbody2D>();
            birdAnimator = GetComponent<Animator>();
        }
        // Update is called once per frame
        private void FixedUpdate()
        {
            //각속도+파워를 업데이트.
            birdAnimator.SetFloat("velocity", birdRigidBody.velocity.magnitude + Mathf.Abs(birdRigidBody.angularVelocity));
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            birdAnimator.SetBool("IsCollision", true);
            birdState = eBirdState.COLLIDED;
        }


    }

}
