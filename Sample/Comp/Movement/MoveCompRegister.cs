using System;
using UnityEngine;

namespace NipaGameKit
{
    public class MoveCompRegister : MonoBehaviour
    {
        private void Awake()
        {
            var moveCompSystem = new MoveCompDataSystem();
            CompDataSystemCollection.Instance.RegisterSystem(moveCompSystem);
        }
    }
}
