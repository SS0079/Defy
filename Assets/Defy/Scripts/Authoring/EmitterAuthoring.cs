using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Defy.Component
{
    public class EmitterAuthoring : MonoBehaviour,IConvertGameObjectToEntity,IDeclareReferencedPrefabs
    {
        public GameObject ParticleEntity;
        public Vector2 SpeedRange;
        public Vector2 LifeSpanRange;
        public AnimationCurve SpeedCurve;
        public int SpeedCurveSampleCount=32;
        public AnimationCurve FadeCurve;
        public int FadeCurveSampleCount=32;
        public EmitterType Type;
        public float3 InitDirection;
        public float Scale=10;
        public int Salvo=1;
        public int Loop=1;
        public float Interval=0.5f;
        public float CountDown;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var emitterConfig = new EntityParticleEmitterConfigData()
            {
                ParticleEntity = conversionSystem.GetPrimaryEntity(ParticleEntity),
                SpeedRange = SpeedRange,
                LifeSpanRange = LifeSpanRange,
                Type = Type,
                InitDirection = InitDirection,
                Scale = Scale,
                Salvo = Salvo,
                Loop = Loop,
                Interval = Interval,
                CountDown = CountDown
            };
            dstManager.AddComponentData(entity, emitterConfig);
            if (SpeedCurve.length>1 || FadeCurve.length>1)
            {
                var curveReference = new SampledCurveReference();
                //generate Sampled curve reference and add to entity
                //check if curves have value by length of keys
                if (SpeedCurve.length>1)
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
                    curveReference.Value0=blobAssetReference;
                }
                if (FadeCurve.length>1)
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
                    curveReference.Value1=blobAssetReference;
                }
                dstManager.AddComponentData(entity, curveReference);
            }
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ParticleEntity);
        }
    }
}