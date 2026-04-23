using System.Collections.Generic;

namespace NipaGameKit.ECS
{
    public static class ChunkExtensions
    {
        public static bool TryGetIndexById<T>(this Chunk chunk, int targetId, out int index)
            where T : struct, IId
        {
            return chunk.TryGetIndexById<T>(targetId, out index);
        }

        public static bool TryGetChunkAndIndexById<T>(this IReadOnlyList<Chunk> chunks, int targetId, out Chunk chunk,
            out int index)
            where T : struct, IId
        {
            for(var i = 0; i < chunks.Count; i++)
            {
                var candidate = chunks[i];
                if(candidate.TryGetIndexById<T>(targetId, out index) == true)
                {
                    chunk = candidate;
                    return true;
                }
            }

            chunk = default;
            index = -1;
            return false;
        }
    }
}
