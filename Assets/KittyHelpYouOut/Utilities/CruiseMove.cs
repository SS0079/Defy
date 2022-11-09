using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KittyHelpYouOut
{
    [AddComponentMenu("KittyHelpYouOut/CruiseMove")]
	public class CruiseMove:MonoBehaviour
    {
        public Transform Mover;
        public bool CanMove;
        public float CruiseSpeed;
        public float ZoomSpeed;
        public Vector2 ZoomRestriction=new Vector2();
        public Action OnMove;
        private void Update()
        {
            if (Mover!=null)
            {
                if (CheckCanMove())
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        Mover.position += new Vector3(0, 0, CruiseSpeed * Time.deltaTime);
                        OnMove?.Invoke();
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        Mover.position += new Vector3(0, 0, -CruiseSpeed * Time.deltaTime);
                        OnMove?.Invoke();
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        Mover.position += new Vector3(-CruiseSpeed * Time.deltaTime, 0, 0);
                        OnMove?.Invoke();
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        Mover.position += new Vector3(CruiseSpeed * Time.deltaTime, 0, 0);
                        OnMove?.Invoke();
                    }
                    var scrollWheel = Input.GetAxis("Mouse ScrollWheel");
                    Mover.position += new Vector3(0, -scrollWheel * ZoomSpeed * Time.deltaTime, 0);
                    Mover.position =new Vector3(Mover.position.x, Mathf.Clamp(Mover.position.y, ZoomRestriction.x, ZoomRestriction.y),this.transform.position.z);
                }
            }
            else
            {
                Debug.LogWarning("Mover is not set",this.gameObject);
            }
        }
        public virtual bool CheckCanMove()
        {
            return CanMove;
        }
    }
}