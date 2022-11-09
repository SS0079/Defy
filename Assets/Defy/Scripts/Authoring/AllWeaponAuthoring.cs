using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Defy.Component
{
    public class AllWeaponAuthoring : MonoBehaviour,IConvertGameObjectToEntity
    {
        public List<WeaponAuthoringObject> AllWeaponConfigs;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //generate a individual random and add to entity
            var individualRandom = new IndividualRandomData();
            individualRandom.Value.InitState((uint)DateTime.Now.Millisecond);
            dstManager.AddComponentData(entity, individualRandom);

            int activateWeaponCount = AllWeaponConfigs.FindAll(i=>i.Activate).Count;
            
            NativeArray<WeaponConfigBuffer> weaponBufferArray = new NativeArray<WeaponConfigBuffer>(activateWeaponCount,Allocator.Temp);
            
            //add up the count of all upgrades whether activated or not

            for (int i = 0; i < AllWeaponConfigs.Count; i++)
            {
                var authoringObject = AllWeaponConfigs[i];
                
                //write weapon config to buffer only if it is activated
                if (!authoringObject.Activate) continue;
                
                var weaponConfig = authoringObject.Weapon;
                weaponConfig.Name = authoringObject.Name;
                weaponConfig.WeaponID = i;
                weaponConfig.CurrentLevel = 0;

                //need load bullet params from weapon config to bullet config
                using BlobAssetStore bas = new BlobAssetStore();
                Entity bulletEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                    authoringObject.BulletPrefab,
                    GameObjectConversionSettings.FromWorld(dstManager.World, bas));
                var bulletConfig = dstManager.GetComponentData<BulletConfig>(bulletEntity);
                bulletConfig.Params = weaponConfig.BulletParams;
                dstManager.SetComponentData(bulletEntity, bulletConfig);
                dstManager.SetComponentData(bulletEntity,new DestroyTimerData().Init(bulletConfig.Params.LifeSpan));
                dstManager.SetComponentData(bulletEntity,new MovementData(){IsLocal = true,LinearVelocity = new float3(0,0,bulletConfig.Params.Speed)});
                dstManager.SetName(bulletEntity,authoringObject.BulletPrefab.name);
                weaponConfig.BulletEntity = bulletEntity;
                weaponBufferArray[i] = new WeaponConfigBuffer() { Value = weaponConfig };
            }
            var allWeaponBuffer = dstManager.AddBuffer<WeaponConfigBuffer>(entity);
            allWeaponBuffer.AddRange(weaponBufferArray);
            

        }

    }

   
    
}