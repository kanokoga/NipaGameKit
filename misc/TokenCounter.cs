using System;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// トークンがあるとコールバックにtrueを渡し、トークンがなくなるとfalseを渡すクラス。
    /// </summary>
    public class TokenCounter
    {
        public int TokenCount { get; private set; } = 0;
        public string Name { get; private set; }

        private Action<bool> hasTokenAction;
        private bool inverse;
        private bool hasToken = false;


        public TokenCounter(string name, Action<bool> hasTokenAction, bool inverse = false)
        {
            this.Name = name;
            this.hasTokenAction = hasTokenAction;
            this.inverse = inverse;
        }

        public void AddToken()
        {
            this.TokenCount++;
            if(this.TokenCount > 0
               && this.hasToken == false)
            {
                this.hasToken = true;
                this.hasTokenAction(true ^ this.inverse);
            }
        }

        public void RemoveToken()
        {
            this.TokenCount--;
            if(this.TokenCount < 0)
            {
                Debug.LogError($"Token count for {this.Name} went below zero!");
                this.TokenCount = 0;
            }

            if(this.TokenCount == 0
               && this.hasToken == true)
            {
                this.hasToken = false;
                this.hasTokenAction(false ^ this.inverse);
            }
        }
    }
}
