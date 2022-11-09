using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Defy
{
    public class PlayerSpawner : MonoBehaviour
    {
        public int Row;
        public int Colum;
        public GameObject Prefab;
        private BlobAssetStore _BAS;
        void Start()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _BAS = new BlobAssetStore();
            var setting = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _BAS);
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, setting);
            for (int j = 0; j < Colum; j++)
            {
                for (int i = 0; i < Colum; i++)
                {
                    entityManager.SetComponentData(entity,new Translation(){ Value = new float3(i*3,20f,j*3f)});
                    entityManager.Instantiate(entity);
                }
            }
        }

        private void OnDestroy()
        {
            _BAS?.Dispose();
        }
    }
}
