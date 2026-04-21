using System;
using System.Collections.Generic;

namespace NipaGameKit.ECS
{
    public abstract class ComponentSystem
    {
        // このシステムが実行対象とするChunkのリスト
        protected List<Chunk> TargetChunks = new List<Chunk>();

        // 実行（サブクラスでロジックを記述）
        public abstract void Update(float deltaTime);

        // 新しいChunkが生成された時にシステムに登録する
        public void RegisterChunk(Chunk chunk)
        {
            if(Filter(chunk))
            {
                TargetChunks.Add(chunk);
            }
        }

        // このシステムが必要なコンポーネントを持っているか判定
        protected abstract bool Filter(Chunk chunk);
    }

// 例： 具体的な実装：移動システム
    // public class MoveSystem : ComponentSystem
    // {
    //     protected override bool Filter(Chunk chunk) =>
    //         chunk.HasComponent<Position>() && chunk.HasComponent<Velocity>();
    //
    //     public override void Update(float deltaTime)
    //     {
    //         foreach(var chunk in TargetChunks)
    //         {
    //             // 配列を直接取得してループ
    //             var positions = chunk.GetArray<Position>();
    //             var velocities = chunk.GetArray<Velocity>();
    //
    //             for(int i = 0; i < chunk.Count; i++)
    //             {
    //                 positions[i].X += velocities[i].VX * deltaTime;
    //                 positions[i].Y += velocities[i].VY * deltaTime;
    //             }
    //         }
    //     }
    // }
}
