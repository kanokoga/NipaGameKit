using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// コンポーネントデータの基底インターフェース
    /// ECS的なアプローチでデータとロジックを分離
    /// </summary>
    public interface ICompData
    {
        int MonoId { get; set; }
        bool IsActive { get; set; }
    }

    /// <summary>
    /// データ構造体の基底（値型で連続メモリに配置）
    /// </summary>
    public struct CompDataBase : ICompData
    {
        public int MonoId { get; set; }
        public bool IsActive { get; set; }
    }
}

