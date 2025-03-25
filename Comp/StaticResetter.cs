using System;
using System.Collections;
using System.Collections.Generic;
using NipaFriends;
using UnityEngine;

namespace NipaGameKit
{
    public class StaticResetter : MonoBehaviour
    {
        public static event Action OnResetStatic = delegate { };

        private void OnDestroy()
        {
            OnResetStatic.Invoke();
        }
    }
}
