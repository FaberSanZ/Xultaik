// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// Copyright (c) Paillat Laszlo. https://github.com/Doraku/DefaultEcs
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	World.cs
=============================================================================*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Zeckoxe.EntityComponentSystem.Serialization;
using Zeckoxe.EntityComponentSystem.Technical;
using Zeckoxe.EntityComponentSystem.Technical.Debug;
using Zeckoxe.EntityComponentSystem.Technical.Helper;
using Zeckoxe.EntityComponentSystem.Technical.Message;
using Zeckoxe.EntityComponentSystem.Threading;

namespace Zeckoxe.EntityComponentSystem
{

    [DebuggerTypeProxy(typeof(WorldDebugView))]
    public sealed class World : IEnumerable<Entity>, IPublisher, IDisposable
    {
        private class Optimizer : IParallelRunnable
        {
            private readonly List<ISortable> _items;

            private Action _mainAction;
            private bool shouldContinue;
            private int _lastIndex;

            public Optimizer()
            {
                _items = new List<ISortable>();
            }

            public void Add(ISortable item)
            {
                lock (this)
                {
                    _items.Add(item);
                }
            }

            public void Remove(ISortable item)
            {
                lock (this)
                {
                    _items.Remove(item);
                }
            }

            public void PrepareForRun(Action mainAction)
            {
                _mainAction = mainAction;
                Volatile.Write(ref shouldContinue, true);
                Interlocked.Exchange(ref _lastIndex, -1);
            }

            public void Run(int index, int maxIndex)
            {
                if (index == maxIndex && _mainAction != null)
                {
                    _mainAction();
                    Volatile.Write(ref shouldContinue, false);
                }

                while (Volatile.Read(ref shouldContinue) && (index = Interlocked.Increment(ref _lastIndex)) < _items.Count)
                {
                    _items[index].Sort(ref shouldContinue);
                }
            }
        }


        public struct Enumerator : IEnumerator<Entity>
        {
            private readonly short _worldId;
            private readonly EntityInfo[] _entityInfos;
            private readonly int _maxIndex;

            private int _index;

            internal Enumerator(World world)
            {
                _worldId = world.WorldId;
                _entityInfos = world.EntityInfos;
                _maxIndex = Math.Min(_entityInfos.Length, world.LastEntityId + 1);

                _index = -1;
            }



            public Entity Current => new Entity(_worldId, _index);

            object IEnumerator.Current => Current;


            public bool MoveNext()
            {
                while (++_index < _maxIndex)
                {
                    if (_entityInfos[_index].Components[IsAliveFlag])
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Reset() => _index = -1;




            public void Dispose()
            {
            }

        }




        private static readonly object _lockObject;
        private static readonly IntDispenser _worldIdDispenser;

        internal static readonly ComponentFlag IsAliveFlag;
        internal static readonly ComponentFlag IsEnabledFlag;

        internal static World[] Worlds;

        private readonly IntDispenser _entityIdDispenser;
        private readonly Optimizer _optimizer;

        internal readonly short WorldId;

        private volatile bool _isDisposed;

        internal EntityInfo[] EntityInfos;

        internal int LastEntityId => _entityIdDispenser.LastInt;




        static World()
        {
            _lockObject = new object();
            _worldIdDispenser = new IntDispenser(0);

            Worlds = new World[2];

            IsAliveFlag = ComponentFlag.GetNextFlag();
            IsEnabledFlag = ComponentFlag.GetNextFlag();
        }


        public World(int maxCapacity)
        {
            if (maxCapacity < 0)
            {
                throw new ArgumentException("Argument cannot be negative", nameof(maxCapacity));
            }

            _entityIdDispenser = new IntDispenser(-1);
            _optimizer = new Optimizer();
            WorldId = (short)_worldIdDispenser.GetFreeInt();

            MaxCapacity = maxCapacity;
            EntityInfos = EmptyArray<EntityInfo>.Value;

            lock (_lockObject)
            {
                ArrayExtension.EnsureLength(ref Worlds, WorldId);

                Worlds[WorldId] = this;
            }

            Subscribe<EntityDisposedMessage>(On);

            _isDisposed = false;
        }


        public World() : this(int.MaxValue)
        {

        }

        public int MaxCapacity { get; }




        private void On(in EntityDisposedMessage message)
        {
            ref EntityInfo entityInfo = ref EntityInfos[message.EntityId];

            entityInfo.Components.Clear();
            _entityIdDispenser.ReleaseInt(message.EntityId);
            ++entityInfo.Version;

            Func<int, bool> cleanParent = entityInfo.Parents;
            entityInfo.Parents = null;
            cleanParent?.Invoke(message.EntityId);

            HashSet<int> children = entityInfo.Children;
            if (children != null)
            {
                entityInfo.Children = null;
                foreach (int childId in children)
                {
                    EntityInfos[childId].Parents -= children.Remove;
                    Publish(new EntityDisposingMessage(childId));
                    Publish(new EntityDisposedMessage(childId));
                }
            }
        }



        internal void Add(ISortable optimizable) => _optimizer.Add(optimizable);

        internal void Remove(ISortable optimizable) => _optimizer.Remove(optimizable);


        public Entity CreateEntity()
        {
            int entityId = _entityIdDispenser.GetFreeInt();

            if (entityId >= MaxCapacity)
            {
                throw new InvalidOperationException("Max number of Entity reached");
            }

            ArrayExtension.EnsureLength(ref EntityInfos, entityId, MaxCapacity);

            EntityInfos[entityId].Components[IsAliveFlag] = true;
            EntityInfos[entityId].Components[IsEnabledFlag] = true;
            Publish(new EntityCreatedMessage(entityId));

            return new Entity(WorldId, entityId);
        }


        public bool SetMaxCapacity<T>(int maxCapacity)
        {
            if (maxCapacity < 0)
            {
                throw new ArgumentException("Argument cannot be negative", nameof(maxCapacity));
            }

            return ComponentManager<T>.GetOrCreate(WorldId, maxCapacity).MaxCapacity == maxCapacity;
        }


        public int GetMaxCapacity<T>() => ComponentManager<T>.Get(WorldId)?.MaxCapacity ?? -1;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Get<T>() => ComponentManager<T>.GetOrCreate(WorldId).AsSpan();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Components<T> GetComponents<T>() => ComponentManager<T>.GetOrCreate(WorldId).AsComponents();


        public EntityRuleBuilder GetEntities() => new EntityRuleBuilder(this, true);


        public EntityRuleBuilder GetDisabledEntities() => new EntityRuleBuilder(this, false);


        public void ReadAllComponentTypes(IComponentTypeReader reader) => Publish(new ComponentTypeReadMessage(reader ?? throw new ArgumentNullException(nameof(reader))));


        public void Optimize(IParallelRunner runner, Action mainAction)
        {
            if (runner is null)
            {
                throw new ArgumentNullException(nameof(runner));
            }

            if (mainAction is null)
            {
                throw new ArgumentNullException(nameof(mainAction));
            }

            _optimizer.PrepareForRun(mainAction);
            runner.Run(_optimizer);
        }


        public void Optimize(IParallelRunner runner)
        {
            if (runner is null)
            {
                throw new ArgumentNullException(nameof(runner));
            }

            _optimizer.PrepareForRun(null);
            runner.Run(_optimizer);
        }


        public void Optimize() => Optimize(DefaultParallelRunner.Default);


        public IDisposable SubscribeEntityCreated(EntityCreatedHandler action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in EntityCreatedMessage message) => action(new Entity(WorldId, message.EntityId)));
        }


        public IDisposable SubscribeEntityEnabled(EntityEnabledHandler action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in EntityEnabledMessage message) => action(new Entity(WorldId, message.EntityId)));
        }


        public IDisposable SubscribeEntityDisabled(EntityDisabledHandler action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in EntityDisabledMessage message) => action(new Entity(WorldId, message.EntityId)));
        }


        public IDisposable SubscribeEntityDisposed(EntityDisposedHandler action)
        {
            IEnumerable<IDisposable> GetSubscriptions(EntityDisposedHandler a)
            {
                yield return Subscribe((in EntityDisposingMessage message) => action(new Entity(WorldId, message.EntityId)));
                yield return Subscribe((in WorldDisposedMessage _) =>
                {
                    foreach (Entity entity in this)
                    {
                        a(entity);
                    }
                });
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return GetSubscriptions(action).Merge();
        }


        public IDisposable SubscribeComponentAdded<T>(ComponentAddedHandler<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in ComponentAddedMessage<T> message) => action(
                new Entity(WorldId, message.EntityId),
                ComponentManager<T>.Get(WorldId).Get(message.EntityId)));
        }


        public IDisposable SubscribeComponentChanged<T>(ComponentChangedHandler<T> action)
        {
            ComponentManager<T>.GetOrCreatePrevious(WorldId);

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in ComponentChangedMessage<T> message) => action(
                new Entity(WorldId, message.EntityId),
                ComponentManager<T>.GetPrevious(WorldId).Get(message.EntityId),
                ComponentManager<T>.Get(WorldId).Get(message.EntityId)));
        }


        public IDisposable SubscribeComponentRemoved<T>(ComponentRemovedHandler<T> action)
        {
            IEnumerable<IDisposable> GetSubscriptions(ComponentRemovedHandler<T> a)
            {
                yield return Subscribe((in ComponentRemovedMessage<T> message) => a(
                    new Entity(WorldId, message.EntityId),
                    ComponentManager<T>.GetPrevious(WorldId).Get(message.EntityId)));
                yield return Subscribe((in EntityDisposingMessage message) =>
                {
                    ComponentPool<T> pool = ComponentManager<T>.Get(WorldId);
                    if (pool?.Has(message.EntityId) is true)
                    {
                        a(new Entity(WorldId, message.EntityId), pool.Get(message.EntityId));
                    }
                });
                yield return Subscribe((in WorldDisposedMessage _) =>
                {
                    ComponentPool<T> pool = ComponentManager<T>.Get(WorldId);
                    if (pool != null)
                    {
                        foreach (Entity entity in pool.GetEntities())
                        {
                            a(entity, pool.Get(entity.EntityId));
                        }
                    }
                });
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            ComponentManager<T>.GetOrCreatePrevious(WorldId);

            return GetSubscriptions(action).Merge();
        }


        public IDisposable SubscribeComponentEnabled<T>(ComponentEnabledHandler<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in ComponentEnabledMessage<T> message) => action(
                new Entity(WorldId, message.EntityId),
                ComponentManager<T>.Get(WorldId).Get(message.EntityId)));
        }


        public IDisposable SubscribeComponentDisabled<T>(ComponentDisabledHandler<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Subscribe((in ComponentDisabledMessage<T> message) => action(
                new Entity(WorldId, message.EntityId),
                ComponentManager<T>.Get(WorldId).Get(message.EntityId)));
        }




        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable Subscribe<T>(MessageHandler<T> action) => Publisher<T>.Subscribe(WorldId, action);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Performance", "RCS1242:Do not pass non-read-only struct by read-only reference.")]
        public void Publish<T>(in T message) => Publisher<T>.Publish(WorldId, message);



        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                Publish(new WorldDisposedMessage(WorldId));
                Publisher.Publish(0, new WorldDisposedMessage(WorldId));

                lock (_lockObject)
                {
                    Worlds[WorldId] = null;
                }

                _worldIdDispenser.ReleaseInt(WorldId);

                GC.SuppressFinalize(this);
            }
        }




        public override string ToString() => $"World {WorldId}";

    }
}
