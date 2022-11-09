using Defy.Component;
using Defy.Utilities;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    // [UpdateAfter(typeof(BuildPhysicsWorld))]
    public partial class SoftCollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _PhysicsWorld;
        public static JobHandle SoftCollisionJobHandle;
        protected override void OnCreate()
        {
            _PhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<SimpleCharacterMoveConfig>()));
        }

        protected override void OnUpdate()
        {
            var collisionWorld = _PhysicsWorld.PhysicsWorld.CollisionWorld;
            SoftCollisionJobHandle = new KeepSocialDistanceJob() { CWorld = collisionWorld,Δt = Time.DeltaTime}.ScheduleParallel();

        }
        
        [BurstCompile]
        private partial struct KeepSocialDistanceJob : IJobEntity
        {
            public float Δt;
            [ReadOnly] public CollisionWorld CWorld;
            
            //calculate the movement to keep social distance by keep distance with all surrounding character, move slowly
            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref Translation translation,
                ref MovementData movementData,
                in SimpleCharacterMoveConfig characterMoveConfig,
                in PhysicsCollider collider)
            {
                NativeList<DistanceHit> overlapHits = new NativeList<DistanceHit>(Allocator.Temp);
                float3 addUpRepulsion=float3.zero;
                //check if there are other mover with in social distance, and add up all repulsion
                if (CWorld.OverlapSphere(translation.Value, characterMoveConfig.SocialDistance, ref overlapHits,collider.Value.Value.Filter))
                {
                    SimplePhysicsUtility.TrimByEntity(ref overlapHits,e);
                    if (overlapHits.Length>0 )
                    {
                        if (overlapHits.Length<12)
                        {
                            for (int i = 0; i < overlapHits.Length; i++)
                            {
                                var neighbourOffset = translation.Value - overlapHits[i].Position;
                                var dis = overlapHits[i].Distance;
                                float3 force = math.normalizesafe(neighbourOffset) * (1-dis/characterMoveConfig.SocialDistance);
                                addUpRepulsion += force;
                            }
                            movementData.ForceVelocity = addUpRepulsion*characterMoveConfig.RepulsionForce;
                            //if add up repulsion is on the very opposite direction of velocity, zero velocity to stop moving
                            if (math.dot(movementData.LinearVelocity,movementData.ForceVelocity)<-0.86)//angle larger than 150 degree
                            {
                                movementData.LinearVelocity=float3.zero;
                            }
                        }
                        else
                        {
                            //more than 8 neighbour means too crewed, just stand still
                            movementData.ForceVelocity=float3.zero;
                            movementData.LinearVelocity=float3.zero;
                        }
                    }
                }
            }
        }
        
  
    }
}