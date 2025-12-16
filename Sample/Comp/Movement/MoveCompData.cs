using UnityEngine;

namespace NipaGameKit
{
    /// <summary>
    /// 移動コンポーネントのデータ構造（SoA構造で保持）
    /// </summary>
    public struct MoveCompData : ICompData
    {
        public int MonoId { get; set; }
        public bool IsActive { get; set; }

        // 移動データ
        public Vector3 Destination;
        public bool HasArrived;
        public Vector3 Velocity;
        public float Speed;
    }

    /// <summary>
    /// 移動コンポーネントのSystem（データを処理するロジック）
    /// </summary>
    public class MoveCompDataSystem : CompDataSystem<MoveCompData>
    {
        public override void Update(float time, float deltaTime)
        {
            // まず、コマンドキューからコマンドを処理
            MoveCommandQueue.ProcessCommands();
            
            // その後、通常の更新処理
            base.Update(time, deltaTime);
        }

        protected override void UpdateData(int monoId, ref MoveCompData data, float time, float deltaTime)
        {
            if (data.HasArrived || !data.IsActive)
            {
                return;
            }

            Vector3 currentPos = GetPosition(monoId);
            Vector3 direction = (data.Destination - currentPos);
            float distance = direction.magnitude;

            if (distance < 0.1f)
            {
                data.HasArrived = true;
                data.Velocity = Vector3.zero;
            }
            else
            {
                direction.Normalize();
                data.Velocity = direction * data.Speed;
                
                // 位置を更新
                Vector3 newPos = currentPos + data.Velocity * deltaTime;
                UnityObjectRegistry.SetPosition(monoId, newPos);
            }
        }

        private Vector3 GetPosition(int monoId)
        {
            return UnityObjectRegistry.GetPosition(monoId);
        }
    }
}

