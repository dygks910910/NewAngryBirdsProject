using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Class
{
    public class AfterOnCollisionDo : MonoBehaviour
    {

        public int delay = 0;
        public BirdSuperPower.ePowerType type = BirdSuperPower.ePowerType.BoombBirdPower;
        public float fPower = 0;
        SuperPower superPower;
        private BirdAnimationChanger animChanger;
        BirdSuperPower superPowerClass;
        // Start is called before the first frame update
        void Start()
        {
            animChanger = GetComponent<BirdAnimationChanger>();
            superPowerClass = GetComponent<BirdSuperPower>();
            animChanger.onChangeToCollitionStateEvent += doSomething;

        }
        private IEnumerator doSomething()
        {
            animChanger.onChangeToCollitionStateEvent -= doSomething;
            yield return new WaitForSeconds(delay);
            superPowerClass.GetSuperPower(type).DoSuperPower(gameObject,fPower);

        }
    }
}
