using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// データ構造を提供するコンポーネントの基底インターフェース
    /// </summary>
    public interface IComp
    {
        void Init(int entityId);
    }

    /// <summary>
    /// データ構造を提供するコンポーネントの基底クラス
    /// MonoBehaviourとしてUnityエディタで設定可能だが、実際の処理はデータ構造で行う
    /// </summary>
    public abstract class Comp<TData> : MonoBehaviour, IComp
        where TData : struct, ICompData
    {
        public int EntityId { get; private set; }
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
        /// NipaEntityから呼ばれる初期化処理
        /// </summary>
        void IComp.Init(int entityId)
        {
            this.EntityId = entityId;

            // Transformを登録
            UnityObjectRegistry.Register(entityId, this.transform);

            // データを作成して登録
            var data = this.CreateData(entityId);
            data.EntityId = entityId;
            data.IsActive = this.enabled;
            this.SyncToData(ref data);
            CompDataCollection<TData>.Add(entityId, data);
        }

        protected virtual void OnEnable()
        {
            if (this.EntityId > 0)
            {
                CompDataCollection<TData>.SetActive(this.EntityId, true);
            }
        }

        protected virtual void OnDisable()
        {
            if (this.EntityId > 0)
            {
                CompDataCollection<TData>.SetActive(this.EntityId, false);
            }
        }

        protected virtual void OnDestroy()
        {
            if (this.EntityId > 0)
            {
                CompDataCollection<TData>.Remove(this.EntityId);
                UnityObjectRegistry.Unregister(this.EntityId);
            }
        }

        /// <summary>
        /// データを取得（読み取り専用）
        /// </summary>
        public bool TryGetData(out TData data)
        {
            return CompDataCollection<TData>.TryGetData(this.EntityId, out data);
        }

        /// <summary>
        /// データを取得（参照で変更可能）
        /// </summary>
        public ref TData GetData()
        {
            return ref CompDataCollection<TData>.GetData(this.EntityId);
        }
    }
}

