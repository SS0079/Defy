using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.Component
{
    [GenerateAuthoringComponent]
    public struct MIRVConfig : IComponentData
    {
        public int SplitCount;
        public float SplitRange;
        public int Salvo;
        public float Interval;
        public float Ceiling;
        public float ReenterSpeed;
        // public Entity ReenterVehicle;
    }
  

  
}