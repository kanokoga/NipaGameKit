using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// MoveCompのテスト用スクリプト
    /// インスペクタでMonoIdと移動位置を指定して移動命令を出す
    /// </summary>
    public class MoveCompTester : MonoBehaviour
    {
        [Header("移動対象の設定")]
        [Tooltip("移動させるオブジェクトのMonoId")]
        [SerializeField] private int targetMonoId = 0;

        [Header("移動位置の設定")]
        [Tooltip("移動先の位置")]
        [SerializeField] private Vector3 destination = Vector3.zero;

        [Header("操作")]
        [Tooltip("このボタンを押すと移動命令を実行")]
        [SerializeField] private bool executeMove = false;

        [Tooltip("このボタンを押すと停止命令を実行")]
        [SerializeField] private bool executeStop = false;

        [Header("デバッグ情報")]
        [SerializeField] private bool showDebugInfo = true;

        private void Update()
        {
            // インスペクタのボタンが押されたら実行
            if (this.executeMove)
            {
                this.executeMove = false;
                this.ExecuteMove();
            }

            if (this.executeStop)
            {
                this.executeStop = false;
                this.ExecuteStop();
            }
        }

        /// <summary>
        /// 移動命令を実行（コマンドキューに追加）
        /// </summary>
        public void ExecuteMove()
        {
            if (!CompDataCollection<MoveCompData>.HasData(this.targetMonoId))
            {
                if (this.showDebugInfo)
                {
                    Debug.LogWarning($"MoveCompTester: MonoId {this.targetMonoId} のMoveCompDataが見つかりません");
                }
                return;
            }

            // コマンドキューに追加
            MoveCommandQueue.EnqueueMove(this.targetMonoId, this.destination);

            if (this.showDebugInfo)
            {
                var queueSize = MoveCommandQueue.QueueSize;
                Debug.Log($"MoveCompTester: MonoId {this.targetMonoId} に移動命令をキューに追加しました。目的地: {this.destination} (キューサイズ: {queueSize})");
            }
        }

        /// <summary>
        /// 停止命令を実行（コマンドキューに追加）
        /// </summary>
        public void ExecuteStop()
        {
            if (!CompDataCollection<MoveCompData>.HasData(this.targetMonoId))
            {
                if (this.showDebugInfo)
                {
                    Debug.LogWarning($"MoveCompTester: MonoId {this.targetMonoId} のMoveCompDataが見つかりません");
                }
                return;
            }

            // コマンドキューに追加
            MoveCommandQueue.EnqueueStop(this.targetMonoId);

            if (this.showDebugInfo)
            {
                var queueSize = MoveCommandQueue.QueueSize;
                Debug.Log($"MoveCompTester: MonoId {this.targetMonoId} に停止命令をキューに追加しました (キューサイズ: {queueSize})");
            }
        }

        /// <summary>
        /// 現在の状態を表示
        /// </summary>
        [ContextMenu("現在の状態を表示")]
        public void ShowCurrentState()
        {
            if (!CompDataCollection<MoveCompData>.TryGetData(this.targetMonoId, out var data))
            {
                Debug.LogWarning($"MoveCompTester: MonoId {this.targetMonoId} のMoveCompDataが見つかりません");
                return;
            }

            var currentPos = UnityObjectRegistry.GetPosition(this.targetMonoId);
            var distance = Vector3.Distance(currentPos, data.Destination);

            Debug.Log($"MoveCompTester: MonoId {this.targetMonoId} の状態\n" +
                     $"現在位置: {currentPos}\n" +
                     $"目的地: {data.Destination}\n" +
                     $"距離: {distance:F2}\n" +
                     $"速度: {data.Velocity}\n" +
                     $"到着済み: {data.HasArrived}\n" +
                     $"アクティブ: {data.IsActive}");
        }

        /// <summary>
        /// 目的地を現在位置から相対的に設定
        /// </summary>
        [ContextMenu("目的地を相対位置で設定")]
        public void SetDestinationRelative()
        {
            var currentPos = UnityObjectRegistry.GetPosition(this.targetMonoId);
            this.destination = currentPos + this.destination;
            this.ExecuteMove();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // 目的地を表示
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.destination, 0.5f);
            Gizmos.DrawLine(this.transform.position, this.destination);

            // ターゲットの現在位置を表示
            if (CompDataCollection<MoveCompData>.HasData(this.targetMonoId))
            {
                var targetPos = UnityObjectRegistry.GetPosition(this.targetMonoId);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(targetPos, 0.3f);
                Gizmos.DrawLine(targetPos, this.destination);
            }
        }
    }
}

