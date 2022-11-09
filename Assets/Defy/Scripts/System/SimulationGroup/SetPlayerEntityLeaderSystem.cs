using Defy.Component;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;


namespace Defy.System
{
    [UpdateInGroup(typeof(DefyUpdateGroup))]
    public partial class SetPlayerEntityLeaderSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            Entities
                .WithAll<WeaponConfig>()
                .ForEach((
                    Entity e,
                    int entityInQueryIndex,
                    ref EntityLeaderData leaderData,
                    in Translation translation,
                    in Rotation rotation
                ) =>
                {
                    leaderData.Position = translation.Value;
                    leaderData.Rotation = rotation.Value;
                }).ScheduleParallel();
        }
    }
}