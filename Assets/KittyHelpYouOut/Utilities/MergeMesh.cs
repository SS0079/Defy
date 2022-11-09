using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NameSpace
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MergeMesh:MonoBehaviour
	{
        [ContextMenu("MergeMesh")]
        private void Merge()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //combine[i].transform = Matrix4x4.identity;
                meshFilters[i].gameObject.SetActive(false);
            }
            var meshFilter = transform.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.CombineMeshes(combine);
            transform.gameObject.SetActive(true);
            //this.transform.localScale = new Vector3(1, 1, 1);
            //this.transform.position = Vector3.zero;
            //this.transform.rotation = Quaternion.identity;
        }
        [ContextMenu("CleanUp")]
        private void CleanUp()
        {
            var meshFilter = this.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = null;
            foreach (var filter in this.transform.GetComponentsInChildren<MeshFilter>(true))
            {
                filter.gameObject.SetActive(true);
            }
        }
	}
}


