using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Defy.Utilities
{
    public static class SimplePhysicsUtility
    {
            public static void SortByFraction<T>(ref NativeList<T> castResults) where T : unmanaged, IQueryResult
        {
            if (castResults.Length<2)
            {
                return;
            }
            T cache;
            for (int i = 0; i < castResults.Length-1-i; i++)
            {
                if (castResults[i].Fraction>castResults[i+1].Fraction)
                {
                    cache = castResults[i];
                    castResults[i] = castResults[i + 1];
                    castResults[i + 1] = cache;
                }
            }
        }

        public static unsafe void TrimByFilter<T>(
             ref NativeList<T> castResults, 
             ComponentDataFromEntity<PhysicsCollider> colliderData, 
             CollisionFilter filter
             )where T : unmanaged, IQueryResult
        {
            if (castResults.Length==0)
            {
                return;
            }
            for (int i = castResults.Length-1; i >=0; i--)
            {
                if (colliderData.HasComponent(castResults[i].Entity))
                {
                    PhysicsCollider collider = colliderData[castResults[i].Entity];
                    if (!CollisionFilter.IsCollisionEnabled(filter,collider.ColliderPtr->Filter))
                    {
                        castResults.RemoveAt(i);
                    }
                }
            }
        }
         
         /// <summary>
         /// The specified entity is removed from the provided list if it is present.
         /// </summary>
         /// <param name="castResults"></param>
         /// <param name="ignore"></param>
         public static void TrimByEntity<T>(ref NativeList<T> castResults, Entity ignore) where T : unmanaged, IQueryResult
         {
             if (ignore == Entity.Null || castResults.Length==0)
             {
                 return;
             }

             for (int i = (castResults.Length - 1); i >= 0; --i)
             {
                 if (ignore == castResults[i].Entity)
                 {
                     castResults.RemoveAt(i);
                 }
             }
         }

         /// <summary>
         /// merge all hit points that too close to each other in Calculate distance 
         /// </summary>
         /// <param name="castResults"></param>
         /// <param name="tolerance"></param>
         private static void MergeHitPoint(ref NativeList<DistanceHit> castResults, float tolerance)
         {
             if (castResults.Length<2 || tolerance==0)
             {
                 return;
             }
             var toleranceSq = tolerance*tolerance;
             for (int i = 0; i < castResults.Length; i++)
             {
                 for (int j = castResults.Length-1; j >i; j--)
                 {
                     var offset = castResults[j].Position-castResults[i].Position;
                     var distanceSq = math.lengthsq(offset);
                     if (distanceSq<=toleranceSq)
                     {
                         castResults.RemoveAt(j);
                     }
                 }
             }
         }

        private static T PickNearestHit<T>(NativeList<T> hitList)where T : unmanaged, IQueryResult
        {
            T result=hitList[0];
            for (int i = 0; i < hitList.Length; i++)
            {
                if(i==0) continue;
                if (hitList[i].Fraction<result.Fraction)
                {
                    result = hitList[i];
                }
            }
            return result;
        }
    }

    public static class RandomUtility
    {
        public static float3 RandomPositionOnDisk(ref Random randomSource, float3 centerPos, float radius)
        {
            var randDir2D = randomSource.NextFloat2Direction();
            var randDir = new float3(randDir2D.x, 0, randDir2D.y);
            var randRadius = (1 - math.pow(randomSource.NextFloat(), 2)) * radius;
            var result = centerPos + randDir * randRadius;
            return result;
        }
    }
        
}