using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct SimpleCharacterMovementData : IComponentData
    {
        /// <summary>
        /// Controlled velocity is calculated,used and cleaned every frame
        /// </summary>
        public float3 ControlledVelocity;
        /// <summary>
        /// Raw acceleration is calculated,used and cleaned every frame
        /// </summary>
        public float3 RawAcceleration;
        /// <summary>
        /// persistent movement , hold movement value between frames
        /// </summary>
        public float3 KinematicVelocity;

        // public GroundingState GroundCheck;

        public bool IsCorner;
        public bool IsGround;
        public bool IsRamp;
    }
    // public enum GroundingState
    // {
    //     InAir=0,
    //     OnGround=1,
    //     Sloping,
    //     Climb
    // }
}