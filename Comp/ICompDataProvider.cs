using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// データ構造を提供するコンポーネントの基底クラス
    /// MonoBehaviourとしてUnityエディタで設定可能だが、実際の処理はデータ構造で行う
    /// </summary>
    public abstract class CompDataProvider<TData> : MonoBehaviour
        where TData : struct, ICompData
    {
        public int MonoId { get; private set; }
        public virtual int InitOrder => 0;

        /// <summary>
        /// データ構造を作成（派生クラスで実装）
        /// </summary>
        protected abstract TData CreateData(int monoId);

        /// <summary>
        /// MonoBehaviourの状態をデータに同期（必要に応じてオーバーライド）
        /// </summary>
        protected virtual void SyncToData(ref TData data) { }

        /// <summary>
        /// データの状態をMonoBehaviourに同期（必要に応じてオーバーライド）
        /// </summary>
        protected virtual void SyncFromData(ref TData data) { }

        /// <summary>
        /// NipaMonoから呼ばれる初期化処理
        /// </summary>
        public void Init(int monoId)
        {
            MonoId = monoId;
            
            // Transformを登録
            CompRegistry.Register(monoId, transform);
            
            // データを作成して登録
            var data = CreateData(monoId);
            data.MonoId = monoId;
            data.IsActive = enabled;
            SyncToData(ref data);
            CompGroupData<TData>.Add(monoId, data);
        }

        protected virtual void OnEnable()
        {
            if (MonoId > 0)
            {
                CompGroupData<TData>.SetActive(MonoId, true);
            }
        }

        protected virtual void OnDisable()
        {
            if (MonoId > 0)
            {
                CompGroupData<TData>.SetActive(MonoId, false);
            }
        }

        protected virtual void OnDestroy()
        {
            if (MonoId > 0)
            {
                CompGroupData<TData>.Remove(MonoId);
                CompRegistry.Unregister(MonoId);
            }
        }

        /// <summary>
        /// データを取得（読み取り専用）
        /// </summary>
        public bool TryGetData(out TData data)
        {
            return CompGroupData<TData>.TryGetData(MonoId, out data);
        }

        /// <summary>
        /// データを取得（参照で変更可能）
        /// </summary>
        public ref TData GetData()
        {
            return ref CompGroupData<TData>.GetData(MonoId);
        }
    }
}

