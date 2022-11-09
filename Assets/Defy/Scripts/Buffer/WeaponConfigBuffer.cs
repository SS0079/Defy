using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    //Mainly use by ChangeWeaponSystem
    [InternalBufferCapacity(10)]
    public struct WeaponConfigBuffer : IBufferElementData
    {
        public WeaponConfig Value;
    }
    
}