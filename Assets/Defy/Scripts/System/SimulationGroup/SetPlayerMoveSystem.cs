using Defy.Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    [UpdateAfter(typeof(SetAimPosSystem))]
    public partial class SetPlayerMoveSystem : SystemBase
    {

        protected override void OnStartRunning()
        {
        }

        protected override void OnUpdate()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            float3 direction = new float3(x,0, y);
            new SetMoveJob() { Direction = direction }.ScheduleParallel();
        }
        
        [BurstCompile]
        private partial struct SetMoveJob : IJobEntity
        {
            public float3 Direction;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref MovementData softCollisionMovementData,
                in SimpleCharacterMoveConfig characterMoveConfig,
                in PlayerTag playerTag)
            {
                softCollisionMovementData.LinearVelocity=math.normalizesafe(Direction)*characterMoveConfig.MoveSpeed;
            }
        }
    }
}