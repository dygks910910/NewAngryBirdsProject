using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YH_Class
{
    public class BirdSuperPower : MonoBehaviour
    {
        // Start is called before the first frame update
        public ePowerType superPowerType = ePowerType.None;
        public float fPower = 0;


        private BirdAnimationChanger animChanger;
        public bool usedPower= false;

        public enum ePowerType { None, YellowBirdPower, GreenBirdPower, BoombBirdPower,
            BlueBirdSuperPower, ChickinBirdSuperPower }
        static public SuperPower[] superPowers = {null, new YellowBirdSuperPower(), new GreenBirdSuperPower(),
        new BoombBirdSuperPower(),new BlueBirdSuperPower(),new ChickinBirdSuperPower()};
        void Awake()
        {
            animChanger = GetComponent<BirdAnimationChanger>();
        }
        // Update is called once per frame
        void Update()
        {
            if(superPowerType != ePowerType.None
                && animChanger.birdState == eBirdState.FLY)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    SuperPower.DoingMethod needDoingCoroutine = superPowers[(int)superPowerType].DoSuperPower(gameObject, fPower);
                    if(needDoingCoroutine != null)
                    {
                        StartCoroutine(needDoingCoroutine(gameObject, fPower));
                    }
                    usedPower = true;
                }
            }
        }
        private void OnDisable()
        {
            usedPower = false;
        }
        public SuperPower GetSuperPower(ePowerType type)
        {
            return superPowers[(int)type];
        }
    }


    abstract public class SuperPower
    {
        //MonoBehaviour를 상속받지 않았기 때문에 리턴하여 코루틴을 사용하기 위함.
        public delegate IEnumerator DoingMethod(GameObject obj, float power);
        public abstract DoingMethod DoSuperPower(GameObject obj,float power);
    }
    class YellowBirdSuperPower : SuperPower
    {
        public override DoingMethod DoSuperPower(GameObject obj, float power)
        {
            YH_Helper.YH_Helper.CreateCollisionEffects("ColisionEffectYellowBird", obj.transform.position);
            Rigidbody2D rgidbody = obj.GetComponent<Rigidbody2D>();
            Vector2 lookVec = rgidbody.velocity.normalized;
            rgidbody.AddForce(lookVec * power, ForceMode2D.Impulse);
            return null;
        }
    }
    class BlueBirdSuperPower : SuperPower
    {
        const int offsetFactor = 3;
        public override SuperPower.DoingMethod DoSuperPower(GameObject obj, float power)
        {
            //오브젝트 활성화.
            YH_Helper.YH_Helper.CreateCollisionEffects("ColisionEffectBlueBird", obj.transform.position);
            GameObject upBird1 = YH_SingleTon.YH_ObjectPool.Instance.GetObj("BlueBird", obj.transform.position);
            GameObject downBird1 = YH_SingleTon.YH_ObjectPool.Instance.GetObj("BlueBird", obj.transform.position);

            //offset 값 얻어오기.
            CapsuleCollider2D coll = obj.GetComponent<CapsuleCollider2D>();
            Vector2 birdCollSize = coll.size;

            //offset 값 세팅.
            upBird1.transform.position = new Vector3(upBird1.transform.position.x,
                upBird1.transform.position.y + birdCollSize .y* offsetFactor,
                upBird1.transform.position.z);

            downBird1.transform.position = new Vector3(downBird1.transform.position.x,
                downBird1.transform.position.y - birdCollSize.y * offsetFactor,
                downBird1.transform.position.z);

            Rigidbody2D rgidbody = obj.GetComponent<Rigidbody2D>();
            Rigidbody2D uprgidbody = upBird1.GetComponentInChildren<Rigidbody2D>();
            Rigidbody2D downrgidbody = downBird1.GetComponentInChildren<Rigidbody2D>();

            //rigidBody값 세팅.
            uprgidbody.velocity = rgidbody.velocity;
            downrgidbody.velocity = rgidbody.velocity;
            uprgidbody.AddForce(new Vector2(0, power), ForceMode2D.Impulse);
            downrgidbody.AddForce(new Vector2(0, -power), ForceMode2D.Impulse);
            return null;
        }
    }
    class GreenBirdSuperPower : SuperPower
    {
        private static WaitForSeconds wfs = new WaitForSeconds(0.1f);
        public override SuperPower.DoingMethod DoSuperPower(GameObject obj, float power)
        {
            Rigidbody2D rigid = obj.GetComponent<Rigidbody2D>();
            rigid.AddTorque(-power,ForceMode2D.Impulse);
            YH_Helper.YH_Helper.CreateCollisionEffects("ColisionEffectGreenBird", obj.transform.position);
            return SuperPowerRoutine;
            //StartCorutine
        }
        private IEnumerator SuperPowerRoutine(GameObject obj, float power)
        {
            Rigidbody2D rigid = obj.GetComponent<Rigidbody2D>();
            BirdAnimationChanger animChanger = obj.GetComponent<BirdAnimationChanger>();
            //x축 힘 + power.
            float veloc = rigid.velocity.x +power;
                //진행방향의 반대되는 힘을 0.5초동안 가함.
            for (int i = 0; i < 5; ++i)
            {
                if (animChanger.birdState != eBirdState.FLY)
                    break;
                rigid.AddForce(new Vector2(-(veloc*2)*0.3f, 0), ForceMode2D.Impulse);
                yield return wfs;
            }
        }
    }
    class BoombBirdSuperPower : SuperPower
    {
        const float radious = 1.5f;
        public override SuperPower.DoingMethod DoSuperPower(GameObject obj, float power)
        {
            YH_SingleTon.YH_ObjectPool.Instance.GetObj("BoombBirdSuperPowerEffect", obj.transform.position);
            YH_SingleTon.YH_ObjectPool.Instance.GetObj("ColisionEffectBoombBird", obj.transform.position);

            YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(obj);

            YH_Helper.YH_Helper.ExplosionObjects(obj, radious, power);
            return null;
        }
    }
    class ChickinBirdSuperPower : SuperPower
    {
        const float radious = 1.5f;
        public override SuperPower.DoingMethod DoSuperPower(GameObject obj, float power)
        {
            YH_SingleTon.YH_ObjectPool.Instance.GetObj("BoombBirdSuperPowerEffect", obj.transform.position);
            YH_SingleTon.YH_ObjectPool.Instance.GetObj("ColisionEffectBoombBird", obj.transform.position);
            GameObject eggBomb = YH_SingleTon.YH_ObjectPool.Instance.GetObj("EggBomb", obj.transform.position);
            float collideryOffset = eggBomb.GetComponent<CapsuleCollider2D>().size.y;
            eggBomb.transform.position = new Vector2(eggBomb.transform.position.x, eggBomb.transform.position.y - collideryOffset*3);
            //YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(obj);
            Rigidbody2D objRbody = obj.GetComponent<Rigidbody2D>();
            Rigidbody2D eggRbody = eggBomb.GetComponent<Rigidbody2D>();

            eggRbody.AddForce(new Vector2(0,-power*2),ForceMode2D.Impulse);
            objRbody.AddForce(new Vector2(0, power), ForceMode2D.Impulse);


            return null;
        }
    }
}
