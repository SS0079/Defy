using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyFinalUpdateGroup))]
    public partial class EnemyAttackSystem : SystemBase
    {
        private Entity _TerminalEntity;
        private float3 _TerminalPos;
        private BeginInitializationEntityCommandBufferSystem _EcbSys;
        private EntityManager _EntityManager;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnStartRunning()
        {
            RequireSingletonForUpdate<TerminalData>();
        }

        protected override void OnUpdate()
        {
            _TerminalEntity = GetSingletonEntity<TerminalData>();
            _TerminalPos=_EntityManager.GetComponentData<Translation>(_TerminalEntity).Value;
            var allCombatData = GetComponentDataFromEntity<CombatData>();
            var writer = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            new EnemyAttackJob() { Writer = writer, TerminalPos = _TerminalPos, TerminalEntity = _TerminalEntity, AllCombatData = allCombatData }.ScheduleParallel();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        private partial struct EnemyAttackJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter Writer;
            public float3 TerminalPos;
            public Entity TerminalEntity;
            [ReadOnly]public ComponentDataFromEntity<CombatData> AllCombatData;
            //add damage to Gate combatData than self destroy
            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                in EnemyAttackConfig enemyAttackConfig,
                in Translation translation,
                ref DestroyFlagData destroyFlagData)
            {
                if (!AllCombatData.HasComponent(TerminalEntity)) return;
                //if enemy come too close, attack
                if (math.distancesq(translation.Value,TerminalPos)<=enemyAttackConfig.FuzeLength*enemyAttackConfig.FuzeLength)
                {
                    var gateCombatData = AllCombatData[TerminalEntity];
                    gateCombatData.IncomingDamage += enemyAttackConfig.Damage;
                    Writer.SetComponent(index,TerminalEntity,gateCombatData);
                    var explodeVfxEntity = Writer.Instantiate(index, enemyAttackConfig.ExplodeVFX);
                    Writer.SetComponent(index,explodeVfxEntity,translation);
                    Writer.AddComponent(index,TerminalEntity,new VfxCountDownData(){CountDown = 0.05f});
                    destroyFlagData.SetYes();
                }
            }
        }
    }
}