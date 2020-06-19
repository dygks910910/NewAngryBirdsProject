using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH_Math
{

    public class YHMath
    {
       static public Quaternion GetRotFromVectors(Vector2 posStart,Vector2 posEnd)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(posEnd.x - posStart.x, posEnd.y - posStart.y) * Mathf.Rad2Deg);
        }
    }

}
