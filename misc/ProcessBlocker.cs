using System;
using UnityEngine;

namespace NipaGameKit
{
    public class ProcessBlocker
    {
        public int BlockerCount { get; private set; } = 0;
        public string Name { get; private set; }

        private Action<bool> setActiveCallback;


        public ProcessBlocker(string name, Action<bool> setActiveCallback)
        {
            this.Name = name;
            this.setActiveCallback = setActiveCallback;
        }

        public void AddBlocker()
        {
            this.BlockerCount++;
            if(this.BlockerCount > 0)
            {
                this.setActiveCallback(false);
            }
        }

        public void RemoveBlocker()
        {
            this.BlockerCount--;
            if(this.BlockerCount < 0)
            {
                Debug.LogError($"Blocker count for {this.Name} went below zero!");
                this.BlockerCount = 0;
            }

            if(this.BlockerCount == 0)
            {
                this.setActiveCallback(true);
            }
        }
    }
}
