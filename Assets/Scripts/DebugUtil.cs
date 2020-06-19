using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

}