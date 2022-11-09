using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    public struct ResetableTimerData : IComponentData
    {
        public Timer Value;
    }

    public struct Timer
    {
        public bool Ready;
        public float Interval;
        public float Current;

        public void Reset()
        {
            Ready = false;
            Current = Interval;
        }

        public void Init(float span)
        {
            Interval = span;
            Reset();
        }

        public void SetTo(float time)
        {
            Current = time;
        }
    }
}