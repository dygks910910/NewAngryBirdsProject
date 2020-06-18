﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
        public Dictionary<string, GameObject> ObjectDic = new Dictionary<string, GameObject>();
        public void LoadAllPrefabs()
        {
            ObjectDic = Resources.LoadAll("Prefabs").ToDictionary(data => data.name, data=> data as GameObject);
        }

        public GameObject GetPrefab(string name)
        {
            return ObjectDic[name] ? ObjectDic[name] : null;
        }
    }

}
