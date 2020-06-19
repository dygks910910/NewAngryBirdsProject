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
        bool isUsingCircleCollider      =true;
        bool isUsingAnimator            =true;
        bool IsUsingBirdCollider        =true;
        bool IsUsingBirdAnimatioChanger =true;
        bool IsUsingAutoDestroyBird     =true;
        bool isUsingBoxCollider = true;
        bool isUsingObstacleInterationScripts = true;

        int tagIdx = 0;
        int sortingLayerIdx = 0;


        [MenuItem("YH_Custom/CreateObstacleWindow")]
        static void CreateWindow()
        {
            CustomWindow wnd = EditorWindow.CreateWindow<CustomWindow>();
            wnd.Show();
            wnd.title = "CreateObstacleWindow";
        }
        private void OnGUI()
        {

            GUILayout.Space(10);
            CustomWndHelper.CreateGameObjectField("값을 참조할 기본 Gameobject",ref referenceObject);
            //CustomWndHelper.CreateLabel("a", "b");
            CustomWndHelper.CreateSpriteField("Hp에따른Sprite1",ref sprite1);
            CustomWndHelper.CreateSpriteField("Hp에따른Sprite2", ref sprite2);
            CustomWndHelper.CreateSpriteField("Hp에따른Sprite3", ref sprite3);
            CustomWndHelper.CreateSpriteField("Hp에따른Sprite4", ref sprite4);
            //체크박스
            GUILayout.Space(10);
            GUILayout.Label("추가할 Component들");
            CustomWndHelper.CreateTextToggle("RigidBody", ref isUsingRigidBody);
            //CustomWndHelper.CreateTextToggle("CircleCollider", ref isUsingCircleCollider);
            //CustomWndHelper.CreateTextToggle("Animator", ref isUsingAnimator);
            //CustomWndHelper.CreateTextToggle("BirdCollider", ref IsUsingBirdCollider);
            //CustomWndHelper.CreateTextToggle("BirdAnimationChanger", ref IsUsingBirdAnimatioChanger);
            //CustomWndHelper.CreateTextToggle("autoDestoryBird", ref IsUsingAutoDestroyBird);
            CustomWndHelper.CreateTextToggle("BoxCollider", ref isUsingBoxCollider);
            CustomWndHelper.CreateTextToggle("Obstacle Scripts", ref isUsingObstacleInterationScripts);


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
                CreateObstacleObject();
            }


        }

        private void CreateObstacleObject()
        {
            GameObject tmp = new GameObject("Obstacle");
            tmp.tag = UnityEditorInternal.InternalEditorUtility.tags[tagIdx];
            SpriteRenderer tmpSpriteRenderer = tmp.AddComponent<SpriteRenderer>();
            tmpSpriteRenderer.sprite = sprite1;
            tmpSpriteRenderer.sortingLayerName = YH_Helper.YH_Helper.GetSortingLayerNames()[sortingLayerIdx];
            if (isUsingRigidBody)
            {
               Rigidbody2D rgd2D = tmp.AddComponent<Rigidbody2D>();
                rgd2D = referenceObject.GetComponent<Rigidbody2D>();
            }
            if (isUsingBoxCollider)
            {
                BoxCollider2D boxColl = tmp.AddComponent<BoxCollider2D>();
            }
            if (isUsingObstacleInterationScripts)
            {
                obstacleInteration interation = tmp.AddComponent<obstacleInteration>();
                interation.sprites = new Sprite[4];
                interation.sprites[0] = sprite1;
                interation.sprites[1] = sprite2;
                interation.sprites[2] = sprite3;
                interation.sprites[3] = sprite4;

            }

        }
    }


}
