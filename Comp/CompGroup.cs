using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NipaGameKit
{
    /// <summary>
    /// 既存のCompGroupをCompGroupDataベースに置き換え
    /// 互換性のため、同じAPIを提供するが内部実装はCompGroupDataを使用
    /// </summary>
    [Obsolete("CompGroupは非推奨です。CompGroupData<TData>を直接使用してください。")]
    public static class CompGroup<T> where T : ICompGroupElement
    {
        // このクラスは後方互換性のため残していますが、新しいコードでは使用しないでください
        // CompGroupData<TData>を直接使用することを推奨します
    }
}
