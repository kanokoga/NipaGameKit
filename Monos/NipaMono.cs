using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// データ構造ベースのコンポーネントシステムのエントリーポイント
    /// CompDataProviderを自動的に検出して初期化
    /// </summary>
    public class NipaMono : MonoBehaviour
    {
        private static int GlobalMonoId = 0;
        public int MonoId { get; private set; }
        private MonoBehaviour[] _dataProviders;

        private void Awake()
        {
            this.MonoId = GlobalMonoId;
            #if UNITY_EDITOR
            this.gameObject.name += $"_{this.MonoId}";
            #endif

            GlobalMonoId++;

            // すべてのCompDataProviderを取得して初期化
            // CompDataProvider<T>を継承しているコンポーネントを検出
            _dataProviders = GetComponents<MonoBehaviour>()
                .Where(comp => IsCompDataProvider(comp))
                .OrderBy(comp => GetInitOrder(comp))
                .ToArray();

            foreach (var provider in _dataProviders)
            {
                // リフレクションでInitを呼び出す
                var initMethod = provider.GetType().GetMethod("Init", new[] { typeof(int) });
                if (initMethod != null)
                {
                    initMethod.Invoke(provider, new object[] { this.MonoId });
                }
            }
        }

        private bool IsCompDataProvider(MonoBehaviour comp)
        {
            var type = comp.GetType();
            while (type != null && type != typeof(MonoBehaviour))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CompDataProvider<>))
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        private int GetInitOrder(MonoBehaviour comp)
        {
            var property = comp.GetType().GetProperty("InitOrder");
            if (property != null)
            {
                var value = property.GetValue(comp);
                if (value is int order)
                {
                    return order;
                }
            }
            return 0;
        }

        private void Start()
        {
            MonoManager.Instance.SetMonoActive(this);
        }

        protected virtual void OnDestroy()
        {
            // CompRegistryから削除（各CompDataProviderのOnDestroyでも削除されるが念のため）
            CompRegistry.Unregister(this.MonoId);
        }
    }
}
