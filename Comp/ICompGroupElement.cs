using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace NipaGameKit
{
    /// <summary>
    /// 非推奨: 新しいシステムではICompDataとCompDataProviderを使用してください
    /// </summary>
    [Obsolete("ICompGroupElementは非推奨です。ICompDataとCompDataProvider<TData>を使用してください。")]
    public interface ICompGroupElement
    {
        public int MonoId { get; }
        public bool IsActive { get; }
        public void UpdateComponent(float time, float deltaTime);
        public void Dispose();
    }
}
