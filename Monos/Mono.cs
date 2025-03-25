using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace NipaGameKit
{
    public class Mono : MonoBehaviour
    {
        private static int GlobalMonoId = 0;
        public int MonoId { get; private set; }
        private CompMonoBase[] components;


        private void Awake()
        {
            this.MonoId = GlobalMonoId;
            GlobalMonoId++;
            this.components = this.GetComponentsInChildren<CompMonoBase>()
                .OrderBy(v=>v.InitOrder)
                .ToArray();
            for (int i = 0; i < this.components.Length; i++)
            {
                this.components[i].Init(this.MonoId);
            }
        }

        private void Start()
        {
            MonoManager.Instance.SetMonoActive(this);
        }

        protected virtual void OnDestroy()
        {
            for (int i = this.components.Length - 1; i >= 0; i--)
            {
                this.components[i].Dispose();
            }
        }
    }
}
