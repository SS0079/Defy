using Unity.Entities;
using Unity.Mathematics;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct AimPosData : IComponentData
    {
        public float3 Value;
        public bool HasValue;
    }
}