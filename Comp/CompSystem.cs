using System;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// コンポーネントデータを処理するSystem基底クラス
    /// ECS的なアプローチでロジックを分離
    /// </summary>
    public abstract class CompSystem<TData> where TData : struct, ICompData
    {
        /// <summary>
        /// システムの更新処理
        /// アクティブなデータを一括処理（キャッシュフレンドリー）
        /// </summary>
        public virtual void Update(float time, float deltaTime)
        {
            _currentTime = time;
            _currentDeltaTime = deltaTime;
            CompDataGroup<TData>.UpdateActiveData(UpdateDataCallback);
        }

        private void UpdateDataCallback(int monoId, ref TData data)
        {
            UpdateData(monoId, ref data, _currentTime, _currentDeltaTime);
        }

        private float _currentTime;
        private float _currentDeltaTime;

        /// <summary>
        /// 個別のデータを更新する処理（派生クラスで実装）
        /// </summary>
        protected abstract void UpdateData(int monoId, ref TData data, float time, float deltaTime);

        /// <summary>
        /// データが追加された時の処理
        /// </summary>
        public virtual void OnDataAdded(int monoId, ref TData data) { }

        /// <summary>
        /// データが削除される前の処理
        /// </summary>
        public virtual void OnDataRemoving(int monoId, ref TData data) { }

        /// <summary>
        /// データが削除された後の処理
        /// </summary>
        public virtual void OnDataRemoved(int monoId) { }
    }

}

