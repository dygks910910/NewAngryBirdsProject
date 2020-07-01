using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigInteraction : MonoBehaviour
{

    public float pigHp = 10;
    public GameObject destroyEffect;
    private Animator animator;
    const int MAX_STATE_COUNT = 3;
    private int[] hpDevisionStage = new int[MAX_STATE_COUNT];
    int preIdx = -1;
    public delegate void PigDieProcessing(GameObject obj,ref PigDieProcessing evtHandle);
    public event PigDieProcessing pigDieEvtHandle; 
    private void Start()
    {
        animator = GetComponent<Animator>();
        int idx = 0;
        for (int i = 1; i <= MAX_STATE_COUNT; ++i)
        {
            hpDevisionStage[idx++] = (int)(pigHp / MAX_STATE_COUNT) * i;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float power = (int)YH_Helper.YH_Helper.CalcPower(collision);

        //외부 힘 충격량 계산.
        if((int)pigHp > 0)
        {
            pigHp -= power;
            YH_Helper.YH_Helper.Create3DScore((int)power * 100, gameObject.transform.position);

        }


    }
    private void Update()
    {
        int idx = GetSpriteIdxByHp();
        if (idx != -1 && preIdx != idx)
        {
            switch (idx)
            {
                case 0:
                    animator.SetBool("LowHP", true);
                    break;
                case 1:
                    animator.SetBool("MiddleHP", true);
                    break;
                case 2:
                    animator.SetBool("FullHP", true);
                    break;
            }

            preIdx = idx;
        }
        if (pigHp < 0)
        {
            YH_Helper.YH_Helper.DestoryObject(destroyEffect, gameObject);
            YH_Helper.YH_Helper.Create3DScore(5000, gameObject.transform.position,Color.green);
            pigDieEvtHandle(gameObject, ref pigDieEvtHandle);
        }   

    }
    private int GetSpriteIdxByHp()
    {
        for (int i = 0; i < MAX_STATE_COUNT - 1; ++i)
        {
            if (hpDevisionStage[i] >= pigHp)
            {
                return i;
            }
        }
        return -1;
    }
}
