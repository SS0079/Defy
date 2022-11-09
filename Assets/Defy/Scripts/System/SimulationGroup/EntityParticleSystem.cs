using System;
using Defy.Component;
using Defy.Utilities;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyLateUpdateGroup))]
    public partial class EntityParticleSystem : SystemBase
    {
        private EntityQuery _WithoutCurveQuery;
        private static readonly quaternion NorthPoleRotation=quaternion.RotateX(math.radians(-90));
        
        private struct EmitInput
        {
            public Entity ParticleEntity;
            public EntityParticleData ParticleData;
            public SampledCurveReference CurveReference;
            public float3 Position;
            public quaternion Rotation;
        }
        private BeginInitializationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var withoutCurveDesc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    typeof(EntityParticleEmitterConfigData),
                    typeof(Translation),
                    typeof(IndividualRandomData),
                    typeof(DestroyFlagData)
                },
                None = new ComponentType[]
                {
                    typeof(SampledCurveReference)
                }
            };
            _WithoutCurveQuery = GetEntityQuery(withoutCurveDesc);
            
        }

        protected override void OnUpdate()
        {
            var writer = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            var Δt = Time.DeltaTime;
            new EmitNoCurveJob() { Δt = Δt, Writer = writer }.ScheduleParallel(_WithoutCurveQuery);
            new EmitWithCurveJob() { Δt = Δt, Writer = writer }.ScheduleParallel();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        //emit without curve
        private partial struct EmitNoCurveJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Writer;
            public float Δt;
            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref EntityParticleEmitterConfigData particleEmitterConfigData,
                in Translation translation,
                ref IndividualRandomData individualRandomData,
                ref DestroyFlagData destroyFlagData)
            {
                if(individualRandomData.Value.state==0)
                {
                    individualRandomData.Init(index);
                }
                if (particleEmitterConfigData.Loop>0 )
                {
                    if (particleEmitterConfigData.CountDown<=0)
                    {
                        for (int j = 0; j < particleEmitterConfigData.Salvo; j++)
                        {
                            var randSpeed = particleEmitterConfigData.SpeedRange.x>0 && particleEmitterConfigData.SpeedRange.y>0?
                                individualRandomData.Value.NextFloat(particleEmitterConfigData.SpeedRange.x, particleEmitterConfigData.SpeedRange.y):
                                0;
                            var randLife = particleEmitterConfigData.LifeSpanRange.x>0 && particleEmitterConfigData.LifeSpanRange.y>0? 
                                individualRandomData.Value.NextFloat(particleEmitterConfigData.LifeSpanRange.x, particleEmitterConfigData.LifeSpanRange.y):
                                0;
                            var particleData = new EntityParticleData()
                            {
                                Speed = randSpeed,
                                LifeSpan = randLife,
                            };
                            EmitInput input=new EmitInput()
                            {
                                ParticleEntity = particleEmitterConfigData.ParticleEntity,
                                ParticleData = particleData
                            };
                            switch (particleEmitterConfigData.Type)
                            {
                                case EmitterType.Sphere:
                                    input.Position = translation.Value;
                                    input.Rotation = individualRandomData.Value.NextQuaternionRotation();
                                    break;
                                case EmitterType.NorthernHemisphere:
                                    input.Position = translation.Value;
                                    quaternion randRot=quaternion.identity;
                                    do
                                    {
                                        randRot = individualRandomData.Value.NextQuaternionRotation();
                                    } while (Quaternion.Angle(randRot, NorthPoleRotation)>90);
                                    input.Rotation = randRot;
                                    break;
                                case EmitterType.Disk:
                                    input.Position = RandomUtility.RandomPositionOnDisk(ref individualRandomData.Value, translation.Value, particleEmitterConfigData.Scale);
                                    input.Rotation=Quaternion.LookRotation(particleEmitterConfigData.InitDirection);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            Emit(index,input,Writer);
                        }
                        particleEmitterConfigData.Loop--;
                        //reset count down timer
                        particleEmitterConfigData.CountDown = particleEmitterConfigData.Interval;
                    }
                    else
                    {
                        particleEmitterConfigData.CountDown -= Δt;
                    }
                }
                else
                {
                    //destroy emitter if loop deplete
                    // Writer.DestroyEntity(index,e);
                    destroyFlagData.SetYes();
                }
            }


 
        }

        [BurstCompile]
        //emit with curve
        private partial struct EmitWithCurveJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Writer;
            public float Δt;
        
            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref EntityParticleEmitterConfigData particleEmitterConfigData,
                in Translation translation,
                ref IndividualRandomData individualRandomData,
                ref DestroyFlagData destroyFlagData,
                in SampledCurveReference curveReference)
            {
                if (individualRandomData.Value.state == 0)
                {
                    individualRandomData.Init(index);
                }
                if (particleEmitterConfigData.Loop > 0)
                {
                    if (particleEmitterConfigData.CountDown <= 0)
                    {
                        for (int j = 0; j < particleEmitterConfigData.Salvo; j++)
                        {
                            var randSpeed = particleEmitterConfigData.SpeedRange.x>0 && particleEmitterConfigData.SpeedRange.y>0?
                                individualRandomData.Value.NextFloat(particleEmitterConfigData.SpeedRange.x, particleEmitterConfigData.SpeedRange.y):
                                0;
                            var randLife = particleEmitterConfigData.LifeSpanRange.x>0 && particleEmitterConfigData.LifeSpanRange.y>0? 
                                individualRandomData.Value.NextFloat(particleEmitterConfigData.LifeSpanRange.x, particleEmitterConfigData.LifeSpanRange.y):
                                0;
                            var particleData = new EntityParticleData()
                            {
                                Speed = randSpeed,
                                LifeSpan = randLife,
                            };
                            EmitInput input = new EmitInput()
                            {
                                ParticleEntity = particleEmitterConfigData.ParticleEntity,
                                ParticleData = particleData,
                                CurveReference = curveReference
                            };
                            switch (particleEmitterConfigData.Type)
                            {
                                case EmitterType.Sphere:
                                    input.Position = translation.Value;
                                    input.Rotation = individualRandomData.Value.NextQuaternionRotation();
                                    break;
                                case EmitterType.NorthernHemisphere:
                                    input.Position = translation.Value;
                                    quaternion randRot=quaternion.identity;
                                    do
                                    {
                                        randRot = individualRandomData.Value.NextQuaternionRotation();
                                        // var angle = ;
                                    } while (Quaternion.Angle(randRot, NorthPoleRotation)>90);
                                    input.Rotation = randRot;
                                    break;
                                case EmitterType.Disk:
                                    input.Position = RandomUtility.RandomPositionOnDisk(ref individualRandomData.Value, translation.Value, particleEmitterConfigData.Scale);
                                    input.Rotation = Quaternion.LookRotation(particleEmitterConfigData.InitDirection);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            Emit(index, input, Writer);
                        }
                        particleEmitterConfigData.Loop--;
        
                        //reset count down timer
                        particleEmitterConfigData.CountDown = particleEmitterConfigData.Interval;
                    }
                    else
                    {
                        particleEmitterConfigData.CountDown -= Δt;
                    }
                }
                else
                {
                    //destroy emitter if loop deplete
                    // Writer.DestroyEntity(index,e);
                    destroyFlagData.SetYes();
                }
            }
        }

        private static void Emit(int index,EmitInput input,EntityCommandBuffer.ParallelWriter writer)
        {
            var particleEntity = writer.Instantiate(index, input.ParticleEntity);
            writer.SetComponent(index, particleEntity, new Translation(){Value = input.Position});
            writer.AddComponent(index, particleEntity, input.ParticleData);
            writer.SetComponent(index, particleEntity, new Rotation() { Value = input.Rotation });
            if (input.ParticleData.HasValue())
            {
                writer.AddComponent(index, particleEntity, new MovementData()
                {
                    IsLocal = true,
                    LinearVelocity = new float3(0, 0, input.ParticleData.Speed)
                });
                writer.AddComponent(index, particleEntity, new DestroyTimerData().Init(input.ParticleData.LifeSpan));
            }
            if (input.CurveReference.Value0.IsCreated || input.CurveReference.Value1.IsCreated)
            {
                writer.AddComponent(index,particleEntity,input.CurveReference);
            }
        }


    }
}