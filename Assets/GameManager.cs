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
        public List<GameObject> birdList = new List<GameObject>();
        public float waitBirdOffset = 1.0f;


        private GameObject BirdGun;
        private GameObject mainCamera;
        private static WaitForSeconds CheckBirdColliderTerm = new WaitForSeconds(0.1f);
        private Queue<GameObject> birdQueue = new Queue<GameObject>();
        private WaitForSeconds PullBirdTerm = new WaitForSeconds(0.01f);
        private WaitForSeconds OneSecWait = new WaitForSeconds(1);
        private WaitForSeconds GameOverDelay = new WaitForSeconds(3);
        LinkedList<GameObject> pigList = new LinkedList<GameObject>();

        GameObject thisGameobj;
        // Start is called before the first frame update
        public void Init()
        {
            StopAllCoroutines();
            checkGameoverCorutine = null;
            YH_SingleTon.CameraManager.Instance.Init();
            YH_SingleTon.ScoreManager.Instance.Init();
            //StrapController.Instance.Init();
            if (birdList.Count > 0)
                birdList.Clear();
            if (birdQueue.Count > 0)
                birdQueue.Clear();
            mainCamera = GameObject.Find("Main Camera");
            BirdGun = GameObject.Find("BirdGun");

            //camFollow = mainCamera.GetComponent<CamFollow>();
            Vector3 gunPosition = BirdGun.transform.position;
            GameObject bird;
            var birdStrList =  YH_SingleTon.DataManager.Instance.mapData.birdInfoList;
            for (int i = 0; i < birdStrList.Count; ++i)
            {
                bird = YH_ObjectPool.Instance.GetObj(birdStrList[i]);
                bird.transform.position = new Vector3(BirdGun.transform.position.x + (waitBirdOffset * i + 1), 0, 0);
                bird.GetComponentInChildren<Rigidbody2D>().isKinematic = true;
                bird.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
                birdQueue.Enqueue(bird);
                birdList.Add(bird);

            }
            //ReloadBurdGun();
            GameObject[] pigs = GameObject.FindGameObjectsWithTag("Pig");
            GameObject tmpObj;
            for(int i = 0; i < pigs.Length; ++i)
            {
                tmpObj = pigs[i];
                tmpObj.GetComponent<PigInteraction>().pigDieEvtHandle += DeletePig;
                pigList.AddLast(tmpObj);
            }
            if(gameObject.activeInHierarchy && checkGameoverCorutine == null)
                checkGameoverCorutine =StartCoroutine(CheckGameoverState());
        }
        Coroutine checkGameoverCorutine;
        private void OnDisable()
        {
            StopCoroutine(CheckGameoverState());
        }
        IEnumerator CheckGameoverState()
        {
            while(true)
            {
                yield return GameOverDelay;
                if (CheckGameOver())
                {
                    yield return GameOverDelay;

                    GameObject tmpBirdObj;
                    BirdAnimationChanger tmpScript;
                    for (int i = 0; i < birdList.Count; ++i)
                    {
                        tmpBirdObj = birdList[i];
                        tmpScript = tmpBirdObj.GetComponent<BirdAnimationChanger>();
                        YH_Helper.YH_Helper.Create3DScore(10000, tmpBirdObj.transform.position, tmpScript.birdColor);
                        yield return OneSecWait;
                    }
                    YH_SingleTon.MainUIController.Instance.SetScorePanelEnable(true);

                    break;
                }
            }
            checkGameoverCorutine = null;
        }
        
        //strapController에서 실행되는 delegate용도.
        public void DeleteBird(GameObject bird)
        {
            birdList.Remove(bird);
        }
        //return val 이 null일 경우에 새가 없음.
        public void ReloadBurdGun()
        {
            GameObject nextBird = GetNextBird();
            if (nextBird != null)
            {
                //birdList.Remove(nextBird);
                //슈팅을 하게 되면 remove수행함.
                YH_SingleTon.StrapController.Instance.shotingEventHandler += DeleteBird;
                if(gameObject.activeInHierarchy)
                    StartCoroutine(CheckBirdCollided(nextBird.GetComponentInChildren<BirdAnimationChanger>()));
                //첫번쨰 새는 새총위에 장전되있기 때문에 수행하지않음.따라서 i는 1부터.
                for (int i = 1; i < birdList.Count; ++i)
                {
                    if(gameObject.activeInHierarchy)
                        StartCoroutine(PullbirdOne(birdList[i]));
                }
                YH_SingleTon.StrapController.Instance.ReloadBirds(nextBird);

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
        public void DeletePig(GameObject pig, ref PigInteraction.PigDieProcessing evtHandle)
        {
            pigList.Remove(pig);
            evtHandle -= DeletePig;
        }
        public GameObject GetNextBird()
        {
            //YH_SingleTon.CameraManager.Instance.SetOriginState();
            GameObject obj = null;
            if (birdQueue.Count > 0)
            {
                obj = birdQueue.Dequeue();
                obj.SetActive(true);
            }
            return obj;
        }
        bool CheckGameOver()
        {
            if (birdQueue.Count < 0)
                return true;
            if (pigList.Count <= 0)
                return true;
            if (birdList.Count == 0 && pigList.Count > 0)
                return true;
            return false;
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
