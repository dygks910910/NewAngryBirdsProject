using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using YH_Data;
using System.IO;
using YH_Class;
using JetBrains.Annotations;
using TMPro.EditorUtilities;
using YH_SingleTon;
using System;

public class MapDataManagerWindow : EditorWindow
{
    [Tooltip("사용할 새 세팅")]
    public List<GameObject> birdList = new List<GameObject>();
    Editor editor;
    [MenuItem("YH_Custom/MapDataManager")]
    static void Init()
    {
        MapDataManagerWindow wnd = EditorWindow.CreateWindow<MapDataManagerWindow>("MapDataManager");
        wnd.Show();
        LoadSceneData();
    }
    static void LoadSceneData()
    {
        if (allObjects != null&& allObjects.Count > 0)
            allObjects.Clear();
        if (allObjjectRoot != null && allObjjectRoot.Count > 0)
            allObjjectRoot.Clear();
        if (prefabs != null && prefabs.Count > 0)
            prefabs.Clear();

        allObjects = GameObject.FindObjectsOfType<GameObject>().ToList();
        allObjjectRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();

        prefabs = allObjjectRoot.Where(data => PrefabUtility.GetCorrespondingObjectFromSource(data) != null &&
                PrefabUtility.GetPrefabInstanceHandle(data.transform) != null).ToList();
    }
    static public List<GameObject> allObjects;
    static public List< GameObject> allObjjectRoot;
    static public List<GameObject> prefabs;
    Vector2 scollPos;
    private void OnGUI()
    {

        if (prefabs != null)
        {
            scollPos = GUILayout.BeginScrollView(scollPos);
            for (int i = 0; i < prefabs.Count; ++i)
            {
                YH_CustomEditor.CustomWndHelper.CreateLabel("prefabs", prefabs[i].name);
            }
            GUILayout.Space(10);
            if(allObjects != null)
            for (int i = 0; i < allObjects.Count; ++i)
            {
                YH_CustomEditor.CustomWndHelper.CreateLabel("object", allObjects[i].name);
            }
            GUILayout.EndScrollView();
        }

        // "target" can be any class derrived from ScriptableObject 
        // (could be EditorWindow, MonoBehaviour, etc)
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("birdList");
        EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties


        if (GUILayout.Button("Reload Scene Data"))
        {
            LoadSceneData();
        }
        if (GUILayout.Button("Generate Map File"))
        {
            SaveMapFile();
        }
        if (GUILayout.Button("Load Map File"))
        {
            LoadMapFile();
        }




    }
    private void SaveMapFile()
    {
        string path = EditorUtility.SaveFilePanel("Json 파일을 Save할 경로", "Assets", "", ".json");
        if (path == "")
            return;
        AngryBirdMapData data = new AngryBirdMapData();
        ObstacleInfo info = new ObstacleInfo();

        GameObject mainCamera = null, birdGun = null,worldRect = null;
        for (int i = 0; i < prefabs.Count; ++i)
        {
           
            if (prefabs[i].name == "Main Camera")
                mainCamera = prefabs[i];
            else if (prefabs[i].name == "BirdGun")
                birdGun = prefabs[i];
            else if (prefabs[i].name == "WorldRect")
                worldRect = prefabs[i];
            info.objectName = prefabs[i].name;
            info.objPosition = prefabs[i].transform.position;
            info.objRotation = prefabs[i].transform.rotation;
            info.objScale = prefabs[i].transform.localScale;
            info.isStatic = prefabs[i].isStatic;
            data.obstacleInfo.Add(info);
        }
        //bird 들 세팅.
        //GameManager manager = gameManager.GetComponent<GameManager>();
        data.birdInfo = birdList.Select(n=>n.name).ToList();

        //worldRect값 세팅.
        WorldArea area = worldRect.GetComponent<WorldArea>();
        data.worldArea = area.worldRect;
        data.worldRange = area.range;


        Debug.Log(data.objectToJson());
        File.WriteAllText(path, data.objectToJson());

        EditorUtility.DisplayDialog("저장경로 확인", path + "에 저장 완료하였습니다", "확인");
    }

    private void LoadMapFile()
    {
        string jsonPath = EditorUtility.OpenFilePanel("Json 파일을 Open할 경로", "Assets", ".json");
        string jsonString = File.ReadAllText(jsonPath);

        AngryBirdMapData data = new AngryBirdMapData();
        data = AngryBirdMapData.JsonToObject<AngryBirdMapData>(jsonString);
        data.PrintData();

        //Resources.LoadAll<GameObject>("Prefabs/CachingOnecePrefabs");

        //ObstacleInfo info = new ObstacleInfo();

        //mainCamera,BirdGun,GameManager를 캐싱.
        GameObject gameManager = null, mainCamera = null, birdGun = null, worldRect = null;

        GameObject obj;
        foreach (var obstacle in data.obstacleInfo)
        {
            obj = FindPrefab(obstacle);
            

            // obj = PrefabUtility.LoadPrefabContents(path);
            if (obj != null)
            {
                obj = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                if (obstacle.objectName == "GameManager")
                    gameManager = obj;
                else if (obstacle.objectName == "Main Camera")
                    mainCamera = obj;
                else if (obstacle.objectName == "BirdGun")
                    birdGun = obj;
                else if (obstacle.objectName == "WorldRect")
                    worldRect = obj;
                obj.transform.position = obstacle.objPosition;
                obj.transform.rotation = obstacle.objRotation;
                obj.transform.localScale = obstacle.objScale;
                obj.isStatic = obstacle.isStatic;
                obj.name = obstacle.objectName;
            }
        }
        if(!ConnectinginScripsInfo(data, gameManager, mainCamera, birdGun, worldRect))
        {
            EditorUtility.DisplayDialog("오류", "data,camera,birdgun,worldRect 중 null이 있습니다","확인");
        }

        //birdGunController.gameManager = manager;
    }
    private bool ConnectinginScripsInfo(AngryBirdMapData data,GameObject gameManager,GameObject mainCamera,
        GameObject birdGun,GameObject wordRect)
    {
        if (data == null || gameManager == null || mainCamera == null || birdGun == null || wordRect == null)
            return false;
        GameObject tmpObj;
        //bird 들 세팅.
        GameManager manager = gameManager.GetComponent<GameManager>();
        //기존 prefavb에 있던 자료 클리어.
        manager.birdList.Clear();
        manager.BirdGun = birdGun;
        manager.mainCamera = mainCamera;
        for (int i = 0; i < data.birdInfo.Count; ++i)
        {
            tmpObj = FindPrefab(data.birdInfo[i]);
            manager.birdList.Add(tmpObj);
        }
        //world Boundary세팅
        WorldArea area = wordRect.GetComponent<WorldArea>();
        area.worldRect = data.worldArea;
        area.range = data.worldRange;

        //camera 세팅
        CamFollow fllow = mainCamera.GetComponent<CamFollow>();
        fllow.WorldRect = wordRect;

        //birdGun Setting
        StrapController birdGunController = birdGun.GetComponent<StrapController>();
        birdGunController.mainCamera = mainCamera.GetComponent<Camera>();
        return true;
    }
    private GameObject FindPrefab(ObstacleInfo info)
    {
        return FindPrefab(info.objectName);
    }
    private GameObject FindPrefab(string name)
    {
        GameObject obj;
        string path = "Assets/Resources/Prefabs/CachingPrefabs/" + name + ".prefab";
        //string path = AssetDatabase.GetAssetPath();

        obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (obj == null)
        {
            path = "Assets/Resources/Prefabs/ChchingOnecePrefabs/" + name + ".prefab";
            obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        return obj;
    }


}
