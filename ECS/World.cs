using System;
using System.Collections.Generic;

namespace NipaGameKit.ECS
{
    public class World
    {
        private List<Chunk> _chunks = new List<Chunk>();
        private List<ComponentSystem> _systems = new List<ComponentSystem>();

        public void AddSystem(ComponentSystem system)
        {
            _systems.Add(system);
            foreach(var chunk in _chunks) system.RegisterChunk(chunk);
        }

        public Chunk CreateChunk(int capacity, params Type[] types)
        {
            var chunk = new Chunk(capacity, types);
            _chunks.Add(chunk);
            // 既存のシステムに新しいChunkを教える
            foreach(var system in _systems) system.RegisterChunk(chunk);
            return chunk;
        }

        public void Update(float deltaTime)
        {
            foreach(var system in _systems) system.Update(deltaTime);
        }
    }
}
