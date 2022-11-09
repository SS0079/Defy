using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KittyHelpYouOut
{
    [ExecuteInEditMode]
    [AddComponentMenu("KittyHelpYouOut/ScaleMeter")]
	public class ScaleMeter : MonoBehaviour
	{
        #region Variable
        public Vector3 realScale=new Vector3();
        public Vector3 originalScale = new Vector3();
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
		#endregion

		#region CustomMethod
        private void GetRealScale()
        {
            realScale = meshRenderer.bounds.size;
        }

        private void GetOriginalScale()
        {
            originalScale = meshFilter.sharedMesh.bounds.size;
        }
		#endregion	
	
		#region BuildInMethod
        private void Awake()
        {
            if (!this.TryGetComponent<MeshRenderer>(out meshRenderer))
            {
                meshRenderer = this.GetComponentsInChildren<MeshRenderer>()?[0];
            }
            if (!this.TryGetComponent<MeshFilter>(out meshFilter))
            {
                meshFilter = this.GetComponentsInChildren<MeshFilter>()?[0];
            }
        }

		private void OnEnable()
        {
            
        }
		
        private void Start()
        {
            
        }

        private void FixedUpdate()
        {
            
        }

        private void Update()
        {
            if (meshRenderer!=null)
            {
                GetRealScale();
            }
            if (meshFilter!=null)
            {
                GetOriginalScale();
            }
        }

        private void LateUpdate()
        {
            
        }

        #endregion
    }
}


