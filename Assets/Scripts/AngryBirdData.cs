using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

namespace YH_Data
{
    [System.Serializable]
    public struct ObstacleInfo
    {
        public string objectName;
        public Vector3 objPosition;
        public Vector3 objScale;
        public Quaternion objRotation;
        public bool isStatic;
    }
    
    [System.Serializable]
    public class AngryBirdMapData
    {
        public List<ObstacleInfo> obstacleInfo = new List<ObstacleInfo>();

        public string objectToJson()
        {
            return JsonUtility.ToJson(this);
        }
        static public T JsonToObject<T>(string JsonData)
        {
            return JsonUtility.FromJson<T>(JsonData);
        }
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

        public void CreateAllObject()
        {
            

        }
    }

    [System.Serializable]
    public class AngryBirdPlayerData
    {



    }
}
