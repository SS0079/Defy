using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Defy.Component
{
    public struct SampledCurveReference : IComponentData
    {
        public BlobAssetReference<SampledCurve> Value0;
        public BlobAssetReference<SampledCurve> Value1;

        public readonly float GetValueAtTime(int valueIndex, float time)
        {
            switch (valueIndex)
            {
                case 0:
                    return Value0.Value.GetValueAtTime(time);
                    break;
                case 1:
                    return Value1.Value.GetValueAtTime(time);
                    break;
                default:
                    Debug.LogError("Switch miss");
                    return default;
                    break;
            }
        }
    }
    
    public struct SampledCurve
    {
        public BlobArray<float> SampledPoints;
        public int NumberOfSamples;
        
        public float GetValueAtTime(float time)
        {
            var approxSampleIndex = (NumberOfSamples - 1) * time;
            var sampleIndexBelow = (int)math.floor(approxSampleIndex);
            if (sampleIndexBelow >= NumberOfSamples - 1)
            {
                return SampledPoints[NumberOfSamples - 1];
            }
            var indexRemainder = approxSampleIndex - sampleIndexBelow;
            return math.lerp(SampledPoints[sampleIndexBelow], SampledPoints[sampleIndexBelow + 1], indexRemainder);
        }
    }
}