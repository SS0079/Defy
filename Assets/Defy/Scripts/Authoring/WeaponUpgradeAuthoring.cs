using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Defy.Component
{
    public class WeaponUpgradeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public List<WeaponUpgradeObject> AllUpgrades;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //generate blobBuilder
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var weaponUpgrade = ref blobBuilder.ConstructRoot<WeaponUpgrade>();
            var upgradeArray = blobBuilder.Allocate(ref weaponUpgrade.Upgrades, AllUpgrades.Count);
            //write weapon upgrade to blob array
            for (int i = 0; i < AllUpgrades.Count; i++)
            {
                upgradeArray[i] = AllUpgrades[i].ToConfig();
            }
            var upgradeReference = new WeaponUpgradeReference();
            var weaponUpgradeBlobRefer = blobBuilder.CreateBlobAssetReference<WeaponUpgrade>(Allocator.Persistent);
            upgradeReference.Value = weaponUpgradeBlobRefer;
            dstManager.AddComponentData(entity, upgradeReference);
        }
        
    }
    
     [Serializable]
    public struct WeaponAuthoringObject
    {
        public bool Activate;
        public string Name;
        public WeaponConfig Weapon;
        public GameObject BulletPrefab;
    }
    public struct WeaponUpgradeConfig
    {
        public int WeaponID;
        public int Level;
        public FixedString32Bytes Append;
        public FixedString64Bytes Description;
        public float DelayBetweenShotMul;
        public int SalvoAdd;
        public float SpreadMul;
        public int MaxAmmoAdd;
        public float BulletSpeedAdd;
        public float BulletSpanAdd;
        public float BulletDamageAdd;
        public float BulletDamageRadiusMul;
        public int BulletPenetrateAdd;

        public bool HasValue()
        {
            return !Append.IsEmpty && !Description.IsEmpty;
        }

        public WeaponConfig Upgrade(WeaponConfig config)
        {
            BulletConfigParams upgradedBullet = new BulletConfigParams
            {
                Type = config.BulletParams.Type,
                IgnoreGround = config.BulletParams.IgnoreGround,
                FuzeLength = config.BulletParams.FuzeLength,
                Speed = config.BulletParams.Speed + BulletSpeedAdd,
                LifeSpan = config.BulletParams.LifeSpan + BulletSpanAdd,
                Damage = config.BulletParams.Damage + BulletDamageAdd,
                DamageRadius = config.BulletParams.DamageRadius + BulletDamageRadiusMul,
                KnockBack = config.BulletParams.KnockBack,
                Penetrate = config.BulletParams.Penetrate + BulletPenetrateAdd
            };
            WeaponConfig result = new WeaponConfig
            {
                Name = config.Name,
                Append = Append,
                WeaponID = config.WeaponID,
                CurrentLevel = config.CurrentLevel+1,
                DelayBetweenShoot = config.DelayBetweenShoot*DelayBetweenShotMul,
                Salvo = config.Salvo+SalvoAdd,
                SpreadDegree = config.SpreadDegree*SpreadMul,
                MaxAmmo = config.MaxAmmo+MaxAmmoAdd,
                BulletParams = upgradedBullet,
                Type = config.Type,
                BulletEntity = config.BulletEntity
            };
            return result;
        }

        public string GetDesString() => Description.ToString();
        public string GetAppendString() => Append.ToString();
    }
    [Serializable]
    public struct WeaponUpgradeObject
    {
        public int WeaponID;
        public int Level;
        public string Append;
        public string Description;
        public float DelayBetweenShotMul;
        public int SalvoAdd;
        public float SpreadMul;
        public int MaxAmmoAdd;
        public float BulletSpeedAdd;
        public float BulletSpanAdd;
        public float BulletDamageAdd;
        public float BulletDamageRangeMul;
        public int BulletPenetrateAdd;

        public WeaponUpgradeConfig ToConfig()
        {
            var result = new WeaponUpgradeConfig
            {
                WeaponID = WeaponID,
                Level = Level,
                Append = Append,
                Description = Description,
                DelayBetweenShotMul = DelayBetweenShotMul,
                SalvoAdd = SalvoAdd,
                SpreadMul = SpreadMul,
                MaxAmmoAdd = MaxAmmoAdd,
                BulletSpeedAdd = BulletSpeedAdd,
                BulletSpanAdd = BulletSpanAdd,
                BulletDamageAdd = BulletDamageAdd,
                BulletDamageRadiusMul = BulletDamageRangeMul,
                BulletPenetrateAdd = BulletPenetrateAdd
            };
            return result;
        }
    }
    
    
    
    public struct WeaponUpgrade
    {
        public BlobArray<WeaponUpgradeConfig> Upgrades;

        public WeaponUpgradeConfig GetUpgrade(int id, int level)
        {
            WeaponUpgradeConfig result = default;
            for (int i = 0; i < Upgrades.Length; i++)
            {
                var thisConfig = Upgrades[i];
                if (thisConfig.WeaponID==id && thisConfig.Level==level)
                {
                    result = thisConfig;
                }
            }
            return result;
        }
    }
}