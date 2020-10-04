namespace Zeckoxe.EntityComponentSystem.Technical.Message
{
    internal readonly struct EntityDisposedMessage
    {
        public readonly int EntityId;

        public EntityDisposedMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}
