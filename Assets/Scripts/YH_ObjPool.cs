using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace YH_SingleTon
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    { // Destroy 여부 확인용
        private static bool _ShuttingDown = false;
        private static object _Lock = new object();
        private static T _Instance;
        public static T Instance
        {
            get
            {
                // 게임 종료 시 Object 보다 싱글톤의 OnDestroy 가 먼저 실행 될 수도 있다. 
                // 해당 싱글톤을 gameObject.Ondestory() 에서는 사용하지 않거나 사용한다면 null 체크를 해주자
                if (_ShuttingDown)
                {
                    Debug.Log("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                    return null;
                }
                lock (_Lock) //Thread Safe 
                {
                    if (_Instance == null)
                    {
                        // 인스턴스 존재 여부 확인 
                        _Instance = (T)FindObjectOfType(typeof(T));
                        // 아직 생성되지 않았다면 인스턴스 생성
                        if (_Instance == null)
                        {
                            // 새로운 게임오브젝트를 만들어서 싱글톤 Attach 
                            var singletonObject = new GameObject();
                            _Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";
                            // Make instance persistent.
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                    return _Instance;
                }
            }
        }
        private void OnApplicationQuit()
        {
            _ShuttingDown = true;
        }
        private void OnDestroy()
        {
            _ShuttingDown = true;
        }
    }
    class YH_ObjectPool : Singleton<YH_ObjectPool>
    {
        public Dictionary<string, Queue<GameObject>> ObjectDic = new Dictionary<string, Queue<GameObject>>();
        //10개 미리 생성.
        private const uint MAX_THREASHHOLD = 10;
        //큐가 비어있을떄 새로 생성할 갯수.
        private const uint MIN_THREASHHOLD = 5;
        GameObject baseObject;
        public void LoadAllPrefabs()
        {
            baseObject = new GameObject("ObjectPool");
            //ObjectDic = Resources.LoadAll("Prefabs").ToDictionary(data => data.name, data=> data as GameObject);
            GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs");
            FillQueue(prefabs, MAX_THREASHHOLD);
        }

        //실패시 null 리턴.
        public GameObject GetObj(string name)
        {
            if(ObjectDic[name].Count > 1)
            {
                GameObject obj = ObjectDic[name].Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else if(ObjectDic[name].Count == 1)
            {
                for(int i =0; i < MIN_THREASHHOLD; ++i)
                {
                    ObjectDic[name].Enqueue(Instantiate(ObjectDic[name].Peek()));
                }
                return GetObj(name);
            }
            return null;
        }
        public void GiveBackObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.parent = baseObject.transform;
            ObjectDic[obj.name].Enqueue(obj);
        }
        private void FillQueue(GameObject[] objs,uint fillCount)
        {
            GameObject tmpObj;
            for (int i = 0; i < objs.Length; ++i)
            {
                ObjectDic.Add(objs[i].name, new Queue<GameObject>());
                for (int genCount = 0; genCount < MAX_THREASHHOLD; ++genCount)
                {
                    tmpObj = Instantiate(objs[i]);
                    tmpObj.SetActive(false);
                    tmpObj.transform.parent = baseObject.transform;
                    //(Clone)제거
                    tmpObj.name = objs[i].name;
                    ObjectDic[objs[i].name].Enqueue(tmpObj);
                }

            }
        }
    }

}
