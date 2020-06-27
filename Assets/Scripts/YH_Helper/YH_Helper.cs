using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.Reflection;
using System;
using UnityEngine.UI;

namespace YH_Helper
{
    public static class YH_Helper 
    {
        //[Obsolete("다른 방식으로 접근하세요.")]
        public static bool CheckAnimStateIsDestory(Animator animator)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Destory"))
            {
            return true;

            }
            return false;
        }

        public static string[] GetSortingLayerNames()
        {
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty =
            internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        public static void BirdDieProcessing(GameObject pivotObj,GameObject birdObj)
        {
            //애니메이션 변수 초기화.
            Animator anim = birdObj.GetComponent<Animator>();
            anim.SetBool("IsCollision", false);
            anim.SetBool("ResetAnimation", false);

            BirdReturnToObjPool(pivotObj);
            CreateBirdDieEffect(birdObj);
        }
        private static void BirdReturnToObjPool(GameObject pivotObj)
        {

            YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(pivotObj);
        }
        private static void CreateBirdDieEffect(GameObject birdObj)
        {
            GameObject dust = YH_SingleTon.YH_ObjectPool.Instance.GetObj("WhiteDustInDestory");
            dust.SetActive(true);
            dust.transform.position = birdObj.transform.position;
            Animator anim;
            anim = birdObj.GetComponent<Animator>();
            dust.GetComponent<ParticleSystem>().Play();
            anim.SetBool("ResetAnimation", true);
        }

        public  static void CreateCollisionEffects(string effectName,Vector2 position)
        {
            GameObject effect = YH_SingleTon.YH_ObjectPool.Instance.GetObj(effectName);
            effect.transform.position = position;
            effect.GetComponent<ControlParticlesInChild>().PlayParticle();
        }

        public static void ExplosionObjects(GameObject obj,float bombRadious,float bombPower)
        {
            Rigidbody2D rbody;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(obj.transform.position, bombRadious);
            for (int i = 0; i < colliders.Length; ++i)
            {
                rbody = colliders[i].gameObject.GetComponent<Rigidbody2D>();
                if (rbody != null)
                {
                    rbody.AddExplosionForce(bombPower, obj.transform.position, bombRadious);
                }
            }
        }
        public static float CalcPower(Collision2D collision)
        {
            //rigidBody 끼리 충돌.
            float power = 0;
            if (collision.rigidbody != null)
                power = collision.rigidbody.velocity.magnitude;
            //내부힘 rigidbody
            if (collision.otherRigidbody != null)
            {
                power += collision.otherRigidbody.velocity.magnitude;
            }
            power += collision.relativeVelocity.magnitude;
            YH_SingleTon.ScoreManager.Instance.AddScore(250);
            return power;
        }
        public static void DestoryObject(GameObject destoryEffect, GameObject obj)
        {
            //if (gameObject.CompareTag("WoodObstacle"))
            //    YH_Effects.Effects.CreateWoodBreakEffect(gameObject.transform.position);
            //else if (gameObject.CompareTag("IceObstacle"))
            //    YH_Effects.Effects.CreateWoodBreakEffect(gameObject.transform.position);
            //else if (gameObject.CompareTag("StoneObstacle"))
            //    YH_Effects.Effects.CreateWoodBreakEffect(gameObject.transform.position);
            //Destroy(gameObject);
            GameObject tmpObj;
            tmpObj = YH_SingleTon.YH_ObjectPool.Instance.GetObj(destoryEffect.name);
            tmpObj.transform.position = obj.transform.position;
            tmpObj.GetComponent<ParticleSystem>().Play();

            YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(obj);
        }

        public static void Create3DScore(int score,Vector3 position)
        {
            Create3DScore(score, position, Color.white);
        }
        public static void Create3DScore(int score, Vector3 position,Color color)
        {
            GameObject textScore = YH_SingleTon.YH_ObjectPool.Instance.GetObj("3dCanvasTextField");
            PopupTextEffect effect = textScore.GetComponentInChildren<PopupTextEffect>();
            effect.CreateText(score.ToString(), position);
            textScore.GetComponentInChildren<Text>().color = color;
            YH_SingleTon.ScoreManager.Instance.AddScore(score);
        }
    }
   

}
