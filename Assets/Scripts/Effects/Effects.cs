using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YH_Effects
{
    [Obsolete("Not use anyMore.Find in Object pool")]
    public class Effects : MonoBehaviour
    {
        public static void CreateBirdCollisionEffect(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetObj("ColisionEffectBird");
            Instantiate(tmp, position, Quaternion.identity);
        }
        public static void CreateWhiteDust(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetObj("WhiteDustInDestory");
            Instantiate(tmp, position, Quaternion.identity);
        }
        public static void CreateWoodBreakEffect(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetObj("WoodBreakEffect");
            Instantiate(tmp, position, Quaternion.identity);
        }
        public static void CreateStoneBreakEffect(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetObj("StoneBreackEffect");
            Instantiate(tmp, position, Quaternion.identity);
        }
        public static void CreateIceBreakEffect(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetObj("IceBreakEffect");
            Instantiate(tmp, position, Quaternion.identity);
        }

    }


}
