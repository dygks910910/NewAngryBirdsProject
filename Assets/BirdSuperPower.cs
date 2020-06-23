using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YH_Class
{
    public class BirdSuperPower : MonoBehaviour
    {
        public enum ePowerType { None,YellowBirdPower,GreenBirdPower,BoombBirdPower}
        // Start is called before the first frame update
        public ePowerType superPowerType = ePowerType.None;
        public float fPower = 0;


        private BirdAnimationChanger animChanger;
        private bool usedPower = false;
        private SuperPower selectedSuperPower;
        void Awake()
        {
            animChanger = GetComponent<BirdAnimationChanger>();
            switch (superPowerType)
            {
                case ePowerType.None:
                    selectedSuperPower = null;
                    break;
                case ePowerType.YellowBirdPower:
                    selectedSuperPower = new YellowBirdSuperPower();
                    break;
                case ePowerType.GreenBirdPower:
                    selectedSuperPower = new GreenBirdSuperPower();
                    break;
                case ePowerType.BoombBirdPower:
                    selectedSuperPower = new BoombBirdSuperPower();
                    break;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if(superPowerType != ePowerType.None
                && animChanger.birdState == eBirdState.FLY)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    selectedSuperPower.DoSuperPower(gameObject, fPower);
                    usedPower = true;
                }
            }
        }
        private void OnDisable()
        {
            usedPower = false;
        }
    }


    interface SuperPower
    {
        void DoSuperPower(GameObject obj,float power);
    }
    class YellowBirdSuperPower : SuperPower
    {
        public void DoSuperPower(GameObject obj, float power)
        {
            YH_SingleTon.YH_ObjectPool.Instance.GetObj("ColisionEffectYellowBird",obj.transform.position);
            Rigidbody2D rgidbody = obj.GetComponent<Rigidbody2D>();
            Vector2 lookVec = rgidbody.velocity.normalized;
            rgidbody.AddForce(lookVec*power, ForceMode2D.Impulse);
        }
    }
    class GreenBirdSuperPower : SuperPower
    {
        public void DoSuperPower(GameObject obj, float power)
        {

        }
    }
    class BoombBirdSuperPower : SuperPower
    {
        public void DoSuperPower(GameObject obj, float power)
        {

        }
    }
}
