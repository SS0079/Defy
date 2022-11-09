using System;
using Defy.Component;
using Defy.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Defy.System
{
    
    [UpdateInGroup(typeof(DefyUpdateGroup),OrderLast = true)]
    public partial class HitCheckSystem : SystemBase
    {
        private BuildPhysicsWorld _BuildPhysicsWorld;
        private float _HitVfxSpan;
        public static JobHandle HitCheckJobHandle;
        private EndSimulationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            _BuildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _EcbSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            _HitVfxSpan = GetSingleton<VFXConfig>().HitEffectFrame;
        }

        protected override void OnUpdate()
        {
            var ecbParallel = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            var collisionWorld = _BuildPhysicsWorld.PhysicsWorld.CollisionWorld;
            var allCombatData = GetComponentDataFromEntity<CombatData>(true);
            var allVfxData = GetComponentDataFromEntity<VfxCountDownData>(true);
            HitCheckJobHandle=new HitCheckJob()
                {
                    Writer = ecbParallel, 
                    HitVfxSpan = _HitVfxSpan, 
                    CWorld = collisionWorld,
                    CombatData = allCombatData,
                    VfxData = allVfxData,
                    Δt = Time.DeltaTime
                }
            .ScheduleParallel();
            Dependency = HitCheckJobHandle;
            _EcbSys.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        private partial struct HitCheckJob : IJobEntity
        {
            public float HitVfxSpan;
            public float Δt;
            [ReadOnly]public CollisionWorld CWorld;
            [ReadOnly]public ComponentDataFromEntity<CombatData> CombatData;
            [ReadOnly]public ComponentDataFromEntity<VfxCountDownData> VfxData;
            public EntityCommandBuffer.ParallelWriter Writer;
            public void Execute(
                Entity e, 
                [EntityInQueryIndex]int index,
                in Translation translation, 
                in LocalToWorld localToWorld,
                ref BulletConfig bulletConfig,
                in SubEntityConfig subEntityConfig,
                ref DestroyFlagData destroyFlagData)
            {
                CollisionFilter filter;
                if (bulletConfig.Params.IgnoreGround)
                {
                    filter = new CollisionFilter()
                    {
                        BelongsTo = (uint)CollisionLayers.Caster,
                        CollidesWith = (uint)CollisionLayers.Enemy
                    };
                }
                else
                {
                    filter = new CollisionFilter()
                    {
                        BelongsTo = (uint)CollisionLayers.Caster,
                        CollidesWith = (uint)CollisionLayers.Enemy | (uint)CollisionLayers.Ground
                    };
                }
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                if (CWorld.SphereCastAll(translation.Value,bulletConfig.Params.FuzeLength, math.normalizesafe(localToWorld.Forward),bulletConfig.Params.Speed*Δt,ref hits,filter))
                {
                    var hit0Pos = hits[0].Position;
                    switch (bulletConfig.Params.Type)
                    {
                        case HitType.Normal:
                            // deal hit to every cast result
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (CombatData.HasComponent(hits[i].Entity))
                                {
                                    var combatCache = CombatData[hits[i].Entity];
                                    combatCache.IncomingDamage += bulletConfig.Params.Damage;
                                    combatCache.KnockBackVelocity += -hits[i].SurfaceNormal * bulletConfig.Params.KnockBack;
                                    Writer.SetComponent(index,hits[i].Entity,combatCache);
                                    // CombatData[hits[i].Entity] = combatCache;
                                    if (!VfxData.HasComponent(hits[i].Entity))
                                    {
                                        Writer.AddComponent(index,hits[i].Entity,new VfxCountDownData(){CountDown = HitVfxSpan});
                                    }
                                }
                                bulletConfig.Params.Penetrate--;
                                if (bulletConfig.Params.Penetrate<=0)
                                {
                                    destroyFlagData.SetYes();
                                }
                            }
                            break;
                        case HitType.Nova:
                            // deal hit to every cast result
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (CombatData.HasComponent(hits[i].Entity))
                                {
                                    var combatCache = CombatData[hits[i].Entity];
                                    combatCache.IncomingDamage += bulletConfig.Params.Damage;
                                    combatCache.KnockBackVelocity += -hits[i].SurfaceNormal * bulletConfig.Params.KnockBack;
                                    Writer.SetComponent(index,hits[i].Entity,combatCache);
                                    if (!VfxData.HasComponent(hits[i].Entity))
                                    {
                                        Writer.AddComponent(index,hits[i].Entity,new VfxCountDownData(){CountDown = HitVfxSpan});
                                    }
                                }
                                bulletConfig.Params.Penetrate--;
                                if (bulletConfig.Params.Penetrate<=0)
                                {
                                    destroyFlagData.SetYes();
                                }
                            }
                            //explode on first hit
                            for (int i = 0; i < 200; i++)
                            {
                                var subEntity=Writer.Instantiate(index,subEntityConfig.Value1);
                                Writer.SetComponent(index,subEntity,new Translation(){Value = new float3(hit0Pos.x,0.5f,hit0Pos.z)});
                                //shrapnel halo should arranged equally, not randomly
                                var arrangedRot = quaternion.AxisAngle(new float3(0, 1, 0), math.PI*2*i/200);
                                Writer.SetComponent(index,subEntity,new Rotation(){Value = arrangedRot});
                            }
                            //if subentity2 is not null ,instantiate it, it should be a vfx
                            if (subEntityConfig.Value2!=null)
                            {
                                var subEntity = Writer.Instantiate(index, subEntityConfig.Value2);
                                Writer.SetComponent(index,subEntity,new Translation(){Value = hit0Pos});
                            }
                            //Nova type count penetrate only once per hit
                            bulletConfig.Params.Penetrate--;
                            if (bulletConfig.Params.Penetrate<=0)
                            {
                                destroyFlagData.SetYes();
                            }
                            break;
                        
                        case HitType.Explode:
                            {
                                //generate particle emitter
                                var subEntity=Writer.Instantiate(index,subEntityConfig.Value1);
                                var randSource = new IndividualRandomData();
                                randSource.Init(index);
                                Writer.SetComponent(index ,subEntity,randSource);
                                Writer.SetComponent(index,subEntity,translation);
                                //overlap sphere to deal damage
                                NativeList<DistanceHit> damageHits=new NativeList<DistanceHit>(Allocator.Temp);
                                if (CWorld.OverlapSphere(translation.Value, bulletConfig.Params.DamageRadius, ref damageHits, filter))
                                {
                                    SimplePhysicsUtility.TrimByEntity(ref damageHits,e);
                                    for (int i = 0; i < damageHits.Length; i++)
                                    {
                                        if (CombatData.HasComponent(damageHits[i].Entity))
                                        {
                                            var combatCache = CombatData[damageHits[i].Entity];
                                            combatCache.IncomingDamage += bulletConfig.Params.Damage;
                                            combatCache.KnockBackVelocity += -damageHits[i].SurfaceNormal * bulletConfig.Params.KnockBack;
                                            Writer.SetComponent(index,damageHits[i].Entity,combatCache);
                                            // CombatData[hits[i].Entity] = combatCache;
                                            if (!VfxData.HasComponent(damageHits[i].Entity))
                                            {
                                                Writer.AddComponent(index,damageHits[i].Entity,new VfxCountDownData(){CountDown = HitVfxSpan});
                                            }
                                        }
                                    }
                                }
                                bulletConfig.Params.Penetrate--;
                                if (bulletConfig.Params.Penetrate<=0)
                                {
                                    destroyFlagData.SetYes();
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}