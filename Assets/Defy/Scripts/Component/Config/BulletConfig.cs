using System;
using Unity.Entities;
using Unity.Physics;
using UnityEngine.Serialization;

namespace Defy.Component
{
    // [GenerateAuthoringComponent]
    public partial struct BulletConfig : IComponentData
    {
        public BulletConfigParams Params;
    }

    [Serializable]
    public struct BulletConfigParams
    {
        public HitType Type;
        public bool IgnoreGround;
        public float FuzeLength;
        public float Speed;
        public float LifeSpan;
        public float Damage;
        public float DamageRadius;
        public float KnockBack;
        public int Penetrate;
    }

    public enum HitType
    {
        Normal,
        Nova,
        Explode
    }
}