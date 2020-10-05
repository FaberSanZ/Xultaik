// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	EntityCommandRecorder.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Zeckoxe.EntityComponentSystem.Technical.Command;
using Zeckoxe.EntityComponentSystem.Technical.Helper;

namespace Zeckoxe.EntityComponentSystem.Command
{

    public sealed unsafe class EntityCommandRecorder : IDisposable
    {

        private readonly List<object> _objects;

        private ReaderWriterLockSlim _lockObject;
        private byte[] _memory;
        private int _nextCommandOffset;


        public EntityCommandRecorder(int capacity, int maxCapacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Argument cannot be negative.", nameof(capacity));
            }
            if (maxCapacity < capacity)
            {
                throw new ArgumentException($"{nameof(maxCapacity)} is inferior to {nameof(capacity)}.", nameof(maxCapacity));
            }

            MaxCapacity = maxCapacity;
            _objects = new List<object>();
            _lockObject = maxCapacity == capacity ? null : new ReaderWriterLockSlim();
            _memory = new byte[capacity];
            _nextCommandOffset = 0;
        }


        public EntityCommandRecorder(int maxCapacity)
            : this(maxCapacity, maxCapacity)
        { }


        public EntityCommandRecorder()
            : this(1024, int.MaxValue)
        { }



        public int MaxCapacity { get; }


        public int Capacity => _memory.Length;


        public int Size => _nextCommandOffset;



        [SuppressMessage("Performance", "RCS1242:Do not pass non-read-only struct by read-only reference.")]
        private void WriteCommand<T>(int offset, in T command) where T : unmanaged
        {
            _lockObject?.EnterReadLock();

            try
            {
                fixed (byte* memory = _memory)
                {
                    *(T*)(memory + offset) = command;
                }
            }
            finally
            {
                _lockObject?.ExitReadLock();
            }
        }

        private int ReserveNextCommand(int commandSize)
        {
            static void Throw() => throw new InvalidOperationException("CommandBuffer is full.");

            int commandOffset;
            int nextCommandOffset;

            do
            {
                commandOffset = _nextCommandOffset;
                nextCommandOffset = commandOffset + commandSize;
                if (nextCommandOffset > _memory.Length)
                {
                    if (nextCommandOffset > MaxCapacity)
                    {
                        Throw();
                    }

                    _lockObject?.EnterWriteLock();
                    try
                    {
                        ArrayExtension.EnsureLength(ref _memory, nextCommandOffset, MaxCapacity);
                    }
                    finally
                    {
                        _lockObject?.ExitWriteLock();
                    }
                }
            }
            while (Interlocked.CompareExchange(ref _nextCommandOffset, nextCommandOffset, commandOffset) != commandOffset);

            return commandOffset;
        }

        [SuppressMessage("Performance", "RCS1242:Do not pass non-read-only struct by read-only reference.")]
        internal void WriteCommand<T>(in T command) where T : unmanaged => WriteCommand(ReserveNextCommand(sizeof(T)), command);

        [SuppressMessage("Performance", "RCS1242:Do not pass non-read-only struct by read-only reference.")]
        internal void WriteSetCommand<T>(int entityOffset, in T component)
        {
            int offset = ReserveNextCommand(sizeof(EntityOffsetComponentCommand) + ComponentCommands.ComponentCommand<T>.SizeOfT);

            _lockObject?.EnterReadLock();
            try
            {
                fixed (byte* memory = _memory)
                {
                    *(EntityOffsetComponentCommand*)(memory + offset) = new EntityOffsetComponentCommand(CommandType.Set, ComponentCommands.ComponentCommand<T>.Index, entityOffset);
                    ComponentCommands.ComponentCommand<T>.WriteComponent(_objects, memory + offset + sizeof(EntityOffsetComponentCommand), component);
                }
            }
            finally
            {
                _lockObject?.ExitReadLock();
            }
        }


        public EntityRecord Record(in Entity entity)
        {
            int offset = ReserveNextCommand(sizeof(EntityCommand));

            WriteCommand(offset, new EntityCommand(CommandType.Entity, entity));

            return new EntityRecord(this, offset + sizeof(CommandType));
        }


        public EntityRecord CreateEntity()
        {
            int offset = ReserveNextCommand(sizeof(EntityCommand));

            WriteCommand(offset, new EntityCommand(CommandType.CreateEntity, default));

            return new EntityRecord(this, offset + sizeof(CommandType));
        }


        public void Execute(World world)
        {
            Executer.Execute(_memory, _nextCommandOffset, _objects, world);

            Clear();
        }


        public void Clear()
        {
            _nextCommandOffset = 0;
            _objects.Clear();

            if (_lockObject != null && MaxCapacity == _memory.Length)
            {
                _lockObject.Dispose();
                _lockObject = null;
            }
        }


        public void Dispose()
        {
            _lockObject?.Dispose();
        }

    }
}
