using System;
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
        public delegate void OnChangeEvent(GameObject obj);
        public event OnChangeEvent onChangeToShottingStateEvent;
        public delegate IEnumerator OnchangetoCollisionMethod();
        public event OnchangetoCollisionMethod onChangeToCollitionStateEvent;


        private BirdSuperPower superPowerClass;

        private static WaitForSeconds wait1Sec = new WaitForSeconds(0.1f); 
        // Start is called before the first frame update
        void Awake()
        {
            birdRigidBody = GetComponent<Rigidbody2D>();
            birdAnimator = GetComponent<Animator>();
            superPowerClass = GetComponent<BirdSuperPower>();
        }
        private void Update()
        {
            switch (birdState)
            {
                case eBirdState.FLY:
                    if (onChangeToShottingStateEvent != null)
                        onChangeToShottingStateEvent(gameObject);
                    break;
                case eBirdState.COLLIDED:
                    if (onChangeToCollitionStateEvent != null)
                        StartCoroutine(onChangeToCollitionStateEvent());
                    break;
            }

        }
        // Update is called once per frame
        private void FixedUpdate()
        {
            //각속도+파워를 업데이트.
            birdAnimator.SetFloat("velocity", birdRigidBody.velocity.magnitude + Mathf.Abs(birdRigidBody.angularVelocity));
            birdAnimator.SetBool("useSuperPower", superPowerClass.usedPower);

        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            birdAnimator.SetBool("IsCollision", true);
            birdState = eBirdState.COLLIDED;
        }

        private void OnEnable()
        {
            //birdAnimator.Play("Entry");
        }
    }

}
