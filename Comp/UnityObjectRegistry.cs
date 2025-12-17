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
        private static Dictionary<int, Transform> EntityIdToTransform = new Dictionary<int, Transform>();
        private static Dictionary<int, GameObject> EntityIdToGameObject = new Dictionary<int, GameObject>();

        

        public static void Register(int entityId, Transform transform)
        {
            EntityIdToTransform[entityId] = transform;
            EntityIdToGameObject[entityId] = transform.gameObject;
        }

        public static void Unregister(int entityId)
        {
            if(EntityIdToTransform.ContainsKey(entityId))
            {
                EntityIdToTransform.Remove(entityId);
            }

            if(EntityIdToGameObject.ContainsKey(entityId))
            {
                EntityIdToGameObject.Remove(entityId);
            }
        }

        public static Transform GetTransform(int entityId)
        {
            return EntityIdToTransform.TryGetValue(entityId, out var transform) ? transform : null;
        }

        public static GameObject GetGameObject(int entityId)
        {
            return EntityIdToGameObject.TryGetValue(entityId, out var go) ? go : null;
        }

        public static Vector3 GetPosition(int entityId)
        {
            var transform = GetTransform(entityId);
            return transform != null ? transform.position : Vector3.zero;
        }

        public static Quaternion GetRotation(int entityId)
        {
            var transform = GetTransform(entityId);
            return transform != null ? transform.rotation : Quaternion.identity;
        }

        public static void SetPosition(int entityId, Vector3 position)
        {
            var transform = GetTransform(entityId);
            if (transform != null)
            {
                transform.position = position;
            }
        }

        public static void SetRotation(int entityId, Quaternion rotation)
        {
            var transform = GetTransform(entityId);
            if (transform != null)
            {
                transform.rotation = rotation;
            }
        }

        private static void Clear()
        {
            EntityIdToTransform.Clear();
            EntityIdToGameObject.Clear();
        }
    }
}

