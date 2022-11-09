using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    [UpdateAfter(typeof(MovementSystem))]
    public partial class BallisticReenterSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            new BallisticReenterJob() {}.ScheduleParallel();
        }
        
        [BurstCompile]
        private partial struct BallisticReenterJob : IJobEntity
        {

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref Translation translation,
                ref Rotation rotation,
                ref MovementData movementData,
                in DestinationData destinationData,
                in BallisticMissileConfig ballisticMissileConfig)
            {
                //if missile ascend above ceiling, commence reenter
                if (translation.Value.y>ballisticMissileConfig.Ceiling)
                {
                    var aboveDestination = new float3(destinationData.Value.x, ballisticMissileConfig.Ceiling-1, destinationData.Value.z);
                    translation.Value = aboveDestination;
                    var downRot = quaternion.LookRotationSafe(new float3(0, -1, 0), new float3(1, 0, 0));
                    rotation.Value = downRot;
                    movementData.LinearVelocity = math.forward() * ballisticMissileConfig.ReenterSpeed;
                }
            }
        }
    }
}