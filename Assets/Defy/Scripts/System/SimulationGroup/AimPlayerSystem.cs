using Defy.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    public partial class AimPlayerSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            new AimPlayerJob { Δt = Time.DeltaTime }.ScheduleParallel();
        }
    }
    public partial struct AimPlayerJob : IJobEntity
    {
        public float Δt;
        public void Execute(ref Rotation rotation ,ref AimPosData aimPosData ,in Translation translation ,in RotateSpeedConfig rotSpeedConfig)
        {
            quaternion result=rotation.Value;
            if (aimPosData.HasValue)
            {
                if (rotSpeedConfig.InstantRotate)
                {
                    float3 aimDir = aimPosData.Value - translation.Value;
                    float3 projectedAimDir = math.project(aimDir, math.up());
                    result = quaternion.LookRotation(aimDir-projectedAimDir, Vector3.up);
                }
                else
                {
                    float3 aimDir = aimPosData.Value - translation.Value;
                    float3 projectedAimDir = math.project(aimDir, math.up());
                    var targetRot = quaternion.LookRotation(aimDir - projectedAimDir, Vector3.up);
                    result = Quaternion.RotateTowards(rotation.Value, targetRot, rotSpeedConfig.Value*Δt);
                }
            }
            rotation.Value = result;
        }
    }
}