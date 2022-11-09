using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    public partial class AmmoUIUpdateSystem : SystemBase
    {
        private Camera _MainCamera;
        private float _ScreenWidth;
        private float _ScreenHeight;
        protected override void OnStartRunning()
        {
            _MainCamera = Camera.main;
            _ScreenWidth = Screen.width;
            _ScreenHeight = Screen.height;
        }

        protected override void OnUpdate()
        {
            Entities
                .ForEach((
                    Entity e,
                    int entityInQueryIndex,
                    AmmoUIReference ammoUIReference,
                    in WeaponConfig weaponConfig,
                    in AmmoCountData ammoCountData
                ) =>
                {
                    ammoUIReference.Value.SetAmmo(ammoCountData.Value,weaponConfig.MaxAmmo);
                    ammoUIReference.Value.SetWeaponName(weaponConfig.Name.ToString()+weaponConfig.Append.ToString());
                }).WithoutBurst().Run();
        }
    }
}