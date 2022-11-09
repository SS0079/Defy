using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
    [RequireComponent(typeof(MeshFilter))]
    public class QuadrangularPrismCollider : MonoBehaviour
    {
        public float maxDistance = 10f;
        public Vector2 size = Vector2.one;

        private MeshFilter mf = null;
        private Mesh mesh = null;
        private MeshCollider mc = null;

        private Vector3 luVec = new Vector3(-1f, 1f, 0);
        private Vector3 ruVec = new Vector3(1f, 1f, 0);
        private Vector3 llVec = new Vector3(-1f, -1f, 0);
        private Vector3 rlVec = new Vector3(1f, -1f, 0);

        private void Start()
        {
            mf = GetComponent<MeshFilter>();
            mesh = CreateQuadrangularPrismMesh();
            mf.sharedMesh = mesh;

            mc = gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
            mc.isTrigger = true;
        }

        private Mesh CreateQuadrangularPrismMesh()
        {
            Vector3 leftUpper = transform.position + (transform.rotation * luVec * size.x / 2f + transform.forward * maxDistance);
            Vector3 rightUpper = transform.position + (transform.rotation * ruVec * size.x / 2f + transform.forward * maxDistance);
            Vector3 leftLower = transform.position + (transform.rotation * llVec * size.y / 2f + transform.forward * maxDistance);
            Vector3 rightLower = transform.position + (transform.rotation * rlVec * size.y / 2f + transform.forward * maxDistance);
            Vector3 top = transform.position;

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
            transform.InverseTransformPoint(top),
            transform.InverseTransformPoint(leftUpper),
            transform.InverseTransformPoint(rightUpper),
            transform.InverseTransformPoint(leftLower),
            transform.InverseTransformPoint(rightLower)
            };
            mesh.triangles = new int[]
            {
            0,1,2,
            0,3,1,
            0,2,4,
            0,4,3,
            3,2,1,
            2,3,4
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.name = "QuadrangularPrism";

            return mesh;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 leftUpper = transform.position + (transform.rotation * luVec * size.x / 2f + transform.forward * maxDistance);
            Vector3 rightUpper = transform.position + (transform.rotation * ruVec * size.x / 2f + transform.forward * maxDistance);
            Vector3 leftLower = transform.position + (transform.rotation * llVec * size.y / 2f + transform.forward * maxDistance);
            Vector3 rightLower = transform.position + (transform.rotation * rlVec * size.y / 2f + transform.forward * maxDistance);
            Vector3 top = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftUpper, rightUpper);
            Gizmos.DrawLine(leftUpper, leftLower);
            Gizmos.DrawLine(rightUpper, rightLower);
            Gizmos.DrawLine(leftLower, rightLower);

            Gizmos.DrawLine(top, leftUpper);
            Gizmos.DrawLine(top, rightUpper);
            Gizmos.DrawLine(top, leftLower);
            Gizmos.DrawLine(top, rightLower);
        }
#endif

    }
}