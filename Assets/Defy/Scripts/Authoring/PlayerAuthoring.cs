using System;
using System.Collections.Generic;
using Cinemachine;
using Defy.MonoBehavior;
using KittyHelpYouOut;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

namespace Defy.Component
{
   
    [DisallowMultipleComponent]
    public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Movement")] 
        public SimpleCharacterMoveConfig CharacterMoveConfig;
        public bool IsSoftCollide;
        public bool InstanceRotate;
        public float RotateSpeed;
        [Header("Camera")] 
        public bool GenerateFollower=false;
        public GameObject Follower;
        public CinemachineVirtualCamera VCam;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PlayerTag());
            dstManager.AddComponentData(entity, new RotateSpeedConfig() { Value = RotateSpeed,InstantRotate = InstanceRotate});
            dstManager.AddComponentData(entity, new AimPosData());
            if (IsSoftCollide)
            {
                dstManager.AddComponentData(entity, new MovementData());
            }
            else
            {
                dstManager.AddComponentData(entity, new SimpleCharacterMovementData());
            }
            dstManager.AddComponentData(entity, CharacterMoveConfig);
            dstManager.AddComponentData(entity, new WeaponConfig());
            dstManager.AddComponentData(entity, new AmmoCountData());
            var randomSource = new IndividualRandomData();
            randomSource.Value = Random.CreateFromIndex((uint)DateTime.Now.Millisecond);
            dstManager.AddComponentData(entity, randomSource);
            dstManager.AddComponentData(entity, new ResetableTimerData());
            dstManager.AddComponentData(entity, new TriggerFlagData());
            
            //weapon setup is completely move to change weapon system
            
            if (GenerateFollower)
            {
                dstManager.AddComponentData(entity, new EntityLeaderData());
                var follower = KittyPool.Instance.GetPoolObject(Follower);
                if (follower==null)
                {
                    // Debug.LogError("follow entity prefab is not set",this);
                    // Debug.Break();
                }
                if (follower.TryGetComponent(out EntityFollower followEntity))
                {
                    followEntity.FollowTarget = entity;
                }
                else
                {
                    // Debug.LogError("follow entity component not found",this);
                }
                if (VCam!=null)
                {
                    VCam.Follow = follower.transform;
                    VCam.LookAt = follower.transform;
                }
                else
                {
                    // Debug.LogError("Virtual camera is not set", this);
                }
            }

        }

    }
}