using System;
using System.Collections;
using System.Collections.Generic;
using NipaGameKit;
using UnityEngine;

namespace NipaGameKit
{
    public abstract class MoveCompBase : CompMonoBase
    {
        public virtual Vector3 Velocity { get; protected set; }
        public virtual bool IsMoving => this.Velocity.sqrMagnitude > 0.01f;


        public override void Init(int monoId)
        {
            base.Init(monoId);
            CompGroup<MoveCompBase>.Add(this);
        }

        public override void UpdateComponent(float time, float deltaTime)
        {
            this._transform.LookAt(this._transform.position + this.Velocity);
            this._transform.position += this.Velocity * deltaTime;
        }

        public virtual void SetPosition(Vector3 position)
        {
            this._transform.position = position;
        }

        public virtual bool SetDestination(Vector3 destination)
        {
            return true;
        }

        public virtual void SetDirection(Vector3 direction)
        {
            this._transform.forward = direction;
        }

        public virtual void Stop()
        {
            this.Velocity = Vector3.zero;
        }

        public override void Dispose()
        {
            CompGroup<MoveCompBase>.Remove(this);
            this._transform = null;
        }

        protected void OnDrawGizmosSelected()
        {
            if(Application.isPlaying==false)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this._transform.position, this._transform.position + this.Velocity * 5f);
        }
    }
}
