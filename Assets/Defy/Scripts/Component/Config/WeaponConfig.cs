using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Defy.Component
{
    [Serializable]
    public partial struct WeaponConfig : IComponentData
    {
        [HideInInspector] public FixedString64Bytes Name;
        [HideInInspector] public FixedString32Bytes Append;
        [HideInInspector] public int WeaponID;
        [HideInInspector] public int CurrentLevel;
        public float DelayBetweenShoot;
        public int Salvo;
        public float SpreadDegree;
        public int MaxAmmo;
        public BulletConfigParams BulletParams;
        [FormerlySerializedAs("Weapon")] public WeaponType Type;
        public Entity BulletEntity;

        public string GetFullName() => Name.ToString() + Append.ToString();
    }
    public enum WeaponType
    {
        FullAuto,
        MIRV,
        BallisticMissile
    }
}