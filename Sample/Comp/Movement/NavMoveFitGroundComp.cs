using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NipaGameKit
{
    public class NavMoveFitGroundComp : NavMoveComp
    {
        [SerializeField] private float angle = 0f;
        [SerializeField] private Transform model;

        public override void UpdateComponent(float time, float deltaTime)
        {
            if(this.IsActive == false)
            {
                return;
            }

            if(this.navMeshAgent.hasPath == true
               && this.navMeshAgent.remainingDistance <= 0.1f)
            {
                this.navMeshAgent.ResetPath();
                return;
            }

            if(this.navMeshAgent.hasPath == false)
            {
                return;
            }

            base.UpdateComponent(time, deltaTime);

            // if(Physics.Raycast(this.model.position + Vector3.up * 5f,
            //        Vector3.down,
            //        out var hit, 10f, GameLayerMask.GroundOnly))
            // {
            //     var normal = hit.normal;
            //     var forward = this.Velocity.normalized;
            //     var dot = Vector3.Dot(normal, forward);
            //     var projectedForward = forward - normal * dot;
            //     var goalRotation = Quaternion.LookRotation(projectedForward, normal);
            //     this.model.rotation =
            //         Quaternion.RotateTowards(this.model.rotation, goalRotation, deltaTime * this.angle);
            //     //Debug.DrawLine(this.model.position, this.model.position + projectedForward * 10f, Color.cyan);
            // }
        }
    }
}
