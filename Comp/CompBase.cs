using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NipaGameKit;

namespace NipaGameKit
{
    public abstract class CompBase : ICompGroupElement
    {
        public int MonoId { get; private set; }
        public bool IsActive { get; private set; } = true;


        public virtual void Init(int monoId)
        {
            this.MonoId = monoId;
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
