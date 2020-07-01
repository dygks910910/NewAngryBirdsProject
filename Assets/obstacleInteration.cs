using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class obstacleInteration : MonoBehaviour
{
    const int MAX_SPRITE_COUNT = 3;
    public float max_hp = 50;
    public float hp = 0;
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    public GameObject destoryEffect = null;

    [SerializeField]
    private int[] hpDevisionStage = new int[MAX_SPRITE_COUNT];
    int preIdx = -1;

    // Start is called before the first frame update
   
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //단계별로 보여줄 HP계산.
        int idx = 0;
        for(int i = 1; i <= MAX_SPRITE_COUNT; ++i)
        {
            hpDevisionStage[idx++] = (int)(hp / MAX_SPRITE_COUNT) * i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int idx = GetSpriteIdxByHp();
        if(idx != -1 && preIdx != idx)
        {
            Debug.Log("changeSprite");
            spriteRenderer.sprite = sprites[idx];
            preIdx = idx;
        }
        if (hp < 0)
            YH_Helper.YH_Helper.DestoryObject(destoryEffect,gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float power = YH_Helper.YH_Helper.CalcPower(collision);
        //외부 힘 충격량 계산.
        hp -= power;
        if((int)hp > 0)
        {
            int powerInt = (int)power * 100;
            if(powerInt > 0)
                YH_Helper.YH_Helper.Create3DScore(powerInt, gameObject.transform.position);
        }
    }


    private int GetSpriteIdxByHp()
    {
        for (int i = 0; i < MAX_SPRITE_COUNT-1; ++i)
        {
            if (hpDevisionStage[i] >= hp)
            {
                return i;
            }
        }
        return -1;
    }
  
    private void OnEnable()
    {
        hp = max_hp;
        spriteRenderer.sprite = sprites[3];
    }

}
