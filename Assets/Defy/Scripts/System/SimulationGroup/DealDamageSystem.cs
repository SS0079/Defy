using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Defy.System
{
    [UpdateInGroup(typeof(DefyFinalUpdateGroup))]
    // [UpdateAfter(typeof(ChangeSwitchableMatSystem))]
    public partial class DealDamageSystem : SystemBase
    {
        

        protected override void OnUpdate()
        {
            Dependency=new DealDamageToSoftCharacterJob().ScheduleParallel(HitCheckSystem.HitCheckJobHandle);
        }
        
        // [BurstCompile]
        // private partial struct DealDamageToCollisionCharacterJob : IJobEntity
        // {
        //     public void Execute(ref CombatData combatData,ref SimpleCharacterMovementData characterMovementData)
        //     {
        //         if (combatData.IncomingDamage!=0)
        //         {
        //             combatData.DealDamage();
        //         }
        //         if (math.lengthsq(combatData.KnockBackVelocity)>0)
        //         {
        //             characterMovementData.KinematicVelocity += combatData.KnockBackVelocity;
        //             combatData.KnockBackVelocity=float3.zero;
        //         }
        //     }
        // }
        
        [BurstCompile]
        private partial struct DealDamageToSoftCharacterJob : IJobEntity
        {

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref CombatData combatData)
            {
                if (combatData.IncomingDamage!=0)
                {
                    combatData.DealDamage();
                }
                
            }
        }
    }
}