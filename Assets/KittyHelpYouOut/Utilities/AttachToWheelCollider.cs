using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AuxiliaryToolbox
{
    [AddComponentMenu("KittyHelpYouOut/AttachToWheelCollider")]
    public class AttachToWheelCollider : MonoBehaviour
	{
        #region Variable
        public WheelCollider Target;
        private Vector3 Pos;
        private Quaternion Rot;
        #endregion

        #region CustomMethod

        #endregion

        #region BuildInMethod
        private void Awake()
        {

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
            Target.GetWorldPose(out Pos, out Rot);
            this.transform.position = Pos;
        }
        private void LateUpdate()
        {

        }
        #endregion
    }
}