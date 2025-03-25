using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NipaGameKit
{
    public static class CompGroup<T> where T : ICompGroupElement
    {
        public static event Action<int> OnComponentAdded = delegate { };
        public static event Action<int> OnComponentRemoving = delegate { };
        public static event Action<int> OnComponentRemoved = delegate { };

        static Dictionary<int, T> ComponentDic = new Dictionary<int, T>();
        static List<T> ComponentList = new List<T>();


        static CompGroup()
        {
            StaticResetter.OnResetStatic += () => Dispose();
        }

        public static void Add(T module)
        {
            ComponentDic.Add(module.MonoId, module);
            ComponentList.Add(module);
            OnComponentAdded.Invoke(module.MonoId);
            Debug.Log($"{typeof(T).Name} CompGroup Added. id:{module.MonoId}");
        }

        public static void Remove(T module)
        {
            OnComponentRemoving.Invoke(module.MonoId);
            ComponentDic.Remove(module.MonoId);
            ComponentList.Remove(module);
            OnComponentRemoved.Invoke(module.MonoId);
        }

        public static void Update(float time, float deltaTime)
        {
            for(int i = 0; i < ComponentList.Count; i++)
            {
                var module = ComponentList[i];
                if(module.IsActive)
                {
                    module.UpdateComponent(time, deltaTime);
                }
            }
        }

        public static T GetComponent(int monoId)
            => ComponentDic[monoId];

        public static bool HasComponent(int monoId)
        {
            return ComponentDic.ContainsKey(monoId);
        }

        public static bool TryGetComponent(int monoId, out T module)
        {
            return ComponentDic.TryGetValue(monoId, out module);
        }

        public static IReadOnlyList<T> GetAll()
        {
            return ComponentList;
        }

        private static void Dispose()
        {
            OnComponentAdded = delegate { };
            OnComponentRemoved = delegate { };
            OnComponentRemoving = delegate { };

            for(int i = ComponentList.Count - 1; i >= 0; i--)
            {
                ComponentList[i].Dispose();
            }

            Debug.Log($"CompGroup: {typeof(T).Name} Cleared: {ComponentDic.Count}");
            ComponentDic.Clear();
            ComponentList.Clear();
        }
    }
}
