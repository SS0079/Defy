using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct BallisticMissileConfig : IComponentData
    {
        public float ReenterSpeed;
        public float Ceiling;
    }
}