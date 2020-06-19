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
                Destroy(gameObject);
                Destroy(pivotObj);
                YH_Effects.Effects.CreateWhiteDust(gameObject.transform.position);
            }    
        }
    }

}
