using System;
using System.Collections.Generic;

namespace NipaGameKit.ECS
{
    public abstract class ComponentSystem
    {
        // このシステムが実行対象とするChunkのリスト
        protected readonly List<Chunk> TargetChunks = new List<Chunk>();
        private readonly HashSet<Chunk> _registeredChunks = new HashSet<Chunk>();

        // 実行（サブクラスでロジックを記述）
        public abstract void Update(float deltaTime);

        // 新しいChunkが生成された時にシステムに登録する
        public void RegisterChunk(Chunk chunk)
        {
            if(chunk == null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }

            if(this._registeredChunks.Contains(chunk))
            {
                return;
            }

            if(Filter(chunk))
            {
                this.TargetChunks.Add(chunk);
                this._registeredChunks.Add(chunk);
            }
        }

        // このシステムが必要なコンポーネントを持っているか判定
        protected abstract bool Filter(Chunk chunk);
    }
}
