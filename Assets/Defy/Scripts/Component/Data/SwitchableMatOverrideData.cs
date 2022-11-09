using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    [MaterialProperty("_Lerp",MaterialPropertyFormat.Float)]
    public struct SwitchableMatOverrideData : IComponentData
    {
        public float Value;
        
    }
}