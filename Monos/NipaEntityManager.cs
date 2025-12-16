using System;
using System.Collections;
using System.Collections.Generic;
using NipaFriends;
using UnityEngine;

namespace NipaGameKit
{
    public class NipaEntityManager : SingletonMonoBehaviour<NipaEntityManager>
    {
        public  IReadOnlyCollection<int> ActiveEntityIds => this.activeEntityIds;
        public  event Action<int> OnMonoIsPreReady = delegate { };
        public  event Action<int> OnMonoIsReady = delegate { };
        public  event Action<int> OnMonoIsDying = delegate { };
        private  List<int> activeEntityIds = new List<int>();


        public void SetEntityActive(NipaEntity nipaEntity)
        {
            this.OnMonoIsPreReady.Invoke(nipaEntity.EntityId);
            nipaEntity.gameObject.SetActive(true);
            this.OnMonoIsReady.Invoke(nipaEntity.EntityId);
            this.activeEntityIds.Add(nipaEntity.EntityId);
        }

        public void SetEntityDead(NipaEntity nipaEntity)
        {
            this.activeEntityIds.Remove(nipaEntity.EntityId);
            this.OnMonoIsDying.Invoke(nipaEntity.EntityId);
        }

        private void OnDestroy()
        {
            this.OnMonoIsReady = delegate { };
            this.OnMonoIsDying = delegate { };
            this.activeEntityIds.Clear();
        }
    }
}
