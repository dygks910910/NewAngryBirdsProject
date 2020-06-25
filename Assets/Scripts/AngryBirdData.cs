using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YH_Data
{
    [System.Serializable]
    public struct ObstacleInfo
    {
        string objectName;
        Vector3 objPosition;
        Vector3 objScale;
        Quaternion objRotation;
    }
    [System.Serializable]
    public struct StaticObject
    {
        string BackgroundName;
        Vector3 objPosition;
        Vector3 objScale;
        Quaternion objRotation;
    }
    [System.Serializable]
    public class AngryBirdMapData
    {
        List<ObstacleInfo> obstacleInfo;
        List<StaticObject> staticObjects;
    }

    [System.Serializable]
    public class AngryBirdPlayerData
    {



    }
}
