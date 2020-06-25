using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel.Design;

public class AutoGenPrefab 
{
    [MenuItem("GameObject/YH_Menu/SelectionsSaveToPrefabs", false,0)]
    static public void CreatePrefab(UnityEditor.MenuCommand menuCommand)
    {
        //오브젝트 수만큼 실행되는걸 방지하기 위함.
        if (Selection.objects.Length > 1)
        {
            if (menuCommand.context != Selection.objects[0])
                return;
        }
        GameObject[] objectArray = Selection.gameObjects;
        string path = EditorUtility.SaveFolderPanel("Save prefab to folder", "Assets/Resources/Prefabs", "");
        int idx = path.IndexOf("Assets");
        if(idx == -1)
        {
            EditorUtility.DisplayDialog("작업이 취소됨", "Assets폴더의 하위폴더여야 합니다.", "확인");
            return;
        }
        path = path.Substring(idx, path.Length - idx);
        foreach (GameObject gameObject in objectArray)
        {
            string pathName = path +"/"+ gameObject.name + ".prefab";
            pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, pathName, InteractionMode.UserAction);
        }
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("GameObject/YH_Menu/SelectionsSaveToPrefabs", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.gameObjects[0] != null && !EditorUtility.IsPersistent(Selection.gameObjects[0]);
    }
}
