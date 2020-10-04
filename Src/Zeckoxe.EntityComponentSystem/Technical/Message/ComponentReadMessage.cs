using Zeckoxe.EntityComponentSystem.Serialization;

namespace Zeckoxe.EntityComponentSystem.Technical.Message
{
    internal readonly struct ComponentReadMessage
    {
        public readonly int EntityId;
        public readonly IComponentReader Reader;

        public ComponentReadMessage(int entityId, IComponentReader reader)
        {
            EntityId = entityId;
            Reader = reader;
        }
    }
}
