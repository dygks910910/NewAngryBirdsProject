using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Class
{

    public class BirdDirectionController : MonoBehaviour
    {
        private Rigidbody2D rBody;
        private BirdAnimationChanger animChanger;
        // Start is called before the first frame update
        void Awake()
        {
            rBody = GetComponent<Rigidbody2D>();
            animChanger = GetComponent<BirdAnimationChanger>();
        }

        // Update is called once per frame
        void Update()
        {
            if (animChanger.birdState == eBirdState.FLY)
            {
                //날아가는 방향에 따라 회전시켜줌.
                Vector3 direction = Vector3.Normalize(rBody.velocity);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

}
