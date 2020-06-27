using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

namespace YH_Data
{
    public class DataManager : YH_SingleTon.Singleton<DataManager>
    {
        public AngryBirdMapData mapData = new AngryBirdMapData();
        public PlayerCommonData playerCommonData = new PlayerCommonData();
    
    }




    [System.Serializable]
    public struct ObstacleInfo
    {
        public string objectName;
        public Vector3 objPosition;
        public Vector3 objScale;
        public Quaternion objRotation;
        public bool isStatic;
    }

    public class Data
    {
        public string objectToJson()
        {
            return JsonUtility.ToJson(this);
        }
        static public T JsonToObject<T>(string JsonData)
        {
            return JsonUtility.FromJson<T>(JsonData);
        }
    }

    [System.Serializable]
    public class AngryBirdMapData : Data
    {
        //모든 오브젝트
        public List<ObstacleInfo> obstacleInfo = new List<ObstacleInfo>();
        //gameManager에 저장될 값.
        public List<string> birdInfo = new List<string>();
        
        //WorldRect값.
        public Rect worldArea;
        public float worldRange;
        public void PrintData()
        {
            foreach(var obstacle in obstacleInfo)
            {
                Debug.Log(obstacle.objectName);
                Debug.Log(obstacle.objPosition);
                Debug.Log(obstacle.objScale);
                Debug.Log(obstacle.objRotation);
                Debug.Log(obstacle.isStatic);
            }
        }
    }

    [System.Serializable]
    public class PlayerCommonData : Data
    {
        public float masterVolume;
    }

}
