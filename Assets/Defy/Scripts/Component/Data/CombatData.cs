using Unity.Entities;
using Unity.Mathematics;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct CombatData : IComponentData
    {
        public float HP;
        public float IncomingDamage;
        public float3 KnockBackVelocity;

        public void DealDamage()
        {
            HP -= IncomingDamage;
            IncomingDamage = 0;
        }
    }
}