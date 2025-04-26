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
        public bool HasArrived { get; protected set; } = false;
        public Action OnArrived { get; set; } = delegate{ };


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
            this.OnArrived = null;
            this._transform = null;
        }

        protected void InvokeArrived()
        {
            this.HasArrived = true;
            this.OnArrived?.Invoke();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if(Application.isPlaying == false)
            {
                return;
            }

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.Destination, 0.5f);
        }
    }
}
