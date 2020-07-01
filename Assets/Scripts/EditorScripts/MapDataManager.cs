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
    [Tooltip("사용할 Bird 세팅")]
    public List<GameObject> birdList = new List<GameObject>();

    #region static 변수.
    static public List<GameObject> allObjects;
    static public List<GameObject> allObjjectRoot;
    static public List<GameObject> obstaclePrefabs;

    static GameObject mainCamera;
    static GameObject worldRect;
    static GameObject gameManager;
    #endregion
    [MenuItem("YH_Custom/MapDataManager")]
    static void CreateWindow()
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

        //데이터가 prefab인것만 읽어오기.
        obstaclePrefabs = allObjjectRoot.Where(data => PrefabUtility.GetCorrespondingObjectFromSource(data) != null &&
               PrefabUtility.GetPrefabInstanceHandle(data.transform) != null).ToList();
        //component에 export할 값이 있어 캐싱.
        mainCamera = GameObject.Find("Main Camera");
        worldRect = GameObject.Find("WorldRect");
        gameManager = GameObject.Find("GameManager");
    }

    
    

    private bool paintMode = false;
    private int threeStarScore = 0;
    Vector2 scollPos;
    private Vector2 cellSize = new Vector2(0.5f, 0.5f);
    [SerializeField]
    private List<GameObject> palette = new List<GameObject>();
    private bool rotation90 = false;

    #region constant value
    private string path = "Assets/Resources/Prefabs/CachingPrefabs/";
    private string pathOnece = "Assets/Resources/Prefabs/ChchingOnecePrefabs/";
    #endregion

    [SerializeField]
    private int paletteIndex;
    Vector2 palleteScollPos;
    #region SceneView Func
    private void OnSceneGUI(SceneView sceneView)
    {
        if (paintMode)
        {
            Vector2 cellCenter = GetSelectedCell(); // Refactoring, I moved some code in this function
            DisplayVisualHelp(cellCenter);
            HandleSceneViewInputs(cellCenter);
            sceneView.Repaint();
        }
    }
    private Vector2 GetSelectedCell()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);
            Vector2Int cell = new Vector2Int(Mathf.RoundToInt(mousePosition.x / cellSize.x), Mathf.RoundToInt(mousePosition.y / cellSize.y));
            return cell * cellSize;
        }
        return Vector2.zero;
    }
    private void HandleSceneViewInputs(Vector2 cellCenter)
    {
        // 씬 내에서 객체를 선택못하게 하기 위함.
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0); // Consume the event
        }
        // 프리팹을 선택하고 씬뷰를 클릭할경우.
        if (paletteIndex < palette.Count && Event.current.type ==
            EventType.MouseDown && Event.current.button == 0)
        {
            // 프리팹과 연결을 유지하면서 생성.
            GameObject prefab = palette[paletteIndex];
            GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            gameObject.transform.position = cellCenter;
            if (rotation90)
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            // Undo 사용.
            Undo.RegisterCreatedObjectUndo(gameObject, "");
        }
    }
    private void DisplayVisualHelp(Vector2 cellCenter)
    {
        // 월드 공간의 마우스 위치 가져오기.
        Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

        // 그리드에서 해당 셀 가져 오기
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(mousePosition.x / cellSize.x), Mathf.RoundToInt(mousePosition.y / cellSize.y));
        cellCenter = cell * cellSize;

        // 그려지는 정점.
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
        //기존 삽입했던 delegate 삭제.
        SceneView.duringSceneGui -= this.OnSceneGUI;
        //다시 추가.
        SceneView.duringSceneGui += this.OnSceneGUI;
        RefreshPalette();
    }
    #endregion

    #region GUI data Func
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
    #endregion
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
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
        GameObject gameManager = null, mainCamera = null, birdGun = null, worldRect = null;
        GameObject obj;
        foreach (var obstacle in data.obstacleInfoList)
        {
            obj = FindPrefab(obstacle);
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
