using Defy.Component;
using Rewired;
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
        private Player _PlayerInput;

        protected override void OnStartRunning()
        {
            _PlayerInput = ReInput.players.GetPlayer(0);
        }

        protected override void OnUpdate()
        {
            float x = _PlayerInput.GetAxis("Horizontal");
            float y = _PlayerInput.GetAxis("Vertical");
            bool jump = _PlayerInput.GetButtonDown("Jump");
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