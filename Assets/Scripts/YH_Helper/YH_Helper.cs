using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.Reflection;

namespace YH_Helper
{
    public static class YH_Helper 
    {
        public static bool CheckAnimStateIsDestory(Animator animator)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Destory"))
            {
            return true;

            }
            return false;
        }

        public static string[] GetSortingLayerNames()
        {
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty =
            internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
    }

}
