using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace YH_Class
{
    public class AutoDestroyBird : MonoBehaviour
    {
        public GameObject pivotObj;
        public GameObject WorldRect;
        private WaitForSeconds  waitSec = new WaitForSeconds(1);
        private Animator anim;
        private WorldArea worldAreaScripts;
        private Vector2 tmpVec;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            worldAreaScripts = WorldRect.GetComponent<WorldArea>();
            StartCoroutine(CheckBirdDeadState());

        }
        private IEnumerator CheckBirdDeadState()
        {
            while(gameObject.activeSelf)
            {
                if (YH_Helper.YH_Helper.CheckAnimStateIsDestory(anim))
                {
                    YH_Helper.YH_Helper.BirdDieProcessing(pivotObj, gameObject);
                    break;
                }
                yield return waitSec;
            }
        }
    }

}
