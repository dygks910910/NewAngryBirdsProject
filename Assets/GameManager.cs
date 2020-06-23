using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {
       YH_SingleTon.YH_ObjectPool.Instance.LoadAllPrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
