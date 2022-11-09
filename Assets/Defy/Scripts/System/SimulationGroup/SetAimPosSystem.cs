using System;
using Defy.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Defy.System
{
    [Flags]
    public enum CollisionLayers
    {
        Actor = 1 << 0,
        Ground = 1 << 1,
        Caster = 1 << 2,
        Enemy=1<<3,
        Wall=1<<4
    }


    
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    public partial class SetAimPosSystem : SystemBase
    {
        private Camera _MainCamera;
        private BuildPhysicsWorld _BuildPhysicsWorld;
        private CollisionWorld _CollisionWorld;
        private float3? _AimPos;
        protected override void OnStartRunning()
        {
            _MainCamera = Camera.main;
            _BuildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            _CollisionWorld = _BuildPhysicsWorld.PhysicsWorld.CollisionWorld;
            var setAimJobHandle = new SetAimPosJob() { CWorld = _CollisionWorld,Ray = _MainCamera.ScreenPointToRay(Input.mousePosition)}.ScheduleParallel();
            setAimJobHandle.Complete();
        }

        
        [BurstCompatible]
        public partial struct SetAimPosJob : IJobEntity
        {
            [ReadOnly] public CollisionWorld CWorld;
            public UnityEngine.Ray Ray;

            public void Execute(ref AimPosData aimPosData)
            {
                var raycastInput = new RaycastInput()
                {
                    Start = Ray.origin,
                    End = Ray.GetPoint(1000f),
                    Filter = new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayers.Caster,
                        CollidesWith = (uint)(CollisionLayers.Ground),
                    }
                };
                if (CWorld.CastRay(raycastInput,out RaycastHit hit))
                {
                    aimPosData.HasValue = true;
                    aimPosData.Value = hit.Position;
                }
                else
                {
                    aimPosData.HasValue = false;
                }
            }
        }
    }
}