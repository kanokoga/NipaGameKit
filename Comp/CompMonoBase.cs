using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NipaGameKit;
using UnityEngine.Serialization;

namespace NipaGameKit
{
    /// <summary>
    /// 非推奨: 新しいシステムではCompDataProvider<TData>を使用してください
    /// </summary>
    [Obsolete("CompMonoBaseは非推奨です。CompDataProvider<TData>を使用してください。")]
    public abstract class CompMonoBase : MonoBehaviour, ICompGroupElement
    {
        public virtual Vector3 Position => this._transform.position;
        public virtual Vector3 Forward => this._transform.forward;
        public virtual Quaternion Rotation => this._transform.rotation;
        public int MonoId { get; private set; }
        public bool IsActive { get; protected set; } = true;
        public virtual int InitOrder { get; } = 0;
        protected Transform _transform;


        public virtual void Init(int monoId)
        {
            this._transform = this.transform;
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

        public virtual void Dispose()
        {
            this._transform = null;
        }
    }
}
