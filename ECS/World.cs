using System;
using System.Collections.Generic;

namespace NipaGameKit.ECS
{
    public class World
    {
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly List<ComponentSystem> _systems = new List<ComponentSystem>();

        public void AddSystem(ComponentSystem system)
        {
            if(system == null)
            {
                throw new ArgumentNullException(nameof(system));
            }

            this._systems.Add(system);
            foreach(var chunk in this._chunks)
            {
                system.RegisterChunk(chunk);
            }
        }

        public Chunk CreateChunk(int capacity, params Type[] types)
        {
            var chunk = new Chunk(capacity, types);
            this._chunks.Add(chunk);
            // 既存のシステムに新しいChunkを教える
            foreach(var system in this._systems)
            {
                system.RegisterChunk(chunk);
            }

            return chunk;
        }

        public void Update(float deltaTime)
        {
            foreach(var system in this._systems)
            {
                system.Update(deltaTime);
            }
        }

        public IReadOnlyList<Chunk> Chunks => this._chunks;
        public IReadOnlyList<ComponentSystem> Systems => this._systems;
    }
}
