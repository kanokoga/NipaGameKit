using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    public abstract class DeathActionBase : MonoBehaviour
    {
        public abstract void Init(int monoId);
        public abstract void OnDeath();
        public abstract void Dispose();
    }
}
