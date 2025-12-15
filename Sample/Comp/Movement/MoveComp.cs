using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// 移動コンポーネントのデータプロバイダー
    /// Unityエディタで設定可能だが、実際の処理はMoveCompDataとMoveCompSystemで行う
    /// </summary>
    public class MoveCompProvider : Comp<MoveCompData>
    {
        [SerializeField] private float speed = 5.0f;

        public override int InitOrder => 0;

        protected override MoveCompData CreateData(int monoId)
        {
            return new MoveCompData
            {
                MonoId = monoId,
                IsActive = enabled,
                Destination = transform.position,
                HasArrived = true,
                Velocity = Vector3.zero,
                Speed = speed
            };
        }

        protected override void SyncToData(ref MoveCompData data)
        {
            data.Speed = speed;
        }

        /// <summary>
        /// 目的地を設定（コマンドキューに追加）
        /// </summary>
        public void SetDestination(Vector3 destination)
        {
            MoveCommandQueue.EnqueueMove(MonoId, destination);
        }

        /// <summary>
        /// 目的地を設定（速度指定付き）
        /// </summary>
        public void SetDestination(Vector3 destination, float speed)
        {
            MoveCommandQueue.EnqueueMove(MonoId, destination, speed);
        }

        /// <summary>
        /// 停止（コマンドキューに追加）
        /// </summary>
        public void Stop()
        {
            MoveCommandQueue.EnqueueStop(MonoId);
        }

        protected void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            if (TryGetData(out MoveCompData data))
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(data.Destination, 0.5f);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, data.Destination);
            }
        }
    }
}

