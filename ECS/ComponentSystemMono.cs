using System;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit.ECS
{
    public abstract class ComponentSystemMono : MonoBehaviour
    {
        protected readonly List<Chunk> TargetChunks = new List<Chunk>();
        private readonly HashSet<Chunk> _registeredChunks = new HashSet<Chunk>();

        public abstract void UpdateSystem(float deltaTime);

        public virtual void Init()
        {
        }

        public void RegisterChunk(Chunk chunk)
        {
            if(chunk == null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }

            if(this._registeredChunks.Contains(chunk))
            {
                return;
            }

            if(this.Filter(chunk))
            {
                this.TargetChunks.Add(chunk);
                this._registeredChunks.Add(chunk);
            }
        }

        protected abstract bool Filter(Chunk chunk);
    }
}
