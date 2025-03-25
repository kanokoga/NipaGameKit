using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NipaGameKit
{
    public class OnDeathComp : CompMonoBase
    {
        private DeathActionBase[] deathActions;

        public override void Init(int monoId)
        {
            base.Init(monoId);
            this.deathActions = this.GetComponentsInChildren<DeathActionBase>();
            foreach(var d in this.deathActions)
            {
                d.Init(monoId);
            }

            CompGroup<OnDeathComp>.Add(this);
        }

        public virtual void OnDeath()
        {
            foreach(var d in this.deathActions)
            {
                d.OnDeath();
            }
        }

        public override void Dispose()
        {
            foreach(var d in this.deathActions)
            {
                d.Dispose();
            }
            CompGroup<OnDeathComp>.Remove(this);
        }
    }
}
