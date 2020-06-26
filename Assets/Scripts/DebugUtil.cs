using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YH_Debug
{

public class DebugUtil
{
        // Start is called before the first frame update
        static public void DrawRect(Rect rt)
        {
            /*
             ---
            l   l
            l   l
             ---
             */
            //draw LT to RT;
            Debug.DrawLine(new Vector3(rt.xMin, rt.yMin), new Vector3(rt.xMax, rt.yMin), UnityEngine.Color.red);
            //draw LT to LB;
            Debug.DrawLine(new Vector3(rt.xMin, rt.yMin), new Vector3(rt.xMin, rt.yMax), UnityEngine.Color.red);
            //////draw LB to RB;
            Debug.DrawLine(new Vector3(rt.xMin, rt.yMax), new Vector3(rt.xMax, rt.yMax), UnityEngine.Color.red);
            //////draw RT to RB;
            Debug.DrawLine(new Vector3(rt.xMax, rt.yMin), new Vector3(rt.xMax, rt.yMax), UnityEngine.Color.red);


        }
        static public void DrawRect(float left,float top,float right,float bottom)
        {
            /*
             ---
            l   l
            l   l
             ---
             */
            //draw LT to RT;
            Debug.DrawLine(new Vector3(left, top), new Vector3(right, top), UnityEngine.Color.red);
            //draw LT to LB;
            Debug.DrawLine(new Vector3(left,top), new Vector3(left,bottom), UnityEngine.Color.red);
            //////draw LB to RB;
            Debug.DrawLine(new Vector3(left,bottom), new Vector3(right, bottom), UnityEngine.Color.red);
            //////draw RT to RB;
            Debug.DrawLine(new Vector3(right,top), new Vector3(right,bottom), UnityEngine.Color.red);
        }
        static public void DrawRectInScene(float left, float top, float right, float bottom)
        {
            //LT to RT
            Handles.DrawLine(new Vector3(left, top, -1),
                new Vector3(right,top, -1));
            ////RT to RB
            Handles.DrawLine(new Vector3(right,top, -1),
                new Vector3(right,bottom, -1));
            //RB to LB
            Handles.DrawLine(new Vector3(right,bottom, -1),
                new Vector3(left,bottom, -1));
            ////LB to LT
            Handles.DrawLine(new Vector3(left,bottom, -1),
                new Vector3(left,top, -1));
        }


    }

}