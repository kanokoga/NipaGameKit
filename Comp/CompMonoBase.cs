using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NipaGameKit;
using UnityEngine.Serialization;

namespace NipaGameKit
{
    public abstract class CompMonoBase : MonoBehaviour, ICompGroupElement
    {
        public int MonoId { get; private set; }
        public bool IsActive { get; protected set; } = true;
        public virtual int InitOrder { get; } = 0;


        public virtual void Init(int monoId)
        {
            this.MonoId = monoId;
            this.IsActive = this.enabled;
        }

        public virtual void SetActive(bool isActive)
        {
            this.IsActive = isActive;
        }

        public virtual void UpdateComponent(float time, float deltaTime)
        {
        }

        public abstract void Dispose();
    }
}
