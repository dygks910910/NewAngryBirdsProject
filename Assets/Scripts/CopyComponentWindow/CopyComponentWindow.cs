using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace YH_CustomEditor
{

    public class CopyComponentWindow : EditorWindow
    {
        GameObject referenceObject;
        GameObject targetObject;

        [MenuItem("YH_Custom/CopyComponents")]
        static void CreateWindow()
        {
            CopyComponentWindow wnd = EditorWindow.CreateWindow<CopyComponentWindow>();
            wnd.Show();
            wnd.titleContent = new GUIContent("CopyComponents");
        }
        private void OnGUI()
        {
            GUILayout.Space(10);
            CustomWndHelper.CreateGameObjectField("값을 참조할 기본 Gameobject", ref referenceObject);
            GUILayout.Space(10);
            CustomWndHelper.CreateGameObjectField("추가할 대상 Gameobject", ref targetObject);


            if (GUILayout.Button("CopyComponent"))
            {
              foreach(var component in referenceObject.GetComponents<Component>())
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject);
                }
            }
        }
    }

}
