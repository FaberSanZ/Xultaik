// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Game.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Vortice.DirectX;
using Vortice.Mathematics;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;
using Zeckoxe.Mathematics;
using Buffer = Zeckoxe.Graphics.Buffer;
//using Zeckoxe.Mathematics;

namespace _02_Hello_Triangle
{
    public class Game : IDisposable
    {
        struct Vertex
        {
            public readonly Vector3 Position;
            public readonly Color4 Color;

            public Vertex(Vector3 position, Color4 color)
            {
                Position = position;
                Color = color;
            }
        };


        public Window Window { get; set; }
        public PresentationParameters Parameters { get; set; }
        public GraphicsAdapter Adapter { get; set; }
        public GraphicsDevice Device { get; set; }
        public SwapChain SwapChain { get; set; }
        public CommandList CommandList { get; set; }


        // New
        public Buffer VertexBuffer;
        public Buffer IndexBuffer;

        PipelineState PipelineState;


        public Game()
        {
            Window = new Window("Zeckoxe Engine - (Hello Triangle)", 1000, 720)
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
            };


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                DeviceHandle = Window.Handle,
                Settings = new Settings()
                {
                    Fullscreen = false,
                    VSync = false,
                },
            };
        }

        public void Initialize()
        {

            Adapter = new GraphicsAdapter();

            Device = new GraphicsDevice(Adapter, Parameters);

            SwapChain = new SwapChain(Device);

            PipelineState = new PipelineState(Device);

            CommandList = new CommandList(Device);


            Vertex[] Vertices = new Vertex[]
            {
                  new Vertex(new Vector3(0f, 0.65f, 1.0f), new Color4(1.0f, 0.0f, 0.0f, 1.0f)),
                  new Vertex(new Vector3(0.5f, -0.65f, 1.0f), new Color4(0.0f, 1.0f, 0.0f, 1.0f)),
                  new Vertex(new Vector3(-0.5f, -0.65f, 1.0f), new Color4(0.0f, 0.0f, 1.0f, 1.0f))
            };


            int[] indices = new int[]
            {
                2, 1, 0
            };



            VertexBuffer = new Buffer(Device, new BufferDescription()
            {
                Flags = BufferFlags.VertexBuffer,
                HeapType = HeapType.Upload,
                SizeInBytes = SizeOf(Vertices),
                StructureByteStride = Unsafe.SizeOf<Vertex>(),
            });



            IndexBuffer = new Buffer(Device, new BufferDescription()
            {
                Flags = BufferFlags.VertexBuffer,
                HeapType = HeapType.Upload,
                SizeInBytes = SizeOf(indices),
                StructureByteStride = Unsafe.SizeOf<int>(),
            });

            IndexBuffer.SetData(indices);

            VertexBuffer.SetData(Vertices);
        }


        public int SizeOf<T>(T[] data) where T : struct => data.Length * Unsafe.SizeOf<T>();
        


        public void Run()
        {
            Initialize();

            BeginRun();

            Window?.Show();

            Tick();
        }

        public void Tick()
        {

            Window.RenderLoop(() =>
            {
                Update();
                Draw();
            });
        }



        public void BeginRun()
        {
            foreach (var Description in Device.NativeAdapter.Description)
                Console.WriteLine(Description);

            foreach (var VendorId in Device.NativeAdapter.VendorId)
                Console.WriteLine(VendorId);

        }

        public void Update()
        {

        }

        public void Draw()
        {
            CommandList.Reset();


            CommandList.ClearTargetColor(SwapChain.BackBuffer, 0.0f, 0.2f, 0.4f, 1.0f);


            CommandList.SetPipelineState(PipelineState);
            CommandList.SetViewport(0, 0, Parameters.BackBufferWidth, Parameters.BackBufferHeight);
            CommandList.SetScissor(0, 0, Parameters.BackBufferWidth, Parameters.BackBufferHeight);
            CommandList.SetVertexBuffer(VertexBuffer);
            CommandList.SetIndexBuffer(IndexBuffer);
            CommandList.DrawIndexed(3);
            CommandList.EndDraw();
            CommandList.FinishFrame();
            SwapChain.Present();
            CommandList.Wait();
        }

        public void Dispose()
        {
            //Device.Dispose();
        }

    }
}
