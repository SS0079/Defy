using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    public partial class FlameAnimateSystem : SystemBase
    {
        

        protected override void OnUpdate()
        {
            new AnimateJob() { Δt = 1 * Time.DeltaTime }.ScheduleParallel();
        }
        
        [BurstCompile]
        private partial struct AnimateJob : IJobEntity
        {
            public float Δt;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref  NonUniformScale scale,
                ref URPMaterialPropertyBaseColor baseColor,
                in IsFlameTag flameTag)
            {
                scale.Value *= 1 + Δt*2;
                baseColor.Value.w *=0.97f;
            }
        }
    }
}