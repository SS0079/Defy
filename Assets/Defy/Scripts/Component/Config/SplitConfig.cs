using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct SplitConfig : IComponentData
    {
        public int Count;
        public float SpreadRange;
    }
}