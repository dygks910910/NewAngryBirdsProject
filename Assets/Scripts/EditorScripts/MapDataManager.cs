using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapDataManagerWindow : EditorWindow
{
    [MenuItem("YH_Custom/MapDataManager")]
    static void CreateWindow()
    {
        MapDataManagerWindow wnd = EditorWindow.CreateWindow<MapDataManagerWindow>();
        wnd.Show();
        wnd.titleContent = new GUIContent("MapDataManager");
        allObjects = GameObject.FindObjectsOfType<GameObject>();
        //allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i  =0; i < allObjects.Length; ++i)
        {
            //if (PrefabUtility.GetPrefabAssetType(allObjects[i]) == PrefabAssetType.Regular)
            if(PrefabUtility.GetPrefabParent(allObjects[i]) != null &&
                PrefabUtility.GetPrefabObject(allObjects[i].transform) != null)
            {
                prefabs.Add(allObjects[i]);
            }
        }
    }

    static public GameObject[] allObjects;
    static public List<GameObject> prefabs = new List<GameObject>();
    private void OnGUI()
    {
        for(int i = 0; i < prefabs.Count;++i)
        {
            YH_CustomEditor.CustomWndHelper.CreateLabel("prefabs", prefabs[i].name);
        }
        GUILayout.Space(10);
        for(int i = 0; i < allObjects.Length; ++i)
        {
            YH_CustomEditor.CustomWndHelper.CreateLabel("object", allObjects[i].name);
        }
    }

}
