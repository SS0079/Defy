using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct MovementData : IComponentData
    {
        public bool IsLocal;
        public float3 LinearVelocity;
        public float3 ForceVelocity;
    }
}