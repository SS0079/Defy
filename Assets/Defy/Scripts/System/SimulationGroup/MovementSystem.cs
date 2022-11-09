using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    [UpdateAfter(typeof(SoftCollisionSystem))]
    public partial class MovementSystem : SystemBase
    {
        private EntityQuery _WithCurveQuery;
        private EntityQuery _NoCurveQuery;
        protected override void OnCreate()
        {
            var noCurveDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    typeof(Translation),
                    typeof(MovementData),
                    typeof(Rotation)
                },
                None = new ComponentType[]
                {
                    typeof(StopMoveTimerData),
                    typeof(SampledCurveReference)
                }
            };
            var withCurveDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    typeof(Translation),
                    typeof(MovementData),
                    typeof(Rotation),
                    typeof(SampledCurveReference),
                    typeof(DestroyTimerData)
                },
                None = new ComponentType[]
                {
                    typeof(StopMoveTimerData),
                }
            };
            _NoCurveQuery = GetEntityQuery(noCurveDesc);
            _WithCurveQuery = GetEntityQuery(withCurveDesc);
            
        }

        protected override void OnUpdate()
        {
            var softCollisionJobHandle = SoftCollisionSystem.SoftCollisionJobHandle;
            var moveJobHandle = new MoveJob(){Δt = Time.DeltaTime}.ScheduleParallel(_NoCurveQuery,softCollisionJobHandle);
            Dependency= new MoveWithCurveJob() { Δt = Time.DeltaTime }.ScheduleParallel(_WithCurveQuery, moveJobHandle);
            // Dependency=JobHandle.CombineDependencies(moveJobHandle,moveWithCurveJobHandle);
        }
        
        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float Δt;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref Translation translation,
                ref MovementData movementData,
                in Rotation rotation)
            {
                var movement = (movementData.LinearVelocity+ movementData.ForceVelocity) * Δt;
                if (movementData.IsLocal)
                {
                    movement = math.mul(rotation.Value, movement);
                }
                translation.Value += movement;
            }
        }
        [BurstCompile]
        private partial struct MoveWithCurveJob : IJobEntity
        {
            public float Δt;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref Translation translation,
                ref MovementData movementData,
                in Rotation rotation,
                in DestroyTimerData destroyTimerData,
                in SampledCurveReference curveReference)
            {
                var movement = (movementData.LinearVelocity+ movementData.ForceVelocity) * Δt*curveReference.GetValueAtTime(0,destroyTimerData.RamainFraction());
                if (movementData.IsLocal)
                {
                    movement = math.mul(rotation.Value, movement);
                }
                translation.Value += movement;
            }
        }
    }
}