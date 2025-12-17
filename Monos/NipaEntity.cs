using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// データ構造ベースのコンポーネントシステムのエントリーポイント
    /// Compを自動的に検出して初期化
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

            // すべてのCompを取得して初期化
            // Comp<T>を継承しているコンポーネントを検出
            this.comps = this.GetComponents<MonoBehaviour>()
                .Where(comp => this.IsCompDataProvider(comp))
                .OrderBy(comp => this.GetInitOrder(comp))
                .ToArray();

            foreach (var provider in this.comps)
            {
                // インターフェースを使って直接呼び出し
                if (provider is IComp comp)
                {
                    comp.Init(this.EntityId);
                }
            }
        }

        private bool IsCompDataProvider(MonoBehaviour comp)
        {
            return comp is IComp;
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
            // CompRegistryから削除（各CompのOnDestroyでも削除されるが念のため）
            UnityObjectRegistry.Unregister(this.EntityId);
        }
    }
}
