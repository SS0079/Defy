using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct DestroyFlagData : IComponentData
    {
        public bool Yes;

        public void SetYes() => Yes = true;
    }
}