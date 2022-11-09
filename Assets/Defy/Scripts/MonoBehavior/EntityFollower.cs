using Defy.Component;
using KittyHelpYouOut;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Defy.MonoBehavior
{
    public class EntityFollower : MonoBehaviour
    {
        private EntityManager _Manager;
        public Entity FollowTarget;
        public Animator FollowerAnimator;
        private Quaternion _LegRotation;
        private void Start()
        {
            _Manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update()
        {
            var leaderPos = (Vector3)_Manager.GetComponentData<EntityLeaderData>(FollowTarget).Position;
            var leaderRot = _Manager.GetComponentData<EntityLeaderData>(FollowTarget).Rotation;
            if (this.transform.position != leaderPos ) 
            {
                FollowerAnimator.SetInteger("State",2);
                var moveDir = Vector3.Normalize(leaderPos - transform.position);
                _LegRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            }
            else
            {
                FollowerAnimator.SetInteger("State",0);
            }
            this.transform.rotation = _LegRotation;
            var upperRotation= Vector3.SignedAngle(transform.forward, (Quaternion)leaderRot * Vector3.forward, Vector3.up);

            //upper animation direction: right is negative, left is positive
            var turnAnimationKey = Mathf.InverseLerp(-180, 180, upperRotation);
            FollowerAnimator.SetFloat("Turn",turnAnimationKey);
            this.transform.position = leaderPos;
        }
    }
}