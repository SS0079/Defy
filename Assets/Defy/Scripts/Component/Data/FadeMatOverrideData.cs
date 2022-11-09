using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    [MaterialProperty("_Fade",MaterialPropertyFormat.Float)]
    public struct FadeMatOverrideData : IComponentData
    {
        public float Value;
        
    }
}