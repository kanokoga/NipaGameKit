using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    public class ComponentUpdater : GameLoopBase
    {
        public override void UpdateGameloop(float time, float deltaTime)
        {
            CompGroup<MoveCompBase>.Update(time, deltaTime);
        }
    }
}
