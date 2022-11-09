using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Defy.Component
{
    public class BulletAuthoring : MonoBehaviour,IConvertGameObjectToEntity,IDeclareReferencedPrefabs
    {
        public BulletConfigParams BulletParams;
        public GameObject SubEntity1;
        public GameObject SubEntity2;
        public GameObject SubEntity3;
        public GameObject SubEntity4;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BulletConfig() { Params = BulletParams });
            var subEntityConfig = new SubEntityConfig();
            if (SubEntity1 != null)
            {
                subEntityConfig.Value1 = conversionSystem.GetPrimaryEntity(SubEntity1);
                dstManager.SetName(subEntityConfig.Value1,SubEntity1.name);
            }
            if (SubEntity2 != null)
            {
                subEntityConfig.Value2 = conversionSystem.GetPrimaryEntity(SubEntity2);
                dstManager.SetName(subEntityConfig.Value2,SubEntity2.name);
            }
            if (SubEntity3 != null)
            {
                subEntityConfig.Value3 = conversionSystem.GetPrimaryEntity(SubEntity3);
                dstManager.SetName(subEntityConfig.Value3,SubEntity3.name);
            }
            if (SubEntity4 != null)
            {
                subEntityConfig.Value4 = conversionSystem.GetPrimaryEntity(SubEntity4);
                dstManager.SetName(subEntityConfig.Value4,SubEntity4.name);
            }
            dstManager.AddComponentData(entity, subEntityConfig);
            dstManager.AddComponentData(entity, new MovementData()
            {
                IsLocal = true,
                LinearVelocity = new float3(0, 0, BulletParams.Speed),
                ForceVelocity = float3.zero
            });
            dstManager.AddComponentData(entity, new DestroyTimerData().Init(BulletParams.LifeSpan));
            dstManager.AddComponent<DestroyFlagData>(entity);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            if (SubEntity1!=null)
            {
                referencedPrefabs.Add(SubEntity1);
            }
            if (SubEntity2!=null)
            {
                referencedPrefabs.Add(SubEntity2);
            }
            if (SubEntity3!=null)
            {
                referencedPrefabs.Add(SubEntity3);
            }
            if (SubEntity4!=null)
            {
                referencedPrefabs.Add(SubEntity4);
            }
        }
    }
}