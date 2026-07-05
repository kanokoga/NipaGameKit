using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace NipaGameKit.ECS
{
    public class World
    {
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly List<ComponentSystemMono> _systems = new List<ComponentSystemMono>();
        private readonly Dictionary<ComponentSystemMono, ProfilerMarker> _systemMarkers =
            new Dictionary<ComponentSystemMono, ProfilerMarker>();

        private static readonly ProfilerMarker UpdateMarker = new ProfilerMarker("ECS.World.Update");

        public void AddSystem(ComponentSystemMono system)
        {
            if(system == null)
            {
                throw new ArgumentNullException(nameof(system));
            }

            this._systems.Add(system);
            this._systemMarkers[system] = new ProfilerMarker($"ECS.{system.GetType().Name}");
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
            UpdateMarker.Begin();
            foreach(var system in this._systems)
            {
                using(this._systemMarkers[system].Auto())
                {
                    system.UpdateSystem(deltaTime);
                }
            }

            UpdateMarker.End();
        }

        public IReadOnlyList<Chunk> Chunks => this._chunks;
        public IReadOnlyList<ComponentSystemMono> Systems => this._systems;
    }
}
