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

            // Allow the use of Undo (Ctrl+Z, Ctrl+Y).
            Undo.RegisterCreatedObjectUndo(gameObject, "");
        }
    }
    private void RefreshPalette()
    {
        palette.Clear();
        string[] prefabFiles = System.IO.Directory.GetFiles(path, "*.prefab");
        foreach (string prefabFile in prefabFiles)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject)) as GameObject;
            if (asset.tag.Contains("Obstacle") || asset.tag.Contains("Background"))
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
    private void OnGUI()
    {
        paintMode = GUILayout.Toggle(paintMode, "start painting", "button", GUILayout.Height(60f));

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

        if(GUILayout.Button("except necessary and clear scene "))
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
    void ClearSceneObjects()
    {
        List<GameObject> allRootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList();
        foreach (var obj in allRootObjects)
        {
            if (obj.name == "GameManager")
                continue;
            if (obj.name == "BirdGun")
                continue;
            if (obj.name == "Canvas")
                continue;
            if (obj.name == "EventSystem")
                continue;
            if (obj.name == "WorldRect")
                continue;
            if (obj.name == "BackGround")
                continue;
            if (obj.name == "Main Camera")
                continue;
            if (obj.name == "Ground")
                continue;
            if (obj.name == "WorldBounds")
                continue;
            DestroyImmediate(obj);
            Undo.DestroyObjectImmediate(obj);

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
           
            if (obstaclePrefabs[i].name == "Main Camera")
            {
                mainCamera = obstaclePrefabs[i];
                data.cameraSize = mainCamera.GetComponent<Camera>().orthographicSize;
            }
            else if (obstaclePrefabs[i].name == "BirdGun")
                birdGun = obstaclePrefabs[i];
            else if (obstaclePrefabs[i].name == "WorldRect")
                worldRect = obstaclePrefabs[i];
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
        string jsonPath = EditorUtility.OpenFilePanel("Json 파일을 Open할 경로", "Assets", ".json");
        string jsonString = File.ReadAllText(jsonPath);

        AngryBirdMapData data = new AngryBirdMapData();
        data.JsonToObject<AngryBirdMapData>(jsonString);
        data.PrintData();

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
