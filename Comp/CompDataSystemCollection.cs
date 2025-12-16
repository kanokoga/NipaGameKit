using System;
using System.Collections.Generic;
using NipaFriends;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// Systemの管理クラス
    /// 型ごとにSystemインスタンスを保持し、更新を管理
    /// </summary>
    public class CompDataSystemCollection : SingletonMonoBehaviour<CompDataSystemCollection>
    {
        private Dictionary<Type, object> _systems = new Dictionary<Type, object>();
        private List<ISystemUpdater> _updaters = new List<ISystemUpdater>();


        /// <summary>
        /// Systemを登録
        /// </summary>
        public void RegisterSystem<TData>(CompDataSystem<TData> dataSystem) where TData : struct, ICompData
        {
            var type = typeof(TData);
            if (!this._systems.ContainsKey(type))
            {
                this._systems[type] = dataSystem;
                this._updaters.Add(new SystemUpdater<TData>(dataSystem));
            }
        }

        /// <summary>
        /// Systemを取得
        /// </summary>
        public CompDataSystem<TData> GetSystem<TData>() where TData : struct, ICompData
        {
            var type = typeof(TData);
            if (this._systems.TryGetValue(type, out var system))
            {
                return (CompDataSystem<TData>)system;
            }
            return null;
        }

        /// <summary>
        /// すべてのSystemを更新
        /// </summary>
        public void UpdateSystems(float time, float deltaTime)
        {
            for (var i = 0; i < this._updaters.Count; i++)
            {
                this._updaters[i].Update(time, deltaTime);
            }
        }

        protected override void OnDestroy()
        {
            this._systems.Clear();
            this._updaters.Clear();
            base.OnDestroy();
        }

        // System更新のためのインターフェース
        private interface ISystemUpdater
        {
            void Update(float time, float deltaTime);
        }

        private class SystemUpdater<TData> : ISystemUpdater where TData : struct, ICompData
        {
            private CompDataSystem<TData> dataSystem;

            public SystemUpdater(CompDataSystem<TData> dataSystem)
            {
                this.dataSystem = dataSystem;
            }

            public void Update(float time, float deltaTime)
            {
                this.dataSystem.Update(time, deltaTime);
            }
        }
    }
}

