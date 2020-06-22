using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Class
{
    public class AutoDestroyBird : MonoBehaviour
    {
        public GameObject pivotObj;
        Animator anim;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
           if(YH_Helper.YH_Helper.CheckAnimStateIsDestory(anim))
            {
                YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(pivotObj);
                //Destroy(gameObject);
                //Destroy(pivotObj);
                //YH_Effects.Effects.CreateWhiteDust(gameObject.transform.position);
                GameObject dust = YH_SingleTon.YH_ObjectPool.Instance.GetObj("WhiteDustInDestory");
                dust.SetActive(true);
                dust.transform.position = gameObject.transform.position;
                //dust.transform.parent = gameObject.transform;
                dust.GetComponent<ParticleSystem>().Play();
                //dust.
            }    
        }
    }

}
