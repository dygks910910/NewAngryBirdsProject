using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YH_Effects
{
    public class Effects : MonoBehaviour
    {
        public static void CreateBirdCollisionEffect(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetPrefab("ColisionEffectBird");
            Instantiate(tmp, position, Quaternion.identity);
        }
        public static void CreateWhiteDust(Vector3 position)
        {
            GameObject tmp = YH_SingleTon.YH_ObjectPool.Instance.GetPrefab("WhiteDustInDestory");
            Instantiate(tmp, position, Quaternion.identity);
        }

    }


}
