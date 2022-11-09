// using Defy.Component;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Systems;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Defy.System
// {
//     [UpdateInGroup(typeof(DefyUpdateGroup))]
//     public partial class SimpleCharacterMovementSystem : SystemBase
//     {
//         private BuildPhysicsWorld _PhysicsWorld;
//         protected override void OnCreate()
//         {
//             _PhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//             RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<SimpleCharacterMovementData>()));
//         }
//
//         protected override void OnUpdate()
//         {
//             var collisionWorld = _PhysicsWorld.PhysicsWorld.CollisionWorld;
//             var colliderData = GetComponentDataFromEntity<PhysicsCollider>(true);
//             Dependency=new MoveWithCollide() { Δt = Time.DeltaTime,CWorld = collisionWorld, ColliderDataFromEntities = colliderData}.ScheduleParallel(Dependency);
//         }
//         
//
//
//         /// <summary>
//         /// One-Stop Character controller logic
//         /// </summary>
//         [BurstCompile]
//         private partial struct MoveWithCollide : IJobEntity
//         {
//             public float Δt;
//             // public float CornerAngle;
//             [ReadOnly]public CollisionWorld CWorld;
//             [ReadOnly]public ComponentDataFromEntity<PhysicsCollider> ColliderDataFromEntities;
//
//             public void Execute(ref SimpleCharacterMovementData characterMovementData,ref Translation translation,in SimpleCharacterMoveConfig characterMoveConfig, in Rotation 
//                 rotation,in PhysicsCollider collider,in Entity entity)
//             {
//                 characterMovementData.IsCorner = false;
//                 characterMovementData.IsGround = false;
//                 characterMovementData.IsRamp = false;
//                 var filter = new CollisionFilter()
//                 {
//                     BelongsTo = (uint)CollisionLayers.Caster,
//                     CollidesWith = (uint)CollisionLayers.Actor | (uint)CollisionLayers.Enemy | (uint)CollisionLayers.Ground | (uint)CollisionLayers.Wall
//                 };
//                 float3 totalMovement = default;
//                 float3 gravityDown = default;
//                 float3 gravityUp = default;
//                 float socialDistance = characterMoveConfig.SocialDistance; 
//                 float slopeLimitCos = math.cos(math.radians(characterMoveConfig.SlopLimit));
//
//                 #region Check ground
//                 gravityDown = math.normalizesafe(characterMoveConfig.Gravity);
//                 gravityUp = -gravityDown;
//                 NativeList<DistanceHit> groundHitList = new NativeList<DistanceHit>(Allocator.Temp);
//                 // NativeList<ColliderCastHit> groundHitList = new NativeList<ColliderCastHit>(Allocator.Temp);
//                 unsafe
//                 {
//                     float probeLength=0;
//                     
//                     //calculate ground check probe length
//                     {
//                         var downVelocity = math.projectsafe(characterMovementData.KinematicVelocity + characterMovementData.ControlledVelocity, gravityUp);
//                         probeLength = math.length(downVelocity)*Δt+socialDistance;
//                         probeLength = math.max(probeLength, socialDistance);
//                     }
//                     var input = new ColliderDistanceInput()
//                     {
//                         Collider = collider.ColliderPtr,
//                         MaxDistance = socialDistance,
//                         Transform = new RigidTransform(rotation.Value,translation.Value+gravityDown*probeLength)
//                     };
//                     // var input = new ColliderCastInput(collider.Value, translation.Value, translation.Value + gravityDown * probeLength);
//                     // Debug.DrawRay(translation.Value, gravityDown * probeLength * 2,Color.red);
//                     //calculate collider distance to check ground
//                     CWorld.CalculateDistance(input, ref groundHitList);
//                     // CWorld.CastCollider(input, ref groundHitList);
//                     SimplePhysicsUtility.TrimByEntity(ref groundHitList, entity);
//                     SimplePhysicsUtility.TrimByFilter(ref groundHitList,ColliderDataFromEntities,filter);
//                     
//                     // Debug.Log($"{groundHitList.Length} | {probeLength}");
//                     
//                     if (groundHitList.Length>0)
//                     {
//                         // for (int i = 0; i < groundHitList.Length; i++)
//                         // {
//                         //     Debug.DrawRay(groundHitList[i].Position,groundHitList[i].SurfaceNormal,Color.yellow);
//                         // }
//                         SimplePhysicsUtility.SortByFraction(ref groundHitList);
//                         if (math.dot(groundHitList[0].SurfaceNormal, gravityUp) > slopeLimitCos || groundHitList.Length > 1)
//                         {
//                             characterMovementData.IsGround = true;
//                         }
//                         else
//                         {
//                             characterMovementData.IsRamp = true;
//                         }
//                     }
//                 }
//                 #endregion
//
//                 #region Kinematic adjust
//                 if (characterMovementData.IsGround && math.dot(characterMovementData.KinematicVelocity,gravityDown)>0)
//                 {
//                     characterMovementData.KinematicVelocity -= math.projectsafe(characterMovementData.KinematicVelocity, gravityUp);
//                 }
//                 #endregion
//                 
//                 #region Prepare total movement
//                 //calculate and split kinematic velocity
//                 
//                 //if character is not grounded, count gravity in
//                 characterMovementData.RawAcceleration +=characterMovementData.IsGround? float3.zero : characterMoveConfig.Gravity;
//                 
//                 characterMovementData.KinematicVelocity += characterMovementData.RawAcceleration*Δt;
//                 
//                 //clean raw acceleration after split,it shouldn't carried between frames
//                 characterMovementData.RawAcceleration=float3.zero;
//
//                 totalMovement = (characterMovementData.KinematicVelocity + characterMovementData.ControlledVelocity) * Δt;
//                 #endregion
//
//                 #region Handle collision check
//                 unsafe
//                 {
//                     // var offset = math.normalizesafe(totalMovement) * (math.length(totalMovement) + socialDistance);
//                     var offset = totalMovement;
//                     NativeList<DistanceHit> moveHitList = new NativeList<DistanceHit>(Allocator.Temp);
//                     {
//                         ColliderDistanceInput input = new ColliderDistanceInput()
//                         {
//                             Collider = collider.ColliderPtr,
//                             MaxDistance = socialDistance,
//                             Transform = new RigidTransform(rotation.Value, translation.Value + offset)
//                         };
//                         CWorld.CalculateDistance(input,ref moveHitList);
//                     }
//                     SimplePhysicsUtility.TrimByEntity(ref moveHitList,entity);
//                     SimplePhysicsUtility.TrimByFilter(ref moveHitList,ColliderDataFromEntities,filter);
//                     if (moveHitList.Length>0)
//                     {
//                         // for (int i = 0; i < moveHitList.Length; i++)
//                         // {
//                         //     Debug.DrawRay(moveHitList[i].Position,moveHitList[i].SurfaceNormal,Color.magenta);
//                         // }
//                         SimplePhysicsUtility.SortByFraction(ref moveHitList);
//                         var hit0 = moveHitList[0];
//                         bool steping = false;
//
//                         #region Obsolete
//                         //check if we can step on, only when we are on ground and is moving
//                         // float3 stepVerticalOffset=float3.zero;
//                         // if(characterMovementData.IsGround && math.lengthsq(characterMovementData.ControlledVelocity)>0)
//                         // {
//                         //     //还是要换成cast collider
//                         //     var checkEnd = translation.Value + totalMovement;
//                         //     // var checkPoint = translation.Value + totalMovement + gravityUp * (characterMoveConfig.StepLimit-socialDistance);
//                         //     var checkLength = characterMoveConfig.StepLimit;
//                         //     var checkStart = checkEnd + gravityUp * checkLength;
//                         //     // ColliderDistanceInput input = new ColliderDistanceInput()
//                         //     // {
//                         //     //     Collider = collider.ColliderPtr,
//                         //     //     MaxDistance = socialDistance,
//                         //     //     Transform = new RigidTransform(rotation.Value, checkPoint)
//                         //     // };
//                         //     ColliderCastInput input = new ColliderCastInput(collider.Value, checkStart, checkEnd);
//                         //     // NativeList<DistanceHit> stepHitList = new NativeList<DistanceHit>(Allocator.Temp);
//                         //     NativeList<ColliderCastHit> stepHitList = new NativeList<ColliderCastHit>(Allocator.Temp);
//                         //     // CWorld.CalculateDistance(input, ref stepHitList);
//                         //     CWorld.CastCollider(input, ref stepHitList);
//                         //     TrimByEntity(ref stepHitList,entity);
//                         //     TrimByFilter(ref stepHitList,ColliderDataFromEntities,filter);
//                         //     SortByFraction(ref stepHitList);
//                         //     if (stepHitList.Length>0 && stepHitList[0].Fraction<=0)
//                         //     {
//                         //         //we hit something ,that means obstacle is too high to step on
//                         //         //than we need to project movement hit normal to horizontal plane
//                         //         //just open a flag to remind fallowing logic to do the project before calculate
//                         //         steping = true;
//                         //         stepVerticalOffset = math.projectsafe(stepHitList[0].Position - translation.Value, gravityUp);
//                         //         translation.Value += stepVerticalOffset;
//                         //     }
//                         // }
//                         // float3 reflectDir=totalMovement + hit0.SurfaceNormal * (socialDistance - hit0.Distance);
//                         // if (canStep)
//                         // {
//                         //     reflectDir += totalMovement + stepVerticalOffset;
//                         // }
//                         // else
//                         // {
//                         //     reflectDir -= math.projectsafe(totalMovement + hit0.SurfaceNormal * (socialDistance - hit0.Distance), gravityUp);
//                         // }
//                         #endregion
//                         float3 reflectDir=totalMovement + hit0.SurfaceNormal * (socialDistance - hit0.Distance);
//                         // Debug.DrawRay(translation.Value+totalMovement*50, hit0.SurfaceNormal * (socialDistance-hit0.Distance)*50,Color.green);
//                         // Debug.DrawRay(translation.Value,totalMovement*50,Color.white);
//                         //handle corner movement
//                         if (moveHitList.Length>1 /*&& !canStep*/)
//                         {
//                             
//                             //check reflect direct, see if there any obstacle
//                             ColliderCastInput cornerCheckInput = new ColliderCastInput(collider.Value, translation.Value, translation.Value + reflectDir*3, rotation.Value);
//                             ColliderCastHit cornerHit=default;
//                             if (CWorld.CastCollider(cornerCheckInput,out cornerHit))
//                             {
//                                 var moveDir = math.normalizesafe(totalMovement);
//                                 var moveRight = math.cross(gravityUp,moveDir);
//                                 var hitaMovementCos = math.dot(moveHitList[0].SurfaceNormal, moveRight);
//                                 var hitbMovmentCos = math.dot(moveHitList[1].SurfaceNormal, moveRight);
//                                 if (hitaMovementCos*hitbMovmentCos<=0)
//                                 {
//                                     characterMovementData.IsCorner = true;
//                                 }
//                             }
//                             // reflectProbe = math.lengthsq(reflectProbe) > (characterMoveConfig.MinMovement * characterMoveConfig.MinMovement) ? reflectProbe : float3.zero;
//                         }
//                         if (characterMovementData.IsCorner)
//                         {
//                             totalMovement=float3.zero;
//                             //cut off horizontal kinematic velocity
//                             characterMovementData.KinematicVelocity = math.projectsafe(characterMovementData.KinematicVelocity, gravityUp);
//                         }
//                         else
//                         {
//                             totalMovement = reflectDir;
//                             //project kinematic velocity on wall direction
//                             // characterMovementData.KinematicVelocity = math.projectsafe(characterMovementData.KinematicVelocity, reflectDir);
//                             //reflect kinematic velocity against wall
//                             characterMovementData.KinematicVelocity = math.reflect(characterMovementData.KinematicVelocity, hit0.SurfaceNormal);
//                         }
//                     }
//                     translation.Value += totalMovement;
//                 }
//                 // Debug.DrawRay(translation.Value,totalMovement*50,Color.red);
//                 #endregion
//                 
//                 #region Handle drag
//                 var kinematicVelocitySqrLength = math.lengthsq(characterMovementData.KinematicVelocity);
//                 if (kinematicVelocitySqrLength>0.00001)
//                 {
//                     characterMovementData.KinematicVelocity -= math.normalizesafe(characterMovementData.KinematicVelocity) * characterMoveConfig.LinearDrag * Δt;
//                     float velocityDot = 0;
//                     if (math.lengthsq(characterMovementData.ControlledVelocity)>0)
//                     {
//                         velocityDot = math.dot(characterMovementData.ControlledVelocity, characterMovementData.KinematicVelocity);
//                     }
//                     if (velocityDot<0)
//                     {
//                         //if control velocity is on the counter direction of kinematic velocity, use control velocity to neutralize kinematic velocity
//                         var neutralize = (velocityDot / math.dot(characterMovementData.KinematicVelocity, characterMovementData.KinematicVelocity)) *
//                                          characterMovementData.KinematicVelocity;
//                         if (math.lengthsq(neutralize)<kinematicVelocitySqrLength)
//                         {
//                             characterMovementData.KinematicVelocity += neutralize*Δt;
//                         }
//                         else
//                         {
//                             characterMovementData.KinematicVelocity=float3.zero;
//                         }
//                     }
//                 }
//                 else
//                 {
//                     characterMovementData.KinematicVelocity=float3.zero;
//                 }
//
//                 // var totalMoveDir = math.normalizesafe(characterMovementData.TotalMovement);
//                 // var totalMoveLength = math.length(characterMovementData.TotalMovement);
//                 // characterMovementData.TotalMovement = totalMoveDir*math.max(totalMoveLength-characterMoveConfig.LinearDrag*Δt,0) ;
//                 #endregion
//
//             }
//         }
//
//         //================================================================================
//
//     
//         
//
//         // /// <summary>
//         // /// return if character finally stop in a corner
//         // /// </summary>
//         // /// <param name="translation"></param>
//         // /// <param name="start">first start point that cast alone the wall</param>
//         // /// <param name="movement"></param>
//         // /// <param name="collider"></param>
//         // /// <param name="filter"></param>
//         // /// <param name="collisionWorld"></param>
//         // /// <param name="fromEntity"></param>
//         // /// <param name="socialDistance"></param>
//         // /// <param name="maxIterate"></param>
//         // /// <returns></returns>
//         // private static bool IterateCollisionCheck(
//         //     ref Translation translation,
//         //     ColliderCastInput input,
//         //     float distance,
//         //     float3 masterDir,
//         //     CollisionFilter filter,
//         //     CollisionWorld collisionWorld, 
//         //     ComponentDataFromEntity<PhysicsCollider> fromEntity,
//         //     float socialDistance,
//         //     int maxIterate=3
//         //     )
//         // {
//         //     //遍历碰撞检测来处理角落判断
//         //     //输入初始位置和初始目的地，并进行cast collider,如果找到碰撞点，就沿碰撞面向初始运动方向再次检测，以此类推。每次检测要扣除相应的位移距离
//         //     //直到出现以下几种情况：
//         //     //1，碰撞检测落空，说明本次遍历中已经走出拐角，这说明角色可能在普通的平直墙体上滑动，或者从钝角拐角处转出，此时可以应用目前遍历结果的位置
//         //     //2,在后续检测中，发现碰撞点距离小于社交距离，说明当前角色进入锐角死角，此时可以应用目前遍历结果的位置
//         //     //检测流程预测如下：
//         //     //第一次检测，从原始初始位置到原始位移目标。考虑在遍历之前肯定已经有一次cast hit，可以在外部算出一次碰撞后的滑动方向，以这个滑动方向作为遍历时的原始检测方向。
//         //     //如果第一次检测就落空说明肯定是平直墙体，这是就可以结束遍历并应用结果了。
//         //     //如果第一次检测有结果，说明可能是锐角或钝角，根据碰撞法线计算滑动方向并进行第二次检测。
//         //     //第二次检测，如果落空说明此处为钝角且角色已经可以转出这个墙角，此时可以将结束遍历并应用结果。
//         //     //如果没落空，即再次命中，说明是锐角，按社交距离对齐后结束遍历应用结果
//         //     //如果碰撞点在社交距离外，说明前方还有空间，继续遍历
//         //     //如果检测方向已经和masterDirection相差超90度，说明发生方向扭转，直接终止遍历并应用当前结果。
//         //     
//         //     //当前问题：
//         //     //在拐角处没能保持社交距离
//         //     //倾斜检测社交距离留空不足
//         //     //遍历可能很难处理角落问题，考虑calculate distance
//         //     
//         //     
//         //     bool justCrashWall = false;
//         //     bool twisted = false;
//         //     float remainLength =distance;
//         //     NativeList<ColliderCastHit> hitList = new NativeList<ColliderCastHit>(Allocator.Temp);
//         //     //prepare first cast
//         //     float3 localPosition=translation.Value;
//         //     ColliderCastInput localInput = input;
//         //     int safetyCount = maxIterate;
//         //     bool isCollide=false;
//         //     //if hit noting, break to result; if remain probe length is shorter than social distance, break result;
//         //     do
//         //     {
//         //         hitList.Clear();
//         //         //Cast!!
//         //         collisionWorld.CastCollider(localInput, ref hitList);
//         //         TrimByFilter(ref hitList,fromEntity,filter);
//         //         //cache previous cast direction
//         //         float3 castDir = math.normalizesafe(localInput.End-localInput.Start);
//         //         if (!justCrashWall)
//         //         {
//         //             // Debug.DrawRay(localPosition,castDir,Color.green);
//         //         }
//         //         else
//         //         {
//         //             // Debug.DrawRay(localPosition,castDir,Color.red);
//         //         }
//         //         if (hitList.Length>0)
//         //         {
//         //             //if cast hit , prepare next cast
//         //             var hit0 = PickNearestHit(hitList);
//         //             // Debug.Log($"{hit0.Fraction} | {remainLength} | {socialDistance}");
//         //             //check hit0 is actually hit anything, if so ,prepare next cast input
//         //             if (hit0.Fraction>0)
//         //             {
//         //                 //snap trans position
//         //                 // var socialOffset = socialDistance / math.dot(hit0.SurfaceNormal, -previousCastDir);
//         //                 var socialOffset = socialDistance ;
//         //                 var hitDistance = math.length(localInput.End - localInput.Start) * hit0.Fraction;
//         //                 
//         //                 //Save position
//         //                 localPosition += castDir* (hitDistance-socialOffset);
//         //                 // Debug.Log($"{remainLength} | {hitDistance.ToString("f10")}");
//         //                 
//         //                 //check if we crash on a wall,thus hit distance is shorter than social distance
//         //                 if (hitDistance<socialOffset)
//         //                 {
//         //                     // Debug.Log("Just hit wall");
//         //                     if (justCrashWall)
//         //                     {
//         //                         //if we just crash wall last iterate
//         //                         // Debug.Log("acute corner");
//         //                         break;
//         //                     }
//         //                     justCrashWall = true;
//         //                 }
//         //                 else
//         //                 {
//         //                     justCrashWall = false;
//         //                 }
//         //                 
//         //                 //prepare next cast input
//         //                 localInput.Start = localPosition;
//         //                 var nextMove = castDir*(math.max(remainLength,socialDistance*2));
//         //                 localInput.End = localInput.Start + (nextMove - math.projectsafe(nextMove,hit0.SurfaceNormal));
//         //                 var nextDir = math.normalizesafe(localInput.End - localInput.Start);
//         //                 var nextLength = math.length(localInput.End - localInput.Start);
//         //
//         //                 if (math.dot(masterDir,localInput.End-localInput.Start)<0)
//         //                 {
//         //                     //if the next cast direction is on opposite from masterDir, break
//         //                     // Debug.Log("twisted");
//         //                     break;
//         //                 }
//         //                 
//         //                 //reduce remainLength
//         //                 remainLength -=math.max(0,hitDistance-socialOffset);
//         //             }
//         //         }
//         //         else
//         //         {
//         //             if (safetyCount==maxIterate)
//         //             {
//         //                 //if miss at first cast, that means we hit a straight wall
//         //                 // Debug.Log("hit straight wall");
//         //             }
//         //             else
//         //             {
//         //                 //if miss at fallowing cast means we hit a obtuse corner
//         //                 // Debug.Log("hit obtuse corner");
//         //             }
//         //             isCollide = true;
//         //             //Save position
//         //             localPosition += castDir * remainLength;
//         //             remainLength = 0;
//         //
//         //         }
//         //         safetyCount--;
//         //         
//         //         if (safetyCount==0)
//         //         {
//         //             //safeCount deplete
//         //             // Debug.Log("Reach limit");
//         //             isCollide = true;
//         //         }
//         //         
//         //     } while (safetyCount>0 && remainLength>0);
//         //     translation.Value = localPosition;
//         //
//         //     return isCollide;
//         // }
//     }
//     
//     
// }