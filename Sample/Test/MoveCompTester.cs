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
            if (executeMove)
            {
                executeMove = false;
                ExecuteMove();
            }

            if (executeStop)
            {
                executeStop = false;
                ExecuteStop();
            }
        }

        /// <summary>
        /// 移動命令を実行（コマンドキューに追加）
        /// </summary>
        public void ExecuteMove()
        {
            if (!CompDataGroup<MoveCompData>.HasData(targetMonoId))
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning($"MoveCompTester: MonoId {targetMonoId} のMoveCompDataが見つかりません");
                }
                return;
            }

            // コマンドキューに追加
            MoveCommandQueue.EnqueueMove(targetMonoId, destination);

            if (showDebugInfo)
            {
                int queueSize = MoveCommandQueue.QueueSize;
                Debug.Log($"MoveCompTester: MonoId {targetMonoId} に移動命令をキューに追加しました。目的地: {destination} (キューサイズ: {queueSize})");
            }
        }

        /// <summary>
        /// 停止命令を実行（コマンドキューに追加）
        /// </summary>
        public void ExecuteStop()
        {
            if (!CompDataGroup<MoveCompData>.HasData(targetMonoId))
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning($"MoveCompTester: MonoId {targetMonoId} のMoveCompDataが見つかりません");
                }
                return;
            }

            // コマンドキューに追加
            MoveCommandQueue.EnqueueStop(targetMonoId);

            if (showDebugInfo)
            {
                int queueSize = MoveCommandQueue.QueueSize;
                Debug.Log($"MoveCompTester: MonoId {targetMonoId} に停止命令をキューに追加しました (キューサイズ: {queueSize})");
            }
        }

        /// <summary>
        /// 現在の状態を表示
        /// </summary>
        [ContextMenu("現在の状態を表示")]
        public void ShowCurrentState()
        {
            if (!CompDataGroup<MoveCompData>.TryGetData(targetMonoId, out MoveCompData data))
            {
                Debug.LogWarning($"MoveCompTester: MonoId {targetMonoId} のMoveCompDataが見つかりません");
                return;
            }

            Vector3 currentPos = CompRegistry.GetPosition(targetMonoId);
            float distance = Vector3.Distance(currentPos, data.Destination);

            Debug.Log($"MoveCompTester: MonoId {targetMonoId} の状態\n" +
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
            Vector3 currentPos = CompRegistry.GetPosition(targetMonoId);
            destination = currentPos + destination;
            ExecuteMove();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            // 目的地を表示
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(destination, 0.5f);
            Gizmos.DrawLine(transform.position, destination);

            // ターゲットの現在位置を表示
            if (CompDataGroup<MoveCompData>.HasData(targetMonoId))
            {
                Vector3 targetPos = CompRegistry.GetPosition(targetMonoId);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(targetPos, 0.3f);
                Gizmos.DrawLine(targetPos, destination);
            }
        }
    }
}

