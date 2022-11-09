using System;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [Serializable]
    public struct EntityParticleData : IComponentData
    {
        public float Speed;
        public float LifeSpan;

        public bool HasValue() => Speed > 0 && LifeSpan > 0 ;

        public static EntityParticleData Zero() => new EntityParticleData()
        {
            Speed = 0,
            LifeSpan = 0,
        };
    }
}