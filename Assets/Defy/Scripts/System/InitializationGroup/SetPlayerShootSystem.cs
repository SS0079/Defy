using Defy.Component;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Defy.System
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SetPlayerShootSystem : SystemBase
    {
        private EntityQuery _EntityQuery;

        protected override void OnUpdate()
        {
            bool trigger=Input.GetMouseButton(0);
            new SetShootingJob() { Trigger = trigger}.ScheduleParallel();
        }
        
        [BurstCompile]
        private partial struct SetShootingJob : IJobEntity
        {
            public bool Trigger;

            public void Execute(in Entity e,
                [EntityInQueryIndex] int index,
                ref TriggerFlagData triggerFlag,
                in PlayerTag playerTag)
            {
                triggerFlag.Value = Trigger;
            }
        }
        
    }
}