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
    public partial class MIRVReenterSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _EcbSys;
        protected override void OnStartRunning()
        {
            _EcbSys = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbp = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            var jobHandle = new MIRVReenterJob() { Writer = ecbp }.ScheduleParallel(Dependency);
            jobHandle.Complete();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        private partial struct MIRVReenterJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Writer;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref Translation translation,
                ref IndividualRandomData randomData,
                in DestinationData destinationData,
                in MIRVConfig mirvConfig,
                in SubEntityConfig subEntityConfig)
            {
                //if missile ascend above ceiling, commence fail down
                if (translation.Value.y>mirvConfig.Ceiling)
                {
                    //instantiate a emitter to spawn reenter vehicle
                    var vehicleEmitter = Writer.Instantiate(index, subEntityConfig.Value1);
                    Writer.SetComponent(index,vehicleEmitter,new Translation(){Value = new float3(destinationData.Value.x,mirvConfig.Ceiling,destinationData.Value.z)});
                    Writer.SetComponent(index,vehicleEmitter,new EntityParticleEmitterConfigData()
                    {
                        ParticleEntity = subEntityConfig.Value2,
                        SpeedRange = float2.zero,
                        LifeSpanRange = float2.zero,
                        Type = EmitterType.Disk,
                        InitDirection = math.down(),
                        Scale = mirvConfig.SplitRange,
                        Salvo = mirvConfig.Salvo,
                        Loop =math.max(1,mirvConfig.SplitCount/mirvConfig.Salvo),
                        Interval = mirvConfig.Interval,
                    });
                    //destroy the Carrier vehicle
                    Writer.DestroyEntity(index,e);
                }
            }
        }
    }
}