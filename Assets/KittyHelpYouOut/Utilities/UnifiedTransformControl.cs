using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KittyHelpYouOut
{
    public enum AnimeType
    {
        position,
        rotation,
        scale
    }
    public enum Axis
    {
        X,
        Y,
        Z
    }
    [AddComponentMenu("KittyHelpYouOut/UnifiedTransformControl")]
    public class UnifiedTransformControl : MonoBehaviour
	{
        #region Variable
        public AnimeType Type;
        public Axis Axis;
        [Header("ControlValue")]
        public float ControlValue;
        public bool DebugInfo = false;

        private List<Transform> _Children;
        public List<Transform> Children
        {
            get
            {
                _Children=_Children == null||_Children.Count==0 ?  this.GetComponentsOnlyInChildren<Transform>():_Children;
                return _Children;
            }
        }
        #endregion

        #region CustomMethod
        private void TransformControl()
        {
            switch (Type)
            {
                case AnimeType.position:
                    switch (Axis)
                    {
                        case Axis.X:
                            foreach (var Child in Children)
                            {
                                Child.position = new Vector3(ControlValue, Child.position.y, Child.position.z);
                            }
                            break;
                        case Axis.Y:
                            foreach (var Child in Children)
                            {
                                Child.position = new Vector3(Child.position.x, ControlValue, Child.position.z);
                            }
                            break;
                        case Axis.Z:
                            foreach (var Child in Children)
                            {
                                Child.position = new Vector3(Child.position.x, Child.position.y, ControlValue);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case AnimeType.rotation:
                    switch (Axis)
                    {
                        case Axis.X:
                            foreach (var Child in Children)
                            {
                                Child.localEulerAngles = new Vector3(ControlValue, Child.localEulerAngles.y, Child.localEulerAngles.z);
                            }
                            break;
                        case Axis.Y:
                            foreach (var Child in Children)
                            {
                                Child.localEulerAngles = new Vector3(Child.localEulerAngles.x, ControlValue, Child.localEulerAngles.z);
                            }
                            break;
                        case Axis.Z:
                            foreach (var Child in Children)
                            {
                                Child.localEulerAngles = new Vector3(Child.localEulerAngles.x, Child.localEulerAngles.y, ControlValue);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case AnimeType.scale:
                    switch (Axis)
                    {
                        case Axis.X:
                            foreach (var Child in Children)
                            {
                                Child.localScale = new Vector3(ControlValue, Child.localScale.y, Child.localScale.z);
                            }
                            break;
                        case Axis.Y:
                            foreach (var Child in Children)
                            {
                                Child.localScale = new Vector3(Child.localScale.x, ControlValue, Child.localScale.z);
                            }
                            break;
                        case Axis.Z:
                            foreach (var Child in Children)
                            {
                                Child.localScale = new Vector3(Child.localScale.x, Child.localScale.y, ControlValue);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
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
            TransformControl();
            if (DebugInfo)
            {
                Debug.Log(Children[0].transform.localEulerAngles);
            }

        }
        private void LateUpdate()
        {

        }
        #endregion
    }
}