using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace YH_CustomEditor
{

    public class CustomWindow : EditorWindow
    {
        const int MAX_SPRITE_COUNT = 4;
        SerializedProperty m_IntProp;
        public GameObject referenceObject;
        Sprite sprite1;
        Sprite sprite2;
        Sprite sprite3;
        Sprite sprite4;
        bool isUsingRigidBody           =true;
        //bool isUsingCircleCollider      =true;
        //bool isUsingAnimator            =true;
        //bool IsUsingBirdCollider        =true;
        //bool IsUsingBirdAnimatioChanger =true;
        //bool IsUsingAutoDestroyBird     =true;
        bool isUsingObstacleInterationScripts = true;

        int tagIdx = 0;
        int sortingLayerIdx = 0;
        enum eColliderType { BOX,POLYGON,SPHERE};
        private string[] colliderOptions = new string[] { "Box", "Polygon","Circle" };
        int colliderSelected = 0;
        [MenuItem("YH_Custom/CreateObstacleWindow")]
        static void CreateWindow()
        {
            CustomWindow wnd = EditorWindow.CreateWindow<CustomWindow>();
            wnd.Show();
            wnd.titleContent = new GUIContent("CreateObstacleWindow");
        }
        private void OnGUI()
        {

            GUILayout.Space(10);
            CustomWndHelper.CreateGameObjectField("값을 참조할 기본 Gameobject",ref referenceObject);
            //CustomWndHelper.CreateLabel("a", "b");
            
            CustomWndHelper.CreateSpriteField("HPLow",ref sprite1);
            CustomWndHelper.CreateSpriteField("HPMidle", ref sprite2);
            CustomWndHelper.CreateSpriteField("HPMidleBig", ref sprite3);
            CustomWndHelper.CreateSpriteField("HPBig", ref sprite4);
            //체크박스
            GUILayout.Space(10);
            GUILayout.Label("추가할 Component들");
            CustomWndHelper.CreateTextToggle("RigidBody", ref isUsingRigidBody);

            colliderSelected = GUILayout.SelectionGrid(colliderSelected,
                colliderOptions, colliderOptions.Length, EditorStyles.radioButton);

            CustomWndHelper.CreateTextToggle("Obstacle Scripts", ref isUsingObstacleInterationScripts);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("tags");
            tagIdx = EditorGUILayout.Popup(tagIdx, UnityEditorInternal.InternalEditorUtility.tags);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("sortingLayer");
            sortingLayerIdx = EditorGUILayout.Popup(sortingLayerIdx, YH_Helper.YH_Helper.GetSortingLayerNames() );
            GUILayout.EndHorizontal();
            if (GUILayout.Button("CreateObstacle"))
            {
                if (!(sprite1 && sprite2 && sprite3 && sprite4 && referenceObject))
                {
                    PopupWindow.Init("모든값을 설정해주세요.", "확인");
                    return;
                }
                CreateObstacleObject();
            }


        }
        private void OnEnable()
        {
            
        }
        private void CreateObstacleObject()
        {
            GameObject tmpGameobj = new GameObject("Obstacle");
            tmpGameobj.tag = UnityEditorInternal.InternalEditorUtility.tags[tagIdx];
            SpriteRenderer tmpSpriteRenderer = tmpGameobj.AddComponent<SpriteRenderer>();
            tmpSpriteRenderer.sprite = sprite4;
            tmpSpriteRenderer.sortingLayerName = YH_Helper.YH_Helper.GetSortingLayerNames()[sortingLayerIdx];
            if (isUsingRigidBody)
            {
               Rigidbody2D rgd2D = tmpGameobj.AddComponent<Rigidbody2D>();
                rgd2D = referenceObject.GetComponent<Rigidbody2D>();
            }
            switch ((eColliderType)colliderSelected)
            {
                case eColliderType.BOX:
                    tmpGameobj.AddComponent<BoxCollider2D>();
                    break;
                case eColliderType.POLYGON:
                    tmpGameobj.AddComponent<PolygonCollider2D>();
                    break;
                case eColliderType.SPHERE:
                    tmpGameobj.AddComponent<CircleCollider2D>();
                    break;

            }

            if (isUsingObstacleInterationScripts)
            {
                obstacleInteration interation = tmpGameobj.AddComponent<obstacleInteration>();
                interation.sprites = new Sprite[4];
                interation.sprites[0] = sprite1;
                interation.sprites[1] = sprite2;
                interation.sprites[2] = sprite3;
                interation.sprites[3] = sprite4;

            }

        }
    }


}
