using System;
using Unity.Entities;
using UnityEngine;

namespace Defy.Component
{
    [Serializable]
    // [GenerateAuthoringComponent]
    public partial struct DestroyTimerData : IComponentData
    {
        public float Current;
        private float Max;

        public float RamainFraction() => 1-Current / Max;

        public DestroyTimerData Init(float span)
        {
            Current = span;
            Max = span;
            return this;
        }
    }
}