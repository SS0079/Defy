using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Defy.Component
{
    [Serializable]
    public struct SimpleCharacterMoveConfig : IComponentData
    {
        public float MoveSpeed;
        public float LinearDrag;
        public float3 Gravity;
        public float JumpForce;
        public float SlopLimit;
        public float SocialDistance;
        public float RepulsionForce;
        // public float FixHeight;
    }


}