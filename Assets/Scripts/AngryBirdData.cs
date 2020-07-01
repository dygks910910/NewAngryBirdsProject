using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using YH_Class;
using YH_SingleTon;

namespace YH_SingleTon
{
    using YH_Data;

    public class DataManager : YH_SingleTon.Singleton<DataManager>
    {

        public AngryBirdMapData mapData = new AngryBirdMapData();
        public PlayerCommonData playerCommonData = new PlayerCommonData();
        public PlayerData playerData = new PlayerData();
        public string currentMapName;

        public void LoadMapData(string mapName)
        {
            mapData.Clear();
            playerCommonData.Clear();
            playerData.Clear();
            YH_SingleTon.YH_ObjectPool.Instance.LoadAllPrefabs();

            //현재 씬 비워주기.
            List<GameObject> allObjjectRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();
            currentMapName = mapName;
            for (int i = 0; i < allObjjectRoot.Count; ++i)
            {
                //Test
                if (allObjjectRoot[i].name == "TestCanvas")
                    continue;
                YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(allObjjectRoot[i]);
            }
            StringBuilder strbuilder = new StringBuilder();
            //json 파일 읽어오기..
            strbuilder.Append("Assets/Data/MapData/");
            strbuilder.Append(mapName);
            strbuilder.Append(".json");
            string jsonString = File.ReadAllText(strbuilder.ToString());
            mapData = mapData.JsonToObject<AngryBirdMapData>(jsonString);
            GameObject gameManager = null, mainCamera = null, birdGun = null, worldRect = null;
            GameObject obj;
            for (int i = 0; i < mapData.obstacleInfoList.Count; ++i)
            {
                if (mapData.obstacleInfoList[i].objectName == "GameManager")
                    continue;
                obj = YH_SingleTon.YH_ObjectPool.Instance.GetObj(mapData.obstacleInfoList[i].objectName);
                // obj = PrefabUtility.LoadPrefabContents(path);
                if (obj != null)
                {
                    if (mapData.obstacleInfoList[i].objectName == "GameManager")
                        gameManager = obj;
                    else if (mapData.obstacleInfoList[i].objectName == "Main Camera")
                        mainCamera = obj;
                    else if (mapData.obstacleInfoList[i].objectName == "BirdGun")
                        birdGun = obj;
                    else if (mapData.obstacleInfoList[i].objectName == "WorldRect")
                        worldRect = obj;
                    obj.transform.position = mapData.obstacleInfoList[i].objPosition;
                    obj.transform.rotation = mapData.obstacleInfoList[i].objRotation;
                    obj.transform.localScale = mapData.obstacleInfoList[i].objScale;
                    obj.isStatic = mapData.obstacleInfoList[i].isStatic;
                    obj.name = mapData.obstacleInfoList[i].objectName;
                }
            }
            if (!ConnectinginScripsInfo(mapData, gameManager, mainCamera, birdGun, worldRect))
            {
                //EditorUtility.DisplayDialog("오류", "data,camera,birdgun,worldRect 중 null이 있습니다", "확인");
                Debug.Log("ConnectinginScripsInfo fail");
            }


        }
        private bool ConnectinginScripsInfo(AngryBirdMapData data, GameObject gameManager, GameObject mainCamera,
       GameObject birdGun, GameObject wordRect)
        {
            if (data == null || mainCamera == null || birdGun == null || wordRect == null)
                return false;

            //camera 세팅
            //CamFollow fllow = mainCamera.GetComponent<CamFollow>();
            YH_SingleTon.CameraManager.Instance.originSize = data.cameraSize;
            YH_SingleTon.CameraManager.Instance.originPosition = data.obstacleInfoList.Find(d => d.objectName == "Main Camera").objPosition;

            //world Boundary세팅
            YH_SingleTon.WorldArea.Instance.worldRect = data.worldArea;
            YH_SingleTon.WorldArea.Instance.range = data.worldRange;


            //Init 순서 중요
            //YH_SingleTon.CameraManager.Instance.Init();
            YH_SingleTon.GameManager.Instance.Init();
            YH_SingleTon.StrapController.Instance.Init();
            YH_SingleTon.ScoreManager.Instance.Init();

            return true;
        }


    }
}
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

    public class Data
    {
        public string objectToJson()
        {
            return JsonUtility.ToJson(this);
        }
        public T JsonToObject<T>(string JsonData)
        {
            return JsonUtility.FromJson<T>(JsonData);
        }
    }

    [System.Serializable]
    public class AngryBirdMapData : Data
    {
        //모든 오브젝트
        public List<ObstacleInfo> obstacleInfoList = new List<ObstacleInfo>();
        //gameManager에 저장될 값.
        public List<string> birdInfoList = new List<string>();
        public float cameraSize;
        //WorldRect값.
        public Rect worldArea;
        public float worldRange;
        //3별 목표 score;
        public int threeStarScore;

        public void PrintData()
        {
            foreach (var obstacle in obstacleInfoList)
            {
                Debug.Log(obstacle.objectName);
                Debug.Log(obstacle.objPosition);
                Debug.Log(obstacle.objScale);
                Debug.Log(obstacle.objRotation);
                Debug.Log(obstacle.isStatic);
            }
        }
        public void Clear()
        {
            obstacleInfoList.Clear();
            birdInfoList.Clear();
        }
    }

    [System.Serializable]
    public class PlayerCommonData : Data
    {
        public float masterVolume;
        public void Clear()
        {
            masterVolume = 0;
        }
    }

    [System.Serializable]
    public class PlayerData : Data
    {
        public string lastCleardLevel;
        public Dictionary<string, int> stageHighscores = new Dictionary<string, int>();

        public int GetHighScore(string stageName)
        {
            if (stageHighscores.ContainsKey(stageName))
                return stageHighscores[stageName];
            else
                return 0;
        }
        public void SetHighScore(string stageName, int score)
        {
            if (stageHighscores.ContainsKey(stageName))
            {
                stageHighscores[stageName] = score;
            }
            else
            {
                stageHighscores.Add(stageName, score);
            }
        }
        public void Clear()
        {
            stageHighscores.Clear();
        }
    }

}
