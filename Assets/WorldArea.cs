using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace YH_Class
{
    public class WorldArea : MonoBehaviour
    {
        [Tooltip("Left top이 원점")]
        public Rect worldRect = new Rect();
        public float range = 5.0f;
    }

    [CustomEditor(typeof(WorldArea))]
    public class WorldAreaEditor : Editor
    {
        public GameObject[] objs = null;
        public Vector3 originPos = new Vector3(0, 0, -1);
        void OnSceneGUI()
        {
            WorldArea rectObj = target as WorldArea;
            if (rectObj.worldRect == null)
                return;
            originPos = rectObj.transform.position;
            Handles.color = Color.cyan;

            Vector3[] verts = new Vector3[]
            {
             new Vector3(originPos.x - rectObj.range, originPos.y,-1),//LT
            new Vector3(originPos.x - rectObj.range, originPos.y + rectObj.range,-1),//LB
            new Vector3(originPos.x + rectObj.range, originPos.y + rectObj.range, -1),//RB
            new Vector3(originPos.x + rectObj.range, originPos.y, -1)//RT
            };
            Handles.DrawSolidRectangleWithOutline(verts, new Color(0.5f, 0.5f, 0.5f, 0.1f), new Color(0, 0, 0, 1));

            foreach (Vector3 posCube in verts)
            {
                rectObj.range = Handles.ScaleValueHandle(rectObj.range,
                    posCube,
                    Quaternion.identity,
                    1.0f,
                    Handles.CubeHandleCap,
                    1.0f);
            }
            rectObj.worldRect.x = verts[0].x;
            rectObj.worldRect.y = verts[0].y + rectObj.range;
            rectObj.worldRect.width = rectObj.range * 2;
            rectObj.worldRect.height = -rectObj.range;

            //LT to RT
            Handles.DrawLine(new Vector3(rectObj.worldRect.xMin, rectObj.worldRect.yMin, -1),
                new Vector3(rectObj.worldRect.xMax, rectObj.worldRect.yMin, -1));
            ////RT to RB
            Handles.DrawLine(new Vector3(rectObj.worldRect.xMax, rectObj.worldRect.yMin, -1),
                new Vector3(rectObj.worldRect.xMax, rectObj.worldRect.yMax, -1));
            //RB to LB
            Handles.DrawLine(new Vector3(rectObj.worldRect.xMax, rectObj.worldRect.yMax, -1),
                new Vector3(rectObj.worldRect.xMin, rectObj.worldRect.yMax, -1));
            ////LB to LT
            Handles.DrawLine(new Vector3(rectObj.worldRect.xMin, rectObj.worldRect.yMax, -1),
                new Vector3(rectObj.worldRect.xMin, rectObj.worldRect.yMin, -1));
        }

    }
}