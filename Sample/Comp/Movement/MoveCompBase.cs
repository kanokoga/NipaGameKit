using System;
using System.Collections;
using System.Collections.Generic;
using NipaGameKit;
using UnityEngine;

namespace NipaGameKit
{
    public abstract class MoveCompBase : CompMonoBase
    {
        public Vector3 Destination { get; protected set; }

        public override void Init(int monoId)
        {
            base.Init(monoId);
            CompGroup<MoveCompBase>.Add(this);
        }

        public override void UpdateComponent(float time, float deltaTime)
        {

        }

        public virtual void SetPosition(Vector3 position)
        {
            this._transform.position = position;
        }

        public virtual void SetDestination(Vector3 destination)
        {
            this.Destination = destination;
        }

        public virtual void SetDirection(Vector3 direction)
        {
            this._transform.forward = direction;
        }

        public virtual void Stop()
        {

        }

        public override void Dispose()
        {
            CompGroup<MoveCompBase>.Remove(this);
            this._transform = null;
        }

        protected void OnDrawGizmosSelected()
        {
            if(Application.isPlaying == false)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this._transform.position, this.Destination);
        }
    }
}
