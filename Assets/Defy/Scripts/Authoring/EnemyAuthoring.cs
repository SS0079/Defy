using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Defy.Component
{
    public class EnemyAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Enemy Parameter")]
        public float MaxHp=10;
        [Header("Movement")] 
        public SimpleCharacterMoveConfig CharacterMoveConfig;
        public bool IsSoftCollide;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, CharacterMoveConfig);
            if (IsSoftCollide)
            {
                dstManager.AddComponentData(entity, new MovementData());
            }
            else
            {
                dstManager.AddComponentData(entity, new SimpleCharacterMovementData());
            }
            dstManager.AddComponentData(entity, new EnemyConfig(){MaxHp = MaxHp});
            dstManager.AddComponentData(entity, new CombatData() { HP = MaxHp });
            dstManager.AddComponentData(entity, new DestroyFlagData() { Yes = false });
        }
    }
}