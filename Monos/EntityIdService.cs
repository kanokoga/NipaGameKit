using UnityEngine;

namespace NipaGameKit
{
    public static class EntityIdService
    {
        private static int EntityId = 0;

        public static int GetNextEntityId()
        {
            var id = EntityId;
            EntityId++;
            if(EntityId >= 256 - 1)
            {
                EntityId = 0;
            }
            return id;
        }

        public static void ResetEntityId()
        {
            EntityId = 0;
        }
    }
}
