using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NipaGameKit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMoveComp : MoveCompBase
    {
        protected NavMeshAgent navMeshAgent;


        public override void Init(int monoId)
        {
            base.Init(monoId);
            this.navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        }

        public override void SetDestination(Vector3 destination)
        {
            this.navMeshAgent.SetDestination(destination);
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
