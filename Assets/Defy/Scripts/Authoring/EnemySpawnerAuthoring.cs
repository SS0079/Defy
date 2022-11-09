using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Defy.Component
{
    [DisallowMultipleComponent]
    public class EnemySpawnerAuthoring : MonoBehaviour ,IConvertGameObjectToEntity,IDeclareReferencedPrefabs
    {
        public EnemySpawnerConfig SpawnerConfig=EnemySpawnerConfig.GetDefault();
        public GameObject Enemy;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spawnerConfig = SpawnerConfig;
            spawnerConfig.Enemy = conversionSystem.GetPrimaryEntity(Enemy);
            dstManager.AddComponentData(entity, spawnerConfig);
            var randomSource = new IndividualRandomData();
            randomSource.Value = Random.CreateFromIndex((uint)entity.Index);
            dstManager.AddComponentData(entity, randomSource);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(Enemy);
        }
    }
    [Serializable]
    public struct EnemySpawnerConfig : IComponentData
    {
        public bool DoSpawn;
        public float SpawnCountDown;
        public float Interval;
        public float Space;
        public int SpawnCount;
        public float MinRange;
        public float MaxRange;
        public Entity Enemy;
        public float FrameRateThreshold;
        public int TotalSpawned;
        public int MaxSpawn;

        public static EnemySpawnerConfig GetDefault()
        {
            return new EnemySpawnerConfig()
            {
                DoSpawn = true,
                SpawnCountDown = 1,
                Interval = 1,
                SpawnCount = 5,
                MinRange = 150,
                MaxRange = 500,
                Enemy = default,
                MaxSpawn = 15000
            };
        }
    }
}