using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace KittyHelpYouOut
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ScaleMeter))]
	public class ScaleMeterInspector : Editor
	{
        #region Variable
        private ScaleMeter scaleMeter;
        #endregion

        #region CustomMethod

        #endregion

        #region BuildInMethod
        private void OnEnable()
        {
            scaleMeter = (ScaleMeter)target;
        }


        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Real Scale:");
            EditorGUILayout.LabelField("x="+scaleMeter.realScale.x.ToString()+"   |   y="+scaleMeter.realScale.y.ToString() + "   |   z="+scaleMeter.realScale.z.ToString());
            EditorGUILayout.LabelField("Original Scale:");
            EditorGUILayout.LabelField("x=" + scaleMeter.originalScale.x.ToString() + "   |   y=" + scaleMeter.originalScale.y.ToString() + "   |   z=" + scaleMeter.originalScale.z.ToString());
            EditorGUILayout.EndVertical();
        }
        #endregion
    }

#endif
}


