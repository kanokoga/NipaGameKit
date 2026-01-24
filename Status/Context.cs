using System;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class Context
    {
        // 外部（Checkerなど）からはこれを呼ぶ
        public T GetContext<T>() where T : Context
        {
            // 1. 共通処理：自分がその型なら即座に返す
            if (this is T result)
            {
                return result;
            }

            // 2. 個別処理：自分の中に含まれるコンテキストを検索する
            return this.SearchComponent<T>();
        }

        // 子クラスで「自分の中身」を探すロジックだけを書く
        protected virtual T SearchComponent<T>() where T : Context
        {
            return null; // デフォルトでは何も持っていない
        }
    }
}
