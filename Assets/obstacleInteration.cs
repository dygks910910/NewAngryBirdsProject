using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class obstacleInteration : MonoBehaviour
{
    const int MAX_SPRITE_COUNT = 4;
    public int max_hp = 50;
    public int hp = 0;
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
            hpDevisionStage[idx++] = (hp / 4) * i;
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
            DestoryObject();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //외부 힘 충격량 계산.
        hp -= (int)CalcPower(collision);
    }

    private float CalcPower(Collision2D collision)
    {
        //rigidBody 끼리 충돌.
        float power = 0;
        if (collision.rigidbody != null)
            power = collision.rigidbody.velocity.magnitude;
        //내부힘 rigidbody
        if (collision.otherRigidbody != null)
        {
            power += collision.otherRigidbody.velocity.magnitude;
        }
        power += collision.relativeVelocity.magnitude;

        return power;
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
    private void DestoryObject()
    {
        //if (gameObject.CompareTag("WoodObstacle"))
        //    YH_Effects.Effects.CreateWoodBreakEffect(gameObject.transform.position);
        //else if (gameObject.CompareTag("IceObstacle"))
        //    YH_Effects.Effects.CreateWoodBreakEffect(gameObject.transform.position);
        //else if (gameObject.CompareTag("StoneObstacle"))
        //    YH_Effects.Effects.CreateWoodBreakEffect(gameObject.transform.position);
        //Destroy(gameObject);
        GameObject tmpObj;
        tmpObj = YH_SingleTon.YH_ObjectPool.Instance.GetObj(destoryEffect.name);
        tmpObj.transform.position = gameObject.transform.position;
        tmpObj.GetComponent<ParticleSystem>().Play();
        YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(gameObject);

    }
    private void OnEnable()
    {
        hp = max_hp;
    }

}
