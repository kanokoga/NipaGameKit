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
    public class NipaEntity : MonoBehaviour
    {
        private static int GlobalMonoId = 0;
        public int EntityId { get; private set; }
        private MonoBehaviour[] comps;

        private void Awake()
        {
            this.EntityId = GlobalMonoId;
            #if UNITY_EDITOR
            this.gameObject.name += $"_{this.EntityId}";
            #endif

            GlobalMonoId++;

            // すべてのCompDataProviderを取得して初期化
            // CompDataProvider<T>を継承しているコンポーネントを検出
            this.comps = this.GetComponents<MonoBehaviour>()
                .Where(comp => this.IsCompDataProvider(comp))
                .OrderBy(comp => this.GetInitOrder(comp))
                .ToArray();

            foreach (var provider in this.comps)
            {
                // リフレクションでInitを呼び出す
                var initMethod = provider.GetType().GetMethod("Init", new[] { typeof(int) });
                if (initMethod != null)
                {
                    initMethod.Invoke(provider, new object[] { this.EntityId });
                }
            }
        }

        private bool IsCompDataProvider(MonoBehaviour comp)
        {
            var type = comp.GetType();
            while (type != null && type != typeof(MonoBehaviour))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Comp<>))
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
            NipaEntityManager.Instance.SetEntityActive(this);
        }

        protected virtual void OnDestroy()
        {
            // CompRegistryから削除（各CompDataProviderのOnDestroyでも削除されるが念のため）
            UnityObjectRegistry.Unregister(this.EntityId);
        }
    }
}
