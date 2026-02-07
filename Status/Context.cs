using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NipaGameKit.Statuses
{
    public class Context
    {
        // 型ごとのメンバー情報をキャッシュ
        private static readonly Dictionary<Type, CachedMemberInfo> MemberCache = new Dictionary<Type, CachedMemberInfo>();

        private class CachedMemberInfo
        {
            public FieldInfo[] ContextFields;
            public PropertyInfo[] ContextProperties;
        }

        // 外部（Checkerなど）からはこれを呼ぶ
        public T GetContext<T>() where T : Context
        {
            // 1. 共通処理：自分がその型なら即座に返す
            if (this is T result)
            {
                return result;
            }

            // 2. 個別処理：自分の中に含まれるコンテキストを検索する
            return this.SearchContext<T>();
        }

        // 子クラスで「自分の中身」を探すロジックだけを書く
        // リフレクションで自動的にContext型のメンバーを検索する
        protected virtual T SearchContext<T>() where T : Context
        {
            var thisType = this.GetType();
            var targetType = typeof(T);

            // キャッシュから取得、なければ作成
            if (!MemberCache.TryGetValue(thisType, out var cachedInfo))
            {
                cachedInfo = new CachedMemberInfo
                {
                    ContextFields = thisType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                    ContextProperties = thisType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                };
                MemberCache[thisType] = cachedInfo;
            }

            // フィールドを検索
            foreach (var field in cachedInfo.ContextFields)
            {
                if (targetType.IsAssignableFrom(field.FieldType))
                {
                    var value = field.GetValue(this);
                    if (value is T result)
                    {
                        return result;
                    }
                }
            }

            // プロパティを検索
            foreach (var property in cachedInfo.ContextProperties)
            {
                if (property.CanRead && targetType.IsAssignableFrom(property.PropertyType))
                {
                    var value = property.GetValue(this);
                    if (value is T result)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}
