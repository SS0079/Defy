using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    //Fix height should apply after soft collision and Move
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    [UpdateAfter(typeof(MovementSystem))]
    public partial class FixHeightSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            new FixHeightJob().ScheduleParallel();
        }
        [BurstCompile]
        private partial struct FixHeightJob : IJobEntity
        {
    
            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref Translation translation,
                in FixHeightConfig fixHeight)
            {
                var pos = translation.Value;
                pos.y = fixHeight.Value;
                translation.Value = pos;
            }
        }
    }
}