using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using TMPro.EditorUtilities;
using UnityEditor;
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
            if (ObjectDic.Count > 0)
                return;
            baseObject = new GameObject("ObjectPool");
            //ObjectDic = Resources.LoadAll("Prefabs").ToDictionary(data => data.name, data=> data as GameObject);
            GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/CachingPrefabs");
            FillQueue(prefabs, MAX_THREASHHOLD);
            GameObject[] prefabsOnlyOne = Resources.LoadAll<GameObject>("Prefabs/ChchingOnecePrefabs");
            FillQueue(prefabsOnlyOne, 1);
        }
        //실패시 null 리턴.
        public GameObject GetObj(string name)
        {
            
            if(ObjectDic[name].Count > 1)
            {
                GameObject obj = ObjectDic[name].Dequeue();
                obj.transform.SetParent(null);
                obj.SetActive(true);
                Debug.Log("getObj " + name);
                return obj;
            }
            else if(ObjectDic[name].Count == 1)
            {
                GameObject tmp;
                for(int i =0; i < MIN_THREASHHOLD; ++i)
                {
                    tmp = CreateObject(ObjectDic[name].Peek());
                    ObjectDic[name].Enqueue(tmp);
                }
                return GetObj(name);
            }
            return null;
        }
        public GameObject GetObj(string name,Vector3 pos)
        {
            GameObject obj = GetObj(name);
            obj.transform.position = pos;
            return obj;
        }

        public void GiveBackObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(baseObject.transform);

            if(!ObjectDic.ContainsKey(obj.name))
            {
                ObjectDic.Add(obj.name, new Queue<GameObject>());
            }
            ObjectDic[obj.name].Enqueue(obj);
            Debug.Log("giveBackObj " + obj.name);
        }
        private void FillQueue(GameObject[] objs,uint fillCount)
        {
            GameObject tmpObj;
            for (int i = 0; i < objs.Length; ++i)
            {
                if(ObjectDic.ContainsKey(objs[i].name) == false)
                     ObjectDic.Add(objs[i].name, new Queue<GameObject>());
                for (int genCount = 0; genCount < fillCount; ++genCount)
                {
                    tmpObj = CreateObject(objs[i]);
                    ObjectDic[objs[i].name].Enqueue(tmpObj);
                }

            }
        }
        private GameObject CreateObject(GameObject srcobj)
        {
            GameObject tmpObj;
            tmpObj = Instantiate(srcobj);
            tmpObj.SetActive(false);
            tmpObj.transform.SetParent(baseObject.transform);
            //(Clone)제거
            tmpObj.name = srcobj.name;
            return tmpObj;
        }
    }

}
