using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
    [AddComponentMenu("KittyHelpYouOut/FreeMove")]
    public class FreeMove : MonoBehaviour
    {
        public Transform Mover;
        public bool CanMove=true;
        public bool CanRotate = true;
        public float moveSpeed;
        public float aimSensitivity;
        private Vector3 moveAxis;
        private Vector3 aimAxis;
        #region CustomMethod
        private void GetInputAxis()
        {
            moveAxis = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));
            aimAxis = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        }
        private void Move()
        {
            Mover.position += transform.TransformDirection(moveAxis.x, 0f, moveAxis.z) * moveSpeed * Time.deltaTime;
            Mover.position += Vector3.up * moveAxis.y * moveSpeed * Time.deltaTime;
        }
        private void Aim()
        {
            Mover.Rotate(aimAxis.x * aimSensitivity * Time.deltaTime, 0f, 0f, Space.Self);
            Mover.RotateAround(Mover.position, Vector3.up, aimAxis.y * aimSensitivity * Time.deltaTime);

        }
        #endregion
        #region BuildInMethod

        void Update()
        {
            if (Mover!=null)
            {
                if (CanMove)
                {
                    GetInputAxis();
                    Move();
                }
                if (CanRotate && Input.GetMouseButton(1))
                {
                    Aim();
                }
            }
            else
            {
                Debug.LogWarning("Mover is not set",this.gameObject);
            }
        }

        #endregion
    }
}