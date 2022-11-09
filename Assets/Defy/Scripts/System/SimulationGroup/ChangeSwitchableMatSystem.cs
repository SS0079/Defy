using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyLateUpdateGroup))]
    public partial class ChangeSwitchableMatSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            float Δt = Time.DeltaTime;
            var writer = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            new LerpMatJob() { Δt = Δt, Writer = writer }.ScheduleParallel();
            new FadeMatJob() { Δt = Δt }.ScheduleParallel();
        }
        
        [BurstCompile]
        private partial struct LerpMatJob : IJobEntity
        {
            public float Δt;
            public EntityCommandBuffer.ParallelWriter Writer;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref SwitchableMatOverrideData matOverride,
                ref VfxCountDownData frameCountData)
            {
                if (frameCountData.CountDown<=0)
                {
                    //count down end, reset matOverride to 0, than remove count down data
                    matOverride.Value = 0;
                    Writer.RemoveComponent<VfxCountDownData>(index,e);
                }
                else
                {
                    frameCountData.CountDown -= Δt;
                    matOverride.Value = 1;
                }
            }
        }
        
        [BurstCompile]
        private partial struct FadeMatJob : IJobEntity
        {
            public float Δt;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                in DestroyTimerData destroyTimerData,
                ref FadeMatOverrideData fadeMatOverrideData,
                in SampledCurveReference curveReference)
            {
                fadeMatOverrideData.Value = curveReference.GetValueAtTime(1,destroyTimerData.RamainFraction());
            }
        }
    }
}