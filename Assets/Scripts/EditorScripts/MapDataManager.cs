using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using YH_Data;
using System.IO;

public class MapDataManagerWindow : EditorWindow
{
    [MenuItem("YH_Custom/MapDataManager")]
    static void CreateWindow()
    {
        MapDataManagerWindow wnd = EditorWindow.CreateWindow<MapDataManagerWindow>();
        wnd.Show();
        wnd.titleContent = new GUIContent("MapDataManager");
        allObjects = GameObject.FindObjectsOfType<GameObject>();
        allObjjectRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        prefabs = allObjjectRoot.Where(data => PrefabUtility.GetCorrespondingObjectFromSource(data) != null &&
                PrefabUtility.GetPrefabInstanceHandle(data.transform) != null).ToArray();
    }
    static public GameObject[] allObjects;
    static public GameObject[] allObjjectRoot;
    static public GameObject[] prefabs;
    Vector2 scollPos;
    private void OnGUI()
    {
        scollPos = GUILayout.BeginScrollView(scollPos);
        for(int i = 0; i < prefabs.Length;++i)
        {
            YH_CustomEditor.CustomWndHelper.CreateLabel("prefabs", prefabs[i].name);
        }
        GUILayout.Space(10);
        //for(int i = 0; i < allObjects.Length; ++i)
        //{
        //    YH_CustomEditor.CustomWndHelper.CreateLabel("object", allObjects[i].name);
        //}
        GUILayout.EndScrollView();
        if(GUILayout.Button("Generate Map File"))
        {
            AngryBirdMapData data = new AngryBirdMapData();
            ObstacleInfo info = new ObstacleInfo();

            for(int i = 0; i < prefabs.Length; ++i)
            {
                info.objectName = prefabs[i].name;
                info.objPosition = prefabs[i].transform.position;
                info.objRotation = prefabs[i].transform.rotation;
                info.objScale = prefabs[i].transform.localScale;
                info.isStatic = prefabs[i].isStatic;
                data.obstacleInfo.Add(info);
            }
            Debug.Log(data.objectToJson());
            string path = EditorUtility.SaveFilePanel("Json 파일을 Save할 경로", "Assets", "",".json");
            File.WriteAllText(path,data.objectToJson());

            EditorUtility.DisplayDialog("저장경로 확인", path + "에 저장 완료하였습니다", "확인");
            
        }

        if (GUILayout.Button("Load Map File"))
        {
            string jsonPath = EditorUtility.OpenFilePanel("Json 파일을 Open할 경로", "Assets",".json");
            string jsonString = File.ReadAllText(jsonPath);

            AngryBirdMapData data = new AngryBirdMapData();
            data = AngryBirdMapData.JsonToObject<AngryBirdMapData>(jsonString);
            
            Resources.LoadAll<GameObject>("Prefabs/CachingOnecePrefabs");

            data.PrintData();
            //ObstacleInfo info = new ObstacleInfo();

            GameObject obj;
            foreach (var obstacle in data.obstacleInfo)
            {
                string path = "Assets/Resources/Prefabs/CachingPrefabs/" + obstacle.objectName + ".prefab";
                //string path = AssetDatabase.GetAssetPath();

                obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if(obj == null)
                {
                    path = "Assets/Resources/Prefabs/ChchingOnecePrefabs/" + obstacle.objectName + ".prefab";
                    obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }

                // obj = PrefabUtility.LoadPrefabContents(path);
                if (obj != null)
                {
                    obj = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                    obj.transform.position = obstacle.objPosition;
                    obj.transform.rotation = obstacle.objRotation;
                    obj.transform.localScale = obstacle.objScale;
                    obj.isStatic = obstacle.isStatic;
                    obj.name = obstacle.objectName;
                }
                
            }

        }
    }
  

}
