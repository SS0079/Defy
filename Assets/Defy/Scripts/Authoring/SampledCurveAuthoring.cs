using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Defy.Component
{
    public class SampledCurveAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public AnimationCurve SpeedCurve;
        public int SpeedCurveSampleCount=32;
        public AnimationCurve FadeCurve;
        public int FadeCurveSampleCount=32;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (SpeedCurve.length > 1 || FadeCurve.length > 1)
            {
                var curveReference = new SampledCurveReference();

                //generate Sampled curve reference and add to entity
                //check if curves have value by length of keys
                if (SpeedCurve.length > 1)
                {
                    using var blobBuilder = new BlobBuilder(Allocator.Temp);
                    ref var sampledCurve = ref blobBuilder.ConstructRoot<SampledCurve>();
                    var sampledCurveArray = blobBuilder.Allocate(ref sampledCurve.SampledPoints, SpeedCurveSampleCount);
                    sampledCurve.NumberOfSamples = SpeedCurveSampleCount;

                    for (var i = 0; i < SpeedCurveSampleCount; i++)
                    {
                        var samplePoint = (float)i / (SpeedCurveSampleCount - 1);
                        var sampleValue = SpeedCurve.Evaluate(samplePoint);
                        sampledCurveArray[i] = sampleValue;
                    }

                    var blobAssetReference = blobBuilder.CreateBlobAssetReference<SampledCurve>(Allocator.Persistent);
                    curveReference.Value0 = blobAssetReference;
                }
                if (FadeCurve.length > 1)
                {
                    using var blobBuilder = new BlobBuilder(Allocator.Temp);
                    ref var sampledCurve = ref blobBuilder.ConstructRoot<SampledCurve>();
                    var sampledCurveArray = blobBuilder.Allocate(ref sampledCurve.SampledPoints, FadeCurveSampleCount);
                    sampledCurve.NumberOfSamples = FadeCurveSampleCount;

                    for (var i = 0; i < FadeCurveSampleCount; i++)
                    {
                        var samplePoint = (float)i / (FadeCurveSampleCount - 1);
                        var sampleValue = FadeCurve.Evaluate(samplePoint);
                        sampledCurveArray[i] = sampleValue;
                    }

                    var blobAssetReference = blobBuilder.CreateBlobAssetReference<SampledCurve>(Allocator.Persistent);
                    curveReference.Value1 = blobAssetReference;
                }
                dstManager.AddComponentData(entity, curveReference);
            }
        }
    }
}