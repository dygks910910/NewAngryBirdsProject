using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.Reflection;
using System;

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
    }

}
