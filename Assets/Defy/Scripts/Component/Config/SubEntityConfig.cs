using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct SubEntityConfig : IComponentData
    {
        public Entity Value1;
        public Entity Value2;
        public Entity Value3;
        public Entity Value4;
        
    }
}