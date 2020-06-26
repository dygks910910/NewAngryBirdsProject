using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH_Class;

public class GameManager : MonoBehaviour
{
    [Tooltip("새들을 저장하는 순서.순서대로 장전된다.")]
    public List<GameObject> birdList;
    public GameObject BirdGun;
    public GameObject mainCamera;

    private CamFollow camFollow;
    private static WaitForSeconds wfs = new WaitForSeconds(0.1f);
    private StrapController birdGunController;
    private Queue<GameObject> birdQueue = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
       YH_SingleTon.YH_ObjectPool.Instance.LoadAllPrefabs();
       birdGunController = BirdGun.GetComponent<StrapController>();
       camFollow = mainCamera.GetComponent<CamFollow>();
        GameObject bird;
       for (int i = 0; i < birdList.Count; ++i)
        {
            bird = YH_SingleTon.YH_ObjectPool.Instance.GetObj(birdList[i].name);
            bird.SetActive(false);
            birdQueue.Enqueue(bird);
        }
        ReloadBurdGun();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //return val 이 null일 경우에 새가 없음.
    private void ReloadBurdGun()
    {
        GameObject nextBird = GetNextBird();
        if(nextBird != null)
        {
            StartCoroutine(CheckBirdCollided(nextBird.GetComponentInChildren<BirdAnimationChanger>()));
            birdGunController.ReloadBirds(nextBird);
        }
        else
        {
            //null이면 게임 종료.결과창 출력.
        }
        
    }
    IEnumerator CheckBirdCollided(BirdAnimationChanger changer)
    {
        while(true)
        {
            if (changer.birdState == eBirdState.COLLIDED || !(changer.gameObject.activeSelf))
            {
                ReloadBurdGun();
                break;
            }
            else
                yield return wfs; 
        }
        
    }
    public GameObject GetNextBird()
    {
        camFollow.SetOriginState();
        GameObject obj = null;
        if (birdQueue.Count > 0)
        {
            obj = birdQueue.Dequeue();
            obj.SetActive(true);
        }
        return obj;
    }
}
