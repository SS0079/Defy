using Defy.Component;
using Defy.Utilities;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Core;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    [UpdateAfter(typeof(SetAimPosSystem))]
    public partial class EnemySpawnSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _EcbSys;
        private BuildPhysicsWorld _PhysicsWorld;
        protected override void OnCreate()
        {
            _EcbSys = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _PhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            RequireSingletonForUpdate<TerminalData>();
        }

        protected override void OnUpdate()
        {
            var ecb = _EcbSys.CreateCommandBuffer().AsParallelWriter();
            var collisionWorld = _PhysicsWorld.PhysicsWorld.CollisionWorld;
            new SpawnEnemyJob() { Δt = Time.DeltaTime, PlayerPos = float3.zero, CWorld = collisionWorld, EcbParallelWriter = ecb }.ScheduleParallel();
            _EcbSys.AddJobHandleForProducer(Dependency);
        }
        
        [BurstCompile]
        private partial struct SpawnEnemyJob : IJobEntity
        {
            public float Δt;
            public float3 PlayerPos;
            [ReadOnly] public CollisionWorld CWorld;
            public EntityCommandBuffer.ParallelWriter EcbParallelWriter;

            //TODO: change spawning logic to a group spawn pattern
            public void Execute(
                in Entity e,
                [EntityInQueryIndex] int index,
                ref EnemySpawnerConfig enemySpawnerConfig,
                ref IndividualRandomData randomData)
            {
                //return if Do spawn is false
                if (!enemySpawnerConfig.DoSpawn) return;
                //return if frame rate too low
                if(1/Δt<enemySpawnerConfig.FrameRateThreshold) return;
                //return if max spawn is set valid and have exceed max spawn
                if (enemySpawnerConfig.MaxSpawn>0 && enemySpawnerConfig.TotalSpawned>=enemySpawnerConfig.MaxSpawn) return;
                var spawnCheckFilter = new CollisionFilter()
                {
                    BelongsTo = (uint)CollisionLayers.Caster,
                    CollidesWith = (uint)CollisionLayers.Enemy
                };
                enemySpawnerConfig.SpawnCountDown -= Δt;
                if (enemySpawnerConfig.SpawnCountDown<=0)
                {
                    //calculate spawn zone scale
                    var spawnZoneScale = math.sqrt(enemySpawnerConfig.SpawnCount);
                    spawnZoneScale = math.ceil(spawnZoneScale);
                    var randFloat2 = randomData.Value.NextFloat2Direction();
                    var length = randomData.Value.NextFloat(enemySpawnerConfig.MinRange, enemySpawnerConfig.MaxRange);
                    var spawnCenter = PlayerPos + new float3(randFloat2.x, 0, randFloat2.y) * length;
                    
                    //spawn in circle randomly
                    var circleRadius = spawnZoneScale * enemySpawnerConfig.Space;
                    //if this position is taken, skip this spawn
                    if (CWorld.CheckSphere(spawnCenter,circleRadius,spawnCheckFilter)) return;
                    for (int i = 0; i < enemySpawnerConfig.SpawnCount; i++)
                    {
                        // var randDirInCircle = randomData.Value.NextFloat2Direction();
                        // var randLengthInCircle = (1-math.pow(randomData.Value.NextFloat(), 2))*circleRadius;
                        // var randPointInCircle = spawnCenter + new float3(randDirInCircle.x, 0, randDirInCircle.y) * randLengthInCircle;
                        var randPointInCircle = RandomUtility.RandomPositionOnDisk(ref randomData.Value, spawnCenter, circleRadius);
                        var newEnemy=EcbParallelWriter.Instantiate(index, enemySpawnerConfig.Enemy);
                        EcbParallelWriter.SetComponent(index,newEnemy,new Translation(){Value = randPointInCircle});
                        enemySpawnerConfig.TotalSpawned++;
                    }
                    
                    // //spawn in matrix
                    // var spawnStartPoint = new float3(spawnZoneScale / 2f*enemySpawnerConfig.Space, 5, spawnZoneScale / 2f*enemySpawnerConfig.Space);
                    // //if this position is taken, skip this spawn
                    // if (CWorld.CheckBox(
                    //         spawnCenter, 
                    //         quaternion.identity, 
                    //         spawnStartPoint, 
                    //         spawnCheckFilter)
                    //    ) return;
                    //
                    // for (int i = 0; i < spawnZoneScale; i++)
                    // {
                    //     //row
                    //     for (int j = 0; j < spawnZoneScale; j++)
                    //     {
                    //         //colum
                    //         var spawnOffset = new float3(i * enemySpawnerConfig.Space, 0, j * enemySpawnerConfig.Space);
                    //         var spawnPos = spawnStartPoint - spawnOffset+spawnCenter;
                    //         var newEnemy=EcbParallelWriter.Instantiate(index, enemySpawnerConfig.Enemy);
                    //         EcbParallelWriter.SetComponent(index,newEnemy,new Translation(){Value = spawnPos});
                    //         enemySpawnerConfig.TotalSpawned++;
                    //     }
                    // }
                    
                    
                    enemySpawnerConfig.SpawnCountDown = enemySpawnerConfig.Interval;
                }
            }
        }
    }
}