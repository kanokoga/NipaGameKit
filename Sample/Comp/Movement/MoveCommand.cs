using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// 移動コマンド
    /// </summary>
    public struct MoveCommand
    {
        public int MonoId;
        public Vector3 Destination;
        public float Speed; // オプション: このコマンド専用の速度
        public bool StopOnArrival; // 到着時に停止するか
    }

    /// <summary>
    /// 移動コマンドキュー
    /// </summary>
    public static class MoveCommandQueue
    {
        /// <summary>
        /// キューのサイズを取得
        /// </summary>
        public static int QueueSize => CommandQueue<MoveCommand>.Count;

        /// <summary>
        /// 移動コマンドを追加
        /// </summary>
        public static void EnqueueMove(int monoId, Vector3 destination, float speed = -1, bool stopOnArrival = true)
        {
            var command = new MoveCommand
            {
                MonoId = monoId,
                Destination = destination,
                Speed = speed,
                StopOnArrival = stopOnArrival
            };
            CommandQueue<MoveCommand>.Enqueue(command);
        }

        /// <summary>
        /// 停止コマンドを追加
        /// </summary>
        public static void EnqueueStop(int monoId)
        {
            var command = new MoveCommand
            {
                MonoId = monoId,
                Destination = UnityObjectRegistry.GetPosition(monoId), // 現在位置
                Speed = 0,
                StopOnArrival = true
            };
            CommandQueue<MoveCommand>.Enqueue(command);
        }

        /// <summary>
        /// コマンドを処理（Systemから呼ばれる）
        /// </summary>
        public static void ProcessCommands()
        {
            while(CommandQueue<MoveCommand>.TryDequeue(out MoveCommand command))
            {
                if(CompDataCollection<MoveCompData>.HasData(command.MonoId))
                {
                    ref var data = ref CompDataCollection<MoveCompData>.GetData(command.MonoId);
                    data.Destination = command.Destination;
                    data.HasArrived = false;

                    // 速度が指定されていれば更新
                    if(command.Speed > 0)
                    {
                        data.Speed = command.Speed;
                    }

                    // 停止コマンドの場合
                    if(command.Speed == 0)
                    {
                        data.Velocity = Vector3.zero;
                        data.HasArrived = true;
                    }
                }
            }
        }
    }
}
