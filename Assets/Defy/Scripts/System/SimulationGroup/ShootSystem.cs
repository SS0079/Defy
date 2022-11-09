using System;
using Defy.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Defy.System
{
    
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    public partial class ShootSystem : SystemBase
    {
        private EntityQuery _ReadyToShootQuery;
        private BeginSimulationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            var readyToShootQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TriggerFlagData) },
                None = new ComponentType[] { typeof(DelayShootData) }
            };
            _ReadyToShootQuery = GetEntityQuery(readyToShootQueryDesc);
            _EcbSys = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }


        protected override void OnUpdate()
        {
            var writer = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            var Δt = Time.DeltaTime;

            //spawn new shell, move it to muzzle pos, rotate it forward, remove shouldShootTag, add DelayBeforeShoot
            new SimpleShootWithSpreadJob() { Writer = writer }.ScheduleParallel();
            
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        
        [BurstCompile]
        private partial struct SimpleShootWithSpreadJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Writer;

            public void Execute(
                in Entity e, 
                [EntityInQueryIndex] int index,
                ref IndividualRandomData randomData,
                in Translation translation,
                in Rotation rotation, 
                ref WeaponConfig weaponConfig,
                ref AmmoCountData ammoCountData,
                in AimPosData aimPos,
                ref ResetableTimerData timer,
                in TriggerFlagData triggerFlagData)
            {
                if (timer.Value.Ready && triggerFlagData.Value)
                {
                    if (ammoCountData.Value>0)
                    {
                        switch (weaponConfig.Type)
                        {
                            case WeaponType.FullAuto:
                                for (int i = 0; i < weaponConfig.Salvo; i++)
                                {
                                    var bullet=Writer.Instantiate(index,weaponConfig.BulletEntity);
                                    Writer.SetComponent(index,bullet,new Translation(){Value = translation.Value});
                                    var radianSpread = math.radians(weaponConfig.SpreadDegree)/2f;
                                    var offsetRot = quaternion.AxisAngle(new float3(0, 1, 0), randomData.Value.NextFloat(-radianSpread, radianSpread));
                                    var randomRot = math.mul(offsetRot, rotation.Value);
                                    Writer.SetComponent(index,bullet,new Rotation(){Value = randomRot});
                                    Writer.SetComponent(index,bullet,new BulletConfig(){Params = weaponConfig.BulletParams});
                                    Writer.SetComponent(index,bullet,new MovementData(){IsLocal = true,LinearVelocity = new float3(0,0,weaponConfig.BulletParams.Speed)});
                                    Writer.SetComponent(index,bullet,new DestroyTimerData().Init(weaponConfig.BulletParams.LifeSpan));
                                }
                                break;
                            case WeaponType.MIRV:
                            {
                                //MIRV create a emitter to generate bullet rain
                                for (int i = 0; i < weaponConfig.Salvo; i++)
                                {
                                    var bullet=Writer.Instantiate(index,weaponConfig.BulletEntity);
                                    Writer.SetComponent(index,bullet,new Translation(){Value = translation.Value+new float3(0,5,0)});
                                    Writer.AddComponent(index,bullet,new DestinationData(){Value = aimPos.Value});
                                    Writer.AddComponent(index,bullet,new IndividualRandomData(){Value = Random.CreateFromIndex((uint)index)});
                                    Writer.SetComponent(index,bullet,new Rotation(){Value = quaternion.LookRotationSafe(new float3(0,1,0),new float3(1,0,0))});
                                    Writer.SetComponent(index,bullet,new BulletConfig(){Params = weaponConfig.BulletParams});
                                    Writer.SetComponent(index,bullet,new MovementData(){IsLocal = true,LinearVelocity = new float3(0,0,weaponConfig.BulletParams.Speed)});
                                    Writer.SetComponent(index,bullet,new DestroyTimerData().Init(weaponConfig.BulletParams.LifeSpan));
                                }
                            }
                                break;
                            case WeaponType.BallisticMissile:
                            {
                                for (int i = 0; i < weaponConfig.Salvo; i++)
                                {
                                    var bullet=Writer.Instantiate(index,weaponConfig.BulletEntity);
                                    Writer.SetComponent(index,bullet,new Translation(){Value = translation.Value+new float3(0,5,0)});
                                    Writer.AddComponent(index,bullet,new DestinationData(){Value = aimPos.Value});
                                    Writer.SetComponent(index,bullet,new Rotation(){Value = quaternion.LookRotationSafe(new float3(0,1,0),new float3(1,0,0))});
                                    Writer.SetComponent(index,bullet,new BulletConfig(){Params = weaponConfig.BulletParams});
                                    Writer.SetComponent(index,bullet,new MovementData(){IsLocal = true,LinearVelocity = new float3(0,0,weaponConfig.BulletParams.Speed)});
                                    Writer.SetComponent(index,bullet,new DestroyTimerData().Init(weaponConfig.BulletParams.LifeSpan));
                                }
                            }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        //cool down after all bullet shot
                        
                        timer.Value.Reset();
                        
                        // ecb.AddComponent(index,e,new DelayShootData(){Value = weaponConfig.DelayBetweenShoot});
                        // ecb.RemoveComponent<ShouldShootTag>(index,e);
                        
                        ammoCountData.Value--;
                    }
                }
                
            }
        }
        
       
    }
}