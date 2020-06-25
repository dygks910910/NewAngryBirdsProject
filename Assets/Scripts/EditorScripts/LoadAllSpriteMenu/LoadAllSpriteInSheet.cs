using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LoadAllSpriteInSheet
{
    [MenuItem("Assets/YH_Menu/LoadAllSpriteToGameobject", false, 0)]
    static void LoadAll()
    {
        //var file = Selection.activeObject.name;
        string path = EditorUtility.OpenFilePanel("", "Assets", "");
        //GameObject obj = new GameObject("test");
        //obj.AddComponent<SpriteRenderer>();
        // AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/Textures/" + SpriteTextureName + TextureExtension
        int idx = path.IndexOf("Assets");

        GameObject obj;
        Sprite[] sprites;
        sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path).OfType<Sprite>().ToArray();

        foreach (Sprite s in sprites)
        {
            obj = new GameObject(s.name);
            obj.AddComponent<SpriteRenderer>().sprite = s;


        }
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("Assets/YH_Menu/LoadAllSpriteToGameobject", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeObject != null && EditorUtility.IsPersistent(Selection.activeObject);
    }
}
