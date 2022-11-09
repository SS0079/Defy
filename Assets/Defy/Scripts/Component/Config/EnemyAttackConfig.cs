using System.Diagnostics;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct EnemyAttackConfig : IComponentData
    {
        public float Damage;
        public float FuzeLength;
        public Entity ExplodeVFX;
    }
}