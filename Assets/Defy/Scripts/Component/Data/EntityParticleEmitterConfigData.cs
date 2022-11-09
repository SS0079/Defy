using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct EntityParticleEmitterConfigData : IComponentData
    {
        public Entity ParticleEntity;
        // public EntityParticleData ParticleData;
        public float2 SpeedRange;
        public float2 LifeSpanRange;
        public EmitterType Type;
        public float3 InitDirection;
        public float Scale;
        public int Salvo;
        public int Loop;
        public float Interval;
        public float CountDown;
    }

    public enum EmitterType
    {
        Sphere,
        NorthernHemisphere,
        Disk
    }

}