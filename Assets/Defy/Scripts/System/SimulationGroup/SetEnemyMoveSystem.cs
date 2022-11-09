using Defy.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    [UpdateBefore(typeof(SoftCollisionSystem))]
    public partial class SetEnemyMoveSystem : SystemBase
    {
        private float3 _GatePosition;
        protected override void OnStartRunning()
        {
            RequireSingletonForUpdate<TerminalData>();
        }

        protected override void OnUpdate()
        {
            new SetEnemyDestinationAsPlayer() { PlayerPos = float3.zero }.ScheduleParallel();
            new SetEnemyMoveWithSoftCollisionJob().ScheduleParallel();
        }
        
        [BurstCompile]
        private partial struct SetEnemyDestinationAsPlayer : IJobEntity
        {
            public float3 PlayerPos;
            
            public void Execute(ref EnemyConfig enemyConfig)
            {
                enemyConfig.Destination = PlayerPos;
            }
        }
        
        [BurstCompile]
        private partial struct SetEnemyMoveWithCollideJob : IJobEntity
        {

            public void Execute(in EnemyConfig enemyConfig, ref SimpleCharacterMovementData characterMovementData,in SimpleCharacterMoveConfig characterMoveConfig,in Translation translation)
            {
                var destination = enemyConfig.Destination - translation.Value;
                characterMovementData.ControlledVelocity = math.normalizesafe(destination) * characterMoveConfig.MoveSpeed;
            }
        }
        
        [BurstCompile]
        private partial struct SetEnemyMoveWithSoftCollisionJob : IJobEntity
        {

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref MovementData softCollisionMovementData,
                in Translation translation,
                in EnemyConfig enemyConfig,
                in SimpleCharacterMoveConfig characterMoveConfig)
            {
                var destination = enemyConfig.Destination - translation.Value;
                softCollisionMovementData.LinearVelocity=math.normalizesafe(destination) * characterMoveConfig.MoveSpeed;
                // Debug.Log(destination);
            }
        }
    }
}