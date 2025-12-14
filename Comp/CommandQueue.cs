using System.Collections.Generic;
using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// コマンドキューシステムの基底クラス
    /// コマンドをキューイングして順番に処理
    /// </summary>
    public static class CommandQueue<TCommand> where TCommand : struct
    {
        private static Queue<TCommand> _commandQueue = new Queue<TCommand>();
        private static int _maxQueueSize = 100;

        /// <summary>
        /// コマンドをキューに追加
        /// </summary>
        public static bool Enqueue(TCommand command)
        {
            if (_commandQueue.Count >= _maxQueueSize)
            {
                Debug.LogWarning($"CommandQueue<{typeof(TCommand).Name}>: キューが満杯です。コマンドを破棄します。");
                return false;
            }

            _commandQueue.Enqueue(command);
            return true;
        }

        /// <summary>
        /// コマンドをキューから取得（FIFO）
        /// </summary>
        public static bool TryDequeue(out TCommand command)
        {
            if (_commandQueue.Count > 0)
            {
                command = _commandQueue.Dequeue();
                return true;
            }

            command = default;
            return false;
        }

        /// <summary>
        /// キューをクリア
        /// </summary>
        public static void Clear()
        {
            _commandQueue.Clear();
        }

        /// <summary>
        /// キューのサイズを取得
        /// </summary>
        public static int Count => _commandQueue.Count;

        /// <summary>
        /// 最大キューサイズを設定
        /// </summary>
        public static int MaxQueueSize
        {
            get => _maxQueueSize;
            set => _maxQueueSize = Mathf.Max(1, value);
        }
    }
}

