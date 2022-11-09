using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct EntityLeaderData : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
    }
}