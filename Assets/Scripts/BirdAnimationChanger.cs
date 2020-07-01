using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Class
{
    public enum eBirdState { IDLE, FLY, COLLIDED, DiSABLE }
    public class BirdAnimationChanger : MonoBehaviour
    {
        #region 기본 객체 초기화
        private Rigidbody2D birdRigidBody;
        private Animator birdAnimator;
        #endregion

        //score 색을 표현하기 위한 변수.
        public Color birdColor;
        public eBirdState birdState = eBirdState.IDLE;


        #region delegate,event
        public delegate void OnChangeEvent(GameObject obj);
        public event OnChangeEvent onChangeToShottingStateEvent;

        public delegate IEnumerator OnchangetoCollisionMethod();
        public event OnchangetoCollisionMethod onChangeToCollitionStateEvent;
        #endregion

        public GameObject birdCollisionEffect;
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
            //각속도+파워를 업데이트.
            birdAnimator.SetFloat("velocity", birdRigidBody.velocity.magnitude + Mathf.Abs(birdRigidBody.angularVelocity));
            if (superPowerClass && superPowerClass.superPowerType != BirdSuperPower.ePowerType.None)
                birdAnimator.SetBool("useSuperPower", superPowerClass.usedPower);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            birdAnimator.SetBool("IsCollision", true);
            birdState = eBirdState.COLLIDED;
            YH_Helper.YH_Helper.CreateCollisionEffects(birdCollisionEffect.name, gameObject.transform.position);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("WorldBoundary"))
            {
                YH_Helper.YH_Helper.BirdDieProcessing(gameObject);
                birdState = eBirdState.COLLIDED;
            }
        }

    }
}
