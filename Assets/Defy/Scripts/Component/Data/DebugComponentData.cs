using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct DebugComponentData : IComponentData
    {
        public float FloatValue;
        public float3 Float3Value;
        public float3 Float3Value2;
    }
}