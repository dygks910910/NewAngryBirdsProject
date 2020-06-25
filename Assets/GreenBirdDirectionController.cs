using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Class
{

    public class GreenBirdDirectionController : MonoBehaviour
    {
        Rigidbody2D rigid;
        BirdAnimationChanger animChanger;
        // Start is called before the first frame update
        void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            animChanger = GetComponent<BirdAnimationChanger>();
            animChanger.onChangeToShottingStateEvent += onChangedtoShottingState;
        }

        private void onChangedtoShottingState(GameObject obj)
        {
            if(obj == gameObject)
            {
                rigid.AddTorque(-1,ForceMode2D.Impulse);
                animChanger.onChangeToShottingStateEvent -= onChangedtoShottingState;
            }
        }
        // Update is called once per frame

    }
}
