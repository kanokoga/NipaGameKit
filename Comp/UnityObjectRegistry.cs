using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// MonoIdとUnityオブジェクト（Transformなど）のマッピング
    /// データ構造からUnityの機能にアクセスするためのレジストリ
    /// </summary>
    public static class UnityObjectRegistry
    {
        private static Dictionary<int, Transform> _monoIdToTransform = new Dictionary<int, Transform>();
        private static Dictionary<int, GameObject> _monoIdToGameObject = new Dictionary<int, GameObject>();

        public static void Register(int monoId, Transform transform)
        {
            _monoIdToTransform[monoId] = transform;
            _monoIdToGameObject[monoId] = transform.gameObject;
        }

        public static void Unregister(int monoId)
        {
            _monoIdToTransform.Remove(monoId);
            _monoIdToGameObject.Remove(monoId);
        }

        public static Transform GetTransform(int monoId)
        {
            return _monoIdToTransform.TryGetValue(monoId, out var transform) ? transform : null;
        }

        public static GameObject GetGameObject(int monoId)
        {
            return _monoIdToGameObject.TryGetValue(monoId, out var go) ? go : null;
        }

        public static Vector3 GetPosition(int monoId)
        {
            var transform = GetTransform(monoId);
            return transform != null ? transform.position : Vector3.zero;
        }

        public static Quaternion GetRotation(int monoId)
        {
            var transform = GetTransform(monoId);
            return transform != null ? transform.rotation : Quaternion.identity;
        }

        public static void SetPosition(int monoId, Vector3 position)
        {
            var transform = GetTransform(monoId);
            if (transform != null)
            {
                transform.position = position;
            }
        }

        public static void SetRotation(int monoId, Quaternion rotation)
        {
            var transform = GetTransform(monoId);
            if (transform != null)
            {
                transform.rotation = rotation;
            }
        }

        public static void Clear()
        {
            _monoIdToTransform.Clear();
            _monoIdToGameObject.Clear();
        }
    }
}

