using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace NipaGameKit
{
    public interface ICompGroupElement
    {
        public int MonoId { get; }
        public bool IsActive { get; }
        public void UpdateComponent(float time, float deltaTime);
        public void Dispose();
    }
}
