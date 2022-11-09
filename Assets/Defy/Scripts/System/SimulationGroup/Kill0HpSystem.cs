using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyFinalUpdateGroup))]
    public partial class Kill0HpSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _EcbSys;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbp = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            new KillJob() { ecbp = ecbp }.ScheduleParallel();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        private partial struct KillJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecbp;

            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                in CombatData combatData,
                in DeadVFXConfig deadVFXConfig,
                in Translation translation,
                ref DestroyFlagData destroyFlagData)
            {
                if (combatData.HP<=0)
                {
                    var emitterEntity = ecbp.Instantiate(index, deadVFXConfig.ParticleEmitter);
                    ecbp.SetComponent(index,emitterEntity,translation);
                    destroyFlagData.SetYes();
                }
            }
        }
    }
}