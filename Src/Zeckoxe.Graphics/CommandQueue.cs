// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	CommandQueue.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;

namespace Zeckoxe.Graphics
{
    public class CommandQueue : GraphicsResource
    {
        internal ID3D12CommandQueue Queue { get; private set; }

        public CommandListType Type { get; set; }

        public CommandQueue(GraphicsDevice device, CommandListType type) : base(device)
        {
            Type = type;

            Recreate();
        }



        public CommandQueue(GraphicsDevice device) : base(device)
        {
            Recreate();
        }


        public void Recreate()
        {
            switch (Type)
            {
                case CommandListType.Direct:
                    Queue = CreateCommandQueueDirect();
                    break;

                case CommandListType.Bundle:
                    Queue = CreateCommandQueueBundle();
                    break;

                case CommandListType.Compute:
                    Queue = CreateCommandQueueCompute();
                    break;

                case CommandListType.Copy:
                    Queue = CreateCommandQueueCopy();
                    break;

                case CommandListType.VideoDecode:
                    Queue = CreateCommandQueueVideoDecode();
                    break;

                case CommandListType.VideoProcess:
                    Queue = CreateCommandQueueVideoProcess();
                    break;

                case CommandListType.VideoEncode:
                    Queue = CreateCommandQueueVideoEncode();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));

            }
        }

        public void Wait(/*Fence fence*/)
        {
            Queue.Wait(new ID3D12Fence(IntPtr.Zero), 20);
        }



        internal ID3D12CommandQueue CreateCommandQueueDirect()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.Direct,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }


        internal ID3D12CommandQueue CreateCommandQueueCopy()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.Copy,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }


        internal ID3D12CommandQueue CreateCommandQueueBundle()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.Bundle,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }


        internal ID3D12CommandQueue CreateCommandQueueCompute()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.Compute,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }


        internal ID3D12CommandQueue CreateCommandQueueVideoDecode()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.VideoDecode,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }

        internal ID3D12CommandQueue CreateCommandQueueVideoEncode()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.VideoEncode,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }


        internal ID3D12CommandQueue CreateCommandQueueVideoProcess()
        {
            // Describe and create the command queue.
            CommandQueueDescription QueueDesc = new CommandQueueDescription()
            {
                Type = Vortice.Direct3D12.CommandListType.VideoProcess,
                Flags = CommandQueueFlags.None,
            };

            return GraphicsDevice.NativeDevice.CreateCommandQueue(QueueDesc);
        }
    }

}
