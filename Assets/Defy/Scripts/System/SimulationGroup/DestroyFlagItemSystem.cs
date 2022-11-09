using Defy.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Defy.System
{
    [UpdateInGroup(typeof(DefyFinalUpdateGroup),OrderLast = true)]
    public partial class DestroyFlagItemSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            new DestroyItemJob() { ecb = ecb }.ScheduleParallel();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        private partial struct DestroyItemJob: IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;

            public void Execute([EntityInQueryIndex] int index, in Entity e, ref DestroyFlagData destroyFlag)
            {
               if(destroyFlag.Yes) ecb.DestroyEntity(index, e);
            }
        }

    }
}