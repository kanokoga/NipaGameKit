using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField] private float timeScale = 1f;
        public static float CurrentTime { get; protected set; }
        private GameLoopBase[] gameLoops;


        private void Awake()
        {
            this.gameLoops = this.gameObject.GetComponentsInChildren<GameLoopBase>();
        }

        private void Update()
        {
            CurrentTime = Time.timeSinceLevelLoad * this.timeScale;
            var deltaTime = Time.deltaTime * this.timeScale;
            for(int i = 0; i < this.gameLoops.Length; i++)
            {
                this.gameLoops[i].UpdateGameloop(CurrentTime, deltaTime);
            }
        }
    }

    public abstract class GameLoopBase : MonoBehaviour
    {
        public abstract void UpdateGameloop(float time, float deltaTime);
    }
}
