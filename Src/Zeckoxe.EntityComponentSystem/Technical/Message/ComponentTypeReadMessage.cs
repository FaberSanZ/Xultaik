using Zeckoxe.EntityComponentSystem.Serialization;

namespace Zeckoxe.EntityComponentSystem.Technical.Message
{
    internal readonly struct ComponentTypeReadMessage
    {
        public readonly IComponentTypeReader Reader;

        public ComponentTypeReadMessage(IComponentTypeReader reader)
        {
            Reader = reader;
        }
    }
}
