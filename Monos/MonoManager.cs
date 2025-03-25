using System;
using System.Collections;
using System.Collections.Generic;
using NipaFriends;
using UnityEngine;

namespace NipaGameKit
{
    public class MonoManager : SingletonMonoBehaviour<MonoManager>
    {
        public static IReadOnlyCollection<int> ActiveMonoIds => _ActiveMonoIds;
        public static event Action<int> OnMonoIsPreReady = delegate { };
        public static event Action<int> OnMonoIsReady = delegate { };
        public static event Action<int> OnMonoIsDying = delegate { };
        private static List<int> _ActiveMonoIds = new List<int>();


        public void SetMonoActive(Mono mono)
        {
            OnMonoIsPreReady.Invoke(mono.MonoId);
            mono.gameObject.SetActive(true);
            OnMonoIsReady.Invoke(mono.MonoId);
            _ActiveMonoIds.Add(mono.MonoId);
        }

        public void SetMonoDead(Mono mono)
        {
            _ActiveMonoIds.Remove(mono.MonoId);
            OnMonoIsDying.Invoke(mono.MonoId);
        }

        private void OnDestroy()
        {
            OnMonoIsReady = delegate { };
            OnMonoIsDying = delegate { };
            _ActiveMonoIds.Clear();
        }
    }
}
