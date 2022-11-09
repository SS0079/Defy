using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System.InitializationGroup
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TimerSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var writer = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            var Δt = Time.DeltaTime;
            new ResetableTimerJob() { Δt = Δt }.ScheduleParallel();
            new DelayedDestroyJob() { Δt = Δt, Writer = writer }.ScheduleParallel();
            new StopMoveTimerJob() { Δt = Δt, Writer = writer }.ScheduleParallel();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        private partial struct ResetableTimerJob : IJobEntity
        {
            public float Δt;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref ResetableTimerData timer)
            {
                if (!timer.Value.Ready)
                {
                    if (timer.Value.Current > 0)
                    {
                        timer.Value.Current -= Δt;
                    }
                    else
                    {
                        timer.Value.Ready = true;
                    }
                }
            }
        }
        
        [BurstCompile]
        private partial struct DelayedDestroyJob : IJobEntity
        {
            public float Δt;
            public EntityCommandBuffer.ParallelWriter Writer;

            public void Execute(in Entity e, [EntityInQueryIndex] int index, ref DestroyTimerData destroyTimerData)
            {
                destroyTimerData.Current -= Δt;
                if (destroyTimerData.Current<=0)
                {
                    Writer.DestroyEntity(index,e);
                }
            }
        }
        
                
        [BurstCompile]
        private partial struct StopMoveTimerJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Writer;
            public float Δt;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref StopMoveTimerData stopMoveTimerData)
            {
                stopMoveTimerData.Value -= Δt;
                if (stopMoveTimerData.Value<=0)
                {
                    Writer.RemoveComponent<StopMoveTimerData>(index,e);
                }
            }
        }
    }
}