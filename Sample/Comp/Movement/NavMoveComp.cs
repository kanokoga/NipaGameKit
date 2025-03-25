using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NipaGameKit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMoveComp : MoveCompBase
    {
        public override Vector3 Velocity => this.navMeshAgent.velocity;
        protected NavMeshAgent navMeshAgent;


        public override void Init(int monoId)
        {
            base.Init(monoId);
            this.navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        }

        public override bool SetDestination(Vector3 destination)
        {
            if(this.IsActive == false)
            {
                return false;
            }

            if(this.navMeshAgent.isOnNavMesh == false)
            {
                return false;
            }

            this.navMeshAgent.SetDestination(destination);
            return true;
        }

        public override void UpdateComponent(float time, float deltaTime)
        {

        }

        public override void Stop()
        {
            if(this.IsActive == false)
            {
                return;
            }
            this.navMeshAgent.ResetPath();
        }

        public override void Dispose()
        {
            this.navMeshAgent = null;
            base.Dispose();
        }
    }
}
