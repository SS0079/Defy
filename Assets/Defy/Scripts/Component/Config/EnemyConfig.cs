using Unity.Entities;
using Unity.Mathematics;

namespace Defy.Component
{
    public struct EnemyConfig : IComponentData
    {
        public float MaxHp;
        public float3 Destination;
    }
}