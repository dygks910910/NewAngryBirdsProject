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
using UnityEngine.UI;
using UnityEditor.Animations;

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
        if (obstaclePrefabs != null && obstaclePrefabs.Count > 0)
            obstaclePrefabs.Clear();

        allObjects = GameObject.FindObjectsOfType<GameObject>().ToList();
        allObjjectRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();

        //데이터가 프리팹이며 장애물일경우 모두 저장.
        //obstaclePrefabs = allObjjectRoot.Where(data => PrefabUtility.GetCorrespondingObjectFromSource(data) != null &&
        //        PrefabUtility.GetPrefabInstanceHandle(data.transform) != null && 
        //        (data.CompareTag("IceObstacle") || data.CompareTag("StoneObstacle") ||
        //        data.CompareTag("WoodObstacle"))).ToList();
        obstaclePrefabs = allObjjectRoot.Where(data => PrefabUtility.GetCorrespondingObjectFromSource(data) != null &&
               PrefabUtility.GetPrefabInstanceHandle(data.transform) != null).ToList();

        mainCamera = GameObject.Find("Main Camera");
        worldRect = GameObject.Find("WorldRect");
        gameManager = GameObject.Find("GameManager");


    }
    static public List<GameObject> allObjects;
    static public List< GameObject> allObjjectRoot;
    static public List<GameObject> obstaclePrefabs;
    
    static GameObject mainCamera;
    static GameObject worldRect;
    static GameObject gameManager;

    private bool paintMode = false;
    private int threeStarScore = 0;
    Vector2 scollPos;
    private Vector2 cellSize = new Vector2(0.5f, 0.5f);
    [SerializeField]
    private List<GameObject> palette = new List<GameObject>();
    private string path = "Assets/Resources/Prefabs/CachingPrefabs/";
    private string pathOnece = "Assets/Resources/Prefabs/ChchingOnecePrefabs/";

    [SerializeField]
    private int paletteIndex;
    Vector2 palleteScollPos;
    private void OnSceneGUI(SceneView sceneView)
    {
        if (paintMode)
        {
            Vector2 cellCenter = GetSelectedCell(); // Refactoring, I moved some code in this function


            DisplayVisualHelp(cellCenter);
            HandleSceneViewInputs(cellCenter);

            // Refresh the view
            sceneView.Repaint();
        }
    }
    private Vector2 GetSelectedCell()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);
            // Get the corresponding cell on our virtual grid
            Vector2Int cell = new Vector2Int(Mathf.RoundToInt(mousePosition.x / cellSize.x), Mathf.RoundToInt(mousePosition.y / cellSize.y));
            return cell * cellSize;
        }
        return Vector2.zero;
    }
    private void HandleSceneViewInputs(Vector2 cellCenter)
    {
        // Filter the left click so that we can't select objects in the scene
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0); // Consume the event
        }
        // We have a prefab selected and we are clicking in the scene view with the left button
        if (paletteIndex < palette.Count && Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            // Create the prefab instance while keeping the prefab link
            GameObject prefab = palette[paletteIndex];
            GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            gameObject.transform.position = cellCenter;
            if(rotation90)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            // Allow the use of Undo (Ctrl+Z, Ctrl+Y).
            Undo.RegisterCreatedObjectUndo(gameObject, "");
        }
    }
    private void RefreshPalette()
    {
        palette.Clear();
        List<string> prefabFile;
        prefabFile = System.IO.Directory.GetFiles(path, "*.prefab").ToList();
        prefabFile.AddRange(System.IO.Directory.GetFiles(pathOnece, "*.prefab"));
        foreach (string prefab in prefabFile)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject)) as GameObject;
            if (asset.tag.Contains("Obstacle") || asset.tag.Contains("nessasary") || asset.tag.Contains("Pig"))
            {
                palette.Add(asset);
            }

        }
    }
    private void DisplayVisualHelp(Vector2 cellCenter)
    {
        // Get the mouse position in world space such as z = 0
        Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

        // Get the corresponding cell on our virtual grid
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(mousePosition.x / cellSize.x), Mathf.RoundToInt(mousePosition.y / cellSize.y));
        cellCenter = cell * cellSize;
        // Vertices of our square
        Vector3 topLeft = cellCenter + Vector2.left * cellSize * 0.5f + Vector2.up * cellSize * 0.5f;
        Vector3 topRight = cellCenter - Vector2.left * cellSize * 0.5f + Vector2.up * cellSize * 0.5f;
        Vector3 bottomLeft = cellCenter + Vector2.left * cellSize * 0.5f - Vector2.up * cellSize * 0.5f;
        Vector3 bottomRight = cellCenter - Vector2.left * cellSize * 0.5f - Vector2.up * cellSize * 0.5f;

        // Rendering
        Handles.color = Color.green;
        Vector3[] lines = { topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft, topLeft };
        Handles.DrawLines(lines);
    }
    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
        RefreshPalette();
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
    private bool rotation90 = false;
    private void OnGUI()
    {
        paintMode = GUILayout.Toggle(paintMode, "start painting", "button", GUILayout.Height(60f));

        rotation90 = GUILayout.Toggle(rotation90, "rotation90");
        // Get a list of previews, one for each of our prefabs
        List<GUIContent> paletteIcons = new List<GUIContent>();
        foreach (GameObject prefab in palette)
        {
            // Get a preview for the prefab
            Texture2D texture = AssetPreview.GetAssetPreview(prefab);
            
            paletteIcons.Add(new GUIContent(prefab.name,texture));
            
        }
        palleteScollPos = GUILayout.BeginScrollView(palleteScollPos);
        // Display the grid
        paletteIndex = GUILayout.SelectionGrid(paletteIndex, paletteIcons.ToArray(), 4,GUILayout.MaxWidth(500));
        GUILayout.EndScrollView();

        if (obstaclePrefabs != null)
        {
            scollPos = GUILayout.BeginScrollView(scollPos);
            for (int i = 0; i < obstaclePrefabs.Count; ++i)
            {
                if(obstaclePrefabs != null)
                    YH_CustomEditor.CustomWndHelper.CreateLabel("Obstacle", obstaclePrefabs[i].name);
            }
            GUILayout.Space(10);
           

            GUILayout.EndScrollView();
        }
        YH_CustomEditor.CustomWndHelper.CreateIntField("목표 3별 점수", ref threeStarScore);

        // "target" can be any class derrived from ScriptableObject 
        // (could be EditorWindow, MonoBehaviour, etc)
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("birdList");
        EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties

        if (GUILayout.Button("Create Necessary Objs"))
        {
            CreateNecessaryObject();
        }
        if (GUILayout.Button("clear scene "))
        {
            ClearSceneObjects();
        }
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
    void CreateNecessaryObject()
    {
        List<GameObject> prefabList = new List<GameObject>();
        //birdGun,canvas,eventSystem,worldRect,WorldBounds
        prefabList.Add(AssetDatabase.LoadAssetAtPath(pathOnece + "Canvas.prefab", typeof(GameObject)) as GameObject);
        prefabList.Add(AssetDatabase.LoadAssetAtPath(pathOnece + "BirdGun.prefab", typeof(GameObject)) as GameObject);
        prefabList.Add(AssetDatabase.LoadAssetAtPath(pathOnece + "EventSystem.prefab", typeof(GameObject)) as GameObject);
        prefabList.Add(AssetDatabase.LoadAssetAtPath(pathOnece + "WorldBounds.prefab", typeof(GameObject)) as GameObject);
        prefabList.Add(AssetDatabase.LoadAssetAtPath(pathOnece + "WorldRect.prefab", typeof(GameObject)) as GameObject);
        prefabList.Add(AssetDatabase.LoadAssetAtPath(pathOnece + "Main Camera.prefab", typeof(GameObject)) as GameObject);
        foreach (var element in prefabList)
        {
            PrefabUtility.InstantiatePrefab(element);
            Undo.RegisterCreatedObjectUndo(element,"");
        }

    }
    void ClearSceneObjects()
    {
        List<GameObject> allRootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();
        foreach (var obj in allRootObjects)
        {
            Undo.DestroyObjectImmediate(obj);
            DestroyImmediate(obj);

        }
    }
    private void SaveMapFile()
    {
        string path = EditorUtility.SaveFilePanel("Json 파일을 Save할 경로", "Assets", "", "json");
        if (path == "")
            return;
        AngryBirdMapData data = new AngryBirdMapData();
        ObstacleInfo info = new ObstacleInfo();

        GameObject mainCamera = null, birdGun = null,worldRect = null;
        for (int i = 0; i < obstaclePrefabs.Count; ++i)
        {
           //추가로 필요한 Component데이터를 쓰기위한 캐싱.
            if (obstaclePrefabs[i].name == "Main Camera")
            {
                mainCamera = obstaclePrefabs[i];
                data.cameraSize = mainCamera.GetComponent<Camera>().orthographicSize;
            }
            else if (obstaclePrefabs[i].name == "BirdGun")
                birdGun = obstaclePrefabs[i];
            else if (obstaclePrefabs[i].name == "WorldRect")
                worldRect = obstaclePrefabs[i];
            //장애물 add
            info.objectName = obstaclePrefabs[i].name;
            info.objPosition = obstaclePrefabs[i].transform.position;
            info.objRotation = obstaclePrefabs[i].transform.rotation;
            info.objScale = obstaclePrefabs[i].transform.localScale;
            info.isStatic = obstaclePrefabs[i].isStatic;
            data.obstacleInfoList.Add(info);
        }
        //bird 들 세팅.
        //GameManager manager = gameManager.GetComponent<GameManager>();
        data.birdInfoList = birdList.Select(n=>n.name).ToList();

        //worldRect값 세팅.
        WorldArea area = worldRect.GetComponent<WorldArea>();
        data.worldArea = area.worldRect;
        data.worldRange = area.range;

        data.threeStarScore = threeStarScore;
        Debug.Log(data.objectToJson());
        File.WriteAllText(path, data.objectToJson());

        EditorUtility.DisplayDialog("저장경로 확인", path + "에 저장 완료하였습니다", "확인");
    }

    private void LoadMapFile()
    {
        string jsonPath = EditorUtility.OpenFilePanel("Json 파일을 Open할 경로", "Assets/Data/MapData/", "json");
        string jsonString = File.ReadAllText(jsonPath);

        AngryBirdMapData data = new AngryBirdMapData();
        data = data.JsonToObject<AngryBirdMapData>(jsonString);
        data.PrintData();
        threeStarScore = data.threeStarScore;
        if (birdList.Count > 0)
            birdList.Clear();
        foreach(var bird in data.birdInfoList)
        {
            birdList.Add(FindPrefab(bird));
        }
        //Resources.LoadAll<GameObject>("Prefabs/CachingOnecePrefabs");

        //ObstacleInfo info = new ObstacleInfo();

        //mainCamera,BirdGun,GameManager를 캐싱.
        GameObject gameManager = null, mainCamera = null, birdGun = null, worldRect = null;

        GameObject obj;
        foreach (var obstacle in data.obstacleInfoList)
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
        //if (!ConnectinginScripsInfo(data, gameManager, mainCamera, birdGun, worldRect))
        //{
        //    EditorUtility.DisplayDialog("오류", "data,camera,birdgun,worldRect 중 null이 있습니다", "확인");
        //}

        //birdGunController.gameManager = manager;
    }
    //private bool ConnectinginScripsInfo(AngryBirdMapData data,GameObject gameManager,GameObject mainCamera,
    //    GameObject birdGun,GameObject wordRect)
    //{
    //    if (data == null || gameManager == null || mainCamera == null || birdGun == null || wordRect == null)
    //        return false;
    //    GameObject tmpObj;
    //    //bird 들 세팅.
    //    GameManager manager = gameManager.GetComponent<GameManager>();
    //    //기존 prefavb에 있던 자료 클리어.
    //    manager.birdList.Clear();
    //    manager.BirdGun = birdGun;
    //    manager.mainCamera = mainCamera;
    //    for (int i = 0; i < data.birdInfoList.Count; ++i)
    //    {
    //        tmpObj = FindPrefab(data.birdInfoList[i]);
    //        manager.birdList.Add(tmpObj);
    //    }
    //    //world Boundary세팅
    //    WorldArea area = wordRect.GetComponent<WorldArea>();
    //    area.worldRect = data.worldArea;
    //    area.range = data.worldRange;

    //    //camera 세팅
    //    CamFollow fllow = mainCamera.GetComponent<CamFollow>();
    //    fllow.WorldRect = wordRect;

    //    //birdGun Setting
    //    StrapController birdGunController = birdGun.GetComponent<StrapController>();
    //    birdGunController.mainCamera = mainCamera.GetComponent<Camera>();
    //    return true;
    //}
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
