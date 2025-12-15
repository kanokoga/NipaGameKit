using System;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// Systemの管理クラス
    /// 型ごとにSystemインスタンスを保持し、更新を管理
    /// </summary>
    public class CompSystemsUpdater : MonoBehaviour
    {
        private static CompSystemsUpdater _instance;
        public static CompSystemsUpdater Instance
        {
            get
            {
                return _instance;
            }
        }

        private Dictionary<Type, object> _systems = new Dictionary<Type, object>();
        private List<ISystemUpdater> _updaters = new List<ISystemUpdater>();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Systemを登録
        /// </summary>
        public void RegisterSystem<TData>(CompSystem<TData> system) where TData : struct, ICompData
        {
            Type type = typeof(TData);
            if (!_systems.ContainsKey(type))
            {
                _systems[type] = system;
                _updaters.Add(new SystemUpdater<TData>(system));
            }
        }

        /// <summary>
        /// Systemを取得
        /// </summary>
        public CompSystem<TData> GetSystem<TData>() where TData : struct, ICompData
        {
            Type type = typeof(TData);
            if (_systems.TryGetValue(type, out object system))
            {
                return (CompSystem<TData>)system;
            }
            return null;
        }

        /// <summary>
        /// すべてのSystemを更新
        /// </summary>
        public void UpdateSystems(float time, float deltaTime)
        {
            for (int i = 0; i < _updaters.Count; i++)
            {
                _updaters[i].Update(time, deltaTime);
            }
        }

        private void Update()
        {
            float time = GameLoop.CurrentTime;
            float deltaTime = Time.deltaTime;
            UpdateSystems(time, deltaTime);
        }

        private void OnDestroy()
        {
            _systems.Clear();
            _updaters.Clear();
            if (_instance == this)
            {
                _instance = null;
            }
        }

        // System更新のためのインターフェース
        private interface ISystemUpdater
        {
            void Update(float time, float deltaTime);
        }

        private class SystemUpdater<TData> : ISystemUpdater where TData : struct, ICompData
        {
            private CompSystem<TData> _system;

            public SystemUpdater(CompSystem<TData> system)
            {
                _system = system;
            }

            public void Update(float time, float deltaTime)
            {
                _system.Update(time, deltaTime);
            }
        }
    }
}

