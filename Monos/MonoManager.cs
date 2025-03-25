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


        public void SetMonoActive(NipaMono nipaMono)
        {
            OnMonoIsPreReady.Invoke(nipaMono.MonoId);
            nipaMono.gameObject.SetActive(true);
            OnMonoIsReady.Invoke(nipaMono.MonoId);
            _ActiveMonoIds.Add(nipaMono.MonoId);
        }

        public void SetMonoDead(NipaMono nipaMono)
        {
            _ActiveMonoIds.Remove(nipaMono.MonoId);
            OnMonoIsDying.Invoke(nipaMono.MonoId);
        }

        private void OnDestroy()
        {
            OnMonoIsReady = delegate { };
            OnMonoIsDying = delegate { };
            _ActiveMonoIds.Clear();
        }
    }
}
