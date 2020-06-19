using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YH_CustomEditor
{
    public class CustomWndHelper : EditorWindow
    {
        public static void CreateLabel(string title, string content)
        { //
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            Rect position = EditorGUILayout.GetControlRect(false, 15f);
            EditorGUI.SelectableLabel(position, string.Format("{0}", content));
            EditorGUILayout.EndHorizontal();
        }
        
        public static void CreateSpriteField(string title,ref Sprite src)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title);
            src = EditorGUILayout.ObjectField(src, typeof(Sprite), true) as Sprite;
            EditorGUILayout.EndHorizontal();
        }
        public static void CreateGameObjectField(string title, ref GameObject src)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title);
            src = EditorGUILayout.ObjectField(src, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndHorizontal();
        }
        public static void CreateTextToggle(string text,ref bool toggle)
        {
            toggle = GUILayout.Toggle(toggle, text);
        }
    }
    public class PopupWindow : EditorWindow
    {
        static string m_strDesc = "";
        static string m_strButton = "";

        static public void Init(string desc, string button)
        {
            PopupWindow window = ScriptableObject.CreateInstance<PopupWindow>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            window.ShowPopup();
            m_strDesc = desc;
            m_strButton = button;

        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(m_strDesc, EditorStyles.wordWrappedLabel);
            GUILayout.Space(70);
            if (GUILayout.Button(m_strButton)) this.Close();
        }

    }

}
