using System;
using System.Collections;
using System.Collections.Generic;
using NipaFriends;
using UnityEngine;

namespace NipaGameKit
{
    public class NipaEntityManager : SingletonMonoBehaviour<NipaEntityManager>
    {
        public static IReadOnlyCollection<int> ActiveMonoIds => _ActiveMonoIds;
        public static event Action<int> OnMonoIsPreReady = delegate { };
        public static event Action<int> OnMonoIsReady = delegate { };
        public static event Action<int> OnMonoIsDying = delegate { };
        private static List<int> _ActiveMonoIds = new List<int>();


        public void SetEntityActive(NipaEntity nipaEntity)
        {
            OnMonoIsPreReady.Invoke(nipaEntity.EntityId);
            nipaEntity.gameObject.SetActive(true);
            OnMonoIsReady.Invoke(nipaEntity.EntityId);
            _ActiveMonoIds.Add(nipaEntity.EntityId);
        }

        public void SetEntityDead(NipaEntity nipaEntity)
        {
            _ActiveMonoIds.Remove(nipaEntity.EntityId);
            OnMonoIsDying.Invoke(nipaEntity.EntityId);
        }

        private void OnDestroy()
        {
            OnMonoIsReady = delegate { };
            OnMonoIsDying = delegate { };
            _ActiveMonoIds.Clear();
        }
    }
}
