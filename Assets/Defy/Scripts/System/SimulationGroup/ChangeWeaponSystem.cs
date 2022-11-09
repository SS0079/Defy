using System;
using System.Linq;
using System.Text;
using Defy.Component;
using Defy.MonoBehavior;
using KittyHelpYouOut;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random=Unity.Mathematics.Random;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    public partial class ChangeWeaponSystem : SystemBase
    {
        public enum ChangeWeaponState
        {
            No,
            Prepare,
            Changing
        }
        
        private int _WeaponIndex = 0;
        private int[] _WeaponIDs;
        private BlobAssetReference<WeaponUpgrade> _AllUpgradeUpgrade;
        public ChangeWeaponState State=ChangeWeaponState.No;
        private int[] _WeaponChoice = new int[3];
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<WeaponConfigBuffer>();
        }

        protected override void OnStartRunning()
        {
            var allWeaponDataEntity = GetSingletonEntity<WeaponConfigBuffer>();
            var allWeaponData = GetBuffer<WeaponConfigBuffer>(allWeaponDataEntity);
            _WeaponIDs = new int[allWeaponData.Length];
            for (int i = 0; i < allWeaponData.Length; i++)
            {
                _WeaponIDs[i] = i;
            }
            _AllUpgradeUpgrade = GetSingleton<WeaponUpgradeReference>().Value;
        }

        protected override void OnUpdate()
        {
            var allWeaponDataEntity = GetSingletonEntity<WeaponConfigBuffer>();
            var allWeaponData = GetBuffer<WeaponConfigBuffer>(allWeaponDataEntity);
            

            switch (State)
            {
                case ChangeWeaponState.No:
                    //check reloading and prepare weapon card pull
                    Entities
                        .ForEach((
                            Entity e,
                            int entityInQueryIndex,
                            ref AmmoCountData ammoCountData,
                            in PlayerTag playerTag
                        ) =>
                        {
                            if (ammoCountData.Value<=0)
                            {
                                State=ChangeWeaponState.Prepare;
                                // pull 3 weapon index randomly
                                int pulledCount = 0;
                                KittyMath.KDShuffle(ref _WeaponIDs);
                                _WeaponChoice= _WeaponIDs.Take(3).ToArray();
                        
                       
                            }
                        }).WithoutBurst().Run();
                    break;
                case ChangeWeaponState.Prepare:
                    //send chosen weapon info to upgrade ui
                    Entities
                        .ForEach((
                            Entity e,
                            int entityInQueryIndex,
                            UpgradeUIReference upgradeUIReference
                        ) =>
                        {
                            State = ChangeWeaponState.Changing;
                            WeaponCardInfo[] infos=new WeaponCardInfo[3];
                            for (int i = 0; i < _WeaponChoice.Length; i++)
                            {
                                var localWeaponConfig = allWeaponData[_WeaponChoice[i]].Value;
                                infos[i] = new WeaponCardInfo
                                {
                                    Id = localWeaponConfig.WeaponID,
                                    Level = localWeaponConfig.CurrentLevel,
                                    FullName = localWeaponConfig.GetFullName(),
                                    UpgradeDesc = _AllUpgradeUpgrade.Value.GetUpgrade(localWeaponConfig.WeaponID, localWeaponConfig.CurrentLevel).Description.ToString()
                                };
                            }
                            upgradeUIReference.Value.ShowUpgrade(infos);
                        
                        }).WithoutBurst().Run();
                    break;
                case ChangeWeaponState.Changing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Dependency.Complete();
        }

        public void ChangeWeaponAndUpgrade(WeaponCardInfo info)
        {
            var allWeaponDataEntity = GetSingletonEntity<WeaponConfigBuffer>();
            var allWeaponData = GetBuffer<WeaponConfigBuffer>(allWeaponDataEntity);
            Entities
                .ForEach((
                    Entity e,
                    int entityInQueryIndex,
                    ref WeaponConfig weaponConfig,
                    ref IndividualRandomData randomData,
                    ref ResetableTimerData resetableTimerData,
                    ref AmmoCountData ammoCountData,
                    in PlayerTag playerTag
                ) =>
                {
                    WeaponUpgradeConfig chosenWeaponUpgrade = _AllUpgradeUpgrade.Value.GetUpgrade(info.Id, info.Level);
                    if (chosenWeaponUpgrade.HasValue())
                    {
                        // if there is a upgrade for the chosen weapon, add upgrade to weapon config before change weapon
                        allWeaponData[info.Id] = new WeaponConfigBuffer() { Value = chosenWeaponUpgrade.Upgrade(allWeaponData[info.Id].Value) };
                    }
                    else
                    {
                        var thisWeapon = allWeaponData[info.Id];
                        thisWeapon.Value.CurrentLevel++;
                        allWeaponData[info.Id] = thisWeapon;
                    }
                    weaponConfig = allWeaponData[info.Id].Value;
                    ammoCountData.Value = weaponConfig.MaxAmmo;
                    resetableTimerData.Value.Init(weaponConfig.DelayBetweenShoot);
                    resetableTimerData.Value.SetTo(0.1f);
                }).WithoutBurst().Run();
        }
    }
}