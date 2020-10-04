using Zeckoxe.EntityComponentSystem.Technical.Command;
using System.Diagnostics.CodeAnalysis;

namespace Zeckoxe.EntityComponentSystem
{
    public static class AoTHelper
    {

        public static void RegisterMessage<T>()
        {
            using World world = new World();

            world.Subscribe(default(MessageHandler<T>));
        }


        [SuppressMessage("Performance", "RCS1242:Do not pass non-read-only struct by read-only reference.")]
        public static void RegisterComponent<T>()
        {
            static bool Filter(in T _) => true;

            new EntityRuleBuilder(default, default)
                .With<T>().WithEither<T>().Or<T>().WithEither<T>().With<T>().With<T>(Filter).WithEither<T>().With<T>(Filter)
                .Without<T>().WithoutEither<T>().Or<T>().WithoutEither<T>().Without<T>()
                .WhenAdded<T>().WhenAddedEither<T>().Or<T>().WhenAddedEither<T>().WhenAdded<T>()
                .WhenChanged<T>().WhenChangedEither<T>().Or<T>().WhenChangedEither<T>().WhenChanged<T>()
                .WhenRemoved<T>().WhenRemovedEither<T>().Or<T>().WhenRemovedEither<T>().WhenRemoved<T>();
        }


        public static void RegisterUnmanagedComponent<T>() where T : unmanaged
        {
            RegisterComponent<T>();

            using World world = new World();

            Entity entity = world.CreateEntity();

            unsafe
            {
                T value;

                UnmanagedComponentCommand<T>.WriteComponent(default, (byte*)&value, default);
                UnmanagedComponentCommand<T>.SetComponent(entity, default, (byte*)&value);
            }
        }
    }
}
