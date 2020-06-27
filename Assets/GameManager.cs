using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YH_Class;
using YH_SingleTon;

namespace YH_SingleTon
{
    public class GameManager : Singleton<GameManager>
    {
        [Tooltip("새들을 저장하는 순서.순서대로 장전된다.")]
        public List<GameObject> birdList;
        public GameObject BirdGun;
        public GameObject mainCamera;
        public Text scoreText;
        public float waitBirdOffset = 0;
        
        public delegate int OnHitObject(int score,GameObject hitedObj);
        public event OnHitObject OnchangedScore;
        
        private CamFollow camFollow;
        private static WaitForSeconds CheckBirdColliderTerm = new WaitForSeconds(0.1f);
        private StrapController birdGunController;
        private Queue<GameObject> birdQueue = new Queue<GameObject>();
        private WaitForSeconds PullBirdTerm = new WaitForSeconds(0.01f);

        
        // Start is called before the first frame update
        void Start()
        {
            YH_SingleTon.YH_ObjectPool.Instance.LoadAllPrefabs();
            birdGunController = BirdGun.GetComponent<StrapController>();
            camFollow = mainCamera.GetComponent<CamFollow>();
            GameObject bird;
            Vector3 gunPosition = BirdGun.transform.position;
            for (int i = 0; i < birdList.Count; ++i)
            {
                bird = YH_SingleTon.YH_ObjectPool.Instance.GetObj(birdList[i].name);
                //bird.SetActive(false);
                birdQueue.Enqueue(bird);
                birdList[i] = bird;
                bird.transform.Translate(BirdGun.transform.position.x + (waitBirdOffset * i + 1), 0, 0);
                bird.GetComponentInChildren<Rigidbody2D>().isKinematic = true;
                bird.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
                
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
            if (nextBird != null)
            {
                birdList.Remove(nextBird);
                StartCoroutine(CheckBirdCollided(nextBird.GetComponentInChildren<BirdAnimationChanger>()));
                for (int i = 0; i < birdList.Count; ++i)
                {
                    StartCoroutine(PullbirdOne(birdList[i]));
                }
                birdGunController.ReloadBirds(nextBird);

            }
            else
            {
                //null이면 게임 종료.결과창 출력.
            }

        }
        IEnumerator CheckBirdCollided(BirdAnimationChanger changer)
        {
            while (true)
            {
                if (changer.birdState == eBirdState.COLLIDED || !(changer.gameObject.activeSelf))
                {
                    ReloadBurdGun();
                    break;
                }
                else
                    yield return CheckBirdColliderTerm;
            }
        }
        IEnumerator PullbirdOne(GameObject bird)
        {
            Vector3 newPos = bird.transform.position;
            newPos.x -= waitBirdOffset;
            for (int i = 0; i < 10; ++i)
            {
                bird.transform.position = Vector3.Lerp(bird.transform.position, newPos, 0.1f * i);
                yield return PullBirdTerm;
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

        private void LoadLevel()
        { 
        }
    }
    //[CustomEditor(typeof(GameManager))]
    //[CanEditMultipleObjects]
    //public class GameManagerEditor : Editor
    //{
    //    SerializedProperty linkedList;

    //    int size;
    //    void OnEnable()
    //    {
    //        linkedList = serializedObject.FindProperty("birdList");
    //    }
    //    public override void OnInspectorGUI()
    //    {
    //        //YH_CustomEditor.CustomWndHelper.CreateLabel("size",)
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.LabelField("size");
    //        linkedList.arraySize = EditorGUILayout.IntField(linkedList.arraySize);
    //        EditorGUILayout.EndHorizontal();
            
    //    }
    //}

}
