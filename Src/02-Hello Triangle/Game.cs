// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Game.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Graphics;
using Zeckoxe.Image;
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
            public readonly Vector4 Color;

            public Vertex(in Vector3 position,in Vector4 color)
            {
                Position = position;
                Color = color;
            }
        };


        public Window Window { get; set; }
        public RenderDescriptor Parameters { get; set; }
        public GraphicsAdapter Adapter { get; set; }
        public GraphicsDevice Device { get; set; }
        public SwapChain SwapChain { get; set; }
        public CommandList CommandList { get; set; }


        // New
        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public PipelineState PipelineState { get; set; }




        public Game()
        {
            Window = new Window("Zeckoxe Engine - (Hello Triangle)", 1000, 720)
            {
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
            };


            Parameters = new RenderDescriptor()
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


            TestIMGLoaders();
        }

        public void Initialize()
        {

            Adapter = new GraphicsAdapter();

            Device = new GraphicsDevice(Adapter, Parameters);

            SwapChain = new SwapChain(Device);

            CreatePSO();

            CommandList = new CommandList(Device);


            Vertex[] Vertices = new Vertex[]
            {
                  new Vertex(new Vector3(0f, 0.65f, 1.0f), new Vector4(1.8f, 0.0f, 0.0f, 1.0f)),
                  new Vertex(new Vector3(0.5f, -0.65f, 1.0f), new Vector4(0.0f, 1.8f, 0.0f, 1.0f)),
                  new Vertex(new Vector3(-0.5f, -0.65f, 1.0f), new Vector4(0.0f, 0.0f, 1.8f, 1.0f))
            };


            int[] indices = new int[]
            {
                2, 1, 0,
            };



            VertexBuffer = new Buffer(Device, new BufferDescription()
            {
                Flags = BufferFlags.VertexBuffer,
                HeapType = HeapType.Upload,
                SizeInBytes = Interop.SizeOf(Vertices),
                StructureByteStride = Interop.SizeOf<Vertex>(),
            });


            IndexBuffer = new Buffer(Device, new BufferDescription()
            {
                Flags = BufferFlags.IndexBuffer,
                HeapType = HeapType.Upload,
                SizeInBytes = Interop.SizeOf(indices),
                StructureByteStride = Interop.SizeOf<int>(),
            });

            IndexBuffer.SetData(indices);

            VertexBuffer.SetData(Vertices);
        }


        public void CreatePSO()
        {
            PipelineStateDescription pipelineStateDescription = new PipelineStateDescription()
            {
                VertexShader =  ShaderByteCode.CompileFromFile("shaders.hlsl", ShaderStage.VertexShader),                                // Compile #1
                PixelShader = new ShaderByteCode(File.ReadAllText("shaders.hlsl"), ShaderStage.PixelShader, "PS", ShaderModel.Model6_0), // Compile #2
            };

            PipelineState = new PipelineState(Device, pipelineStateDescription);
        }


  

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
            foreach (string Description in Device.NativeAdapter.Description)
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

            CommandList.SetViewport(0, 0, Parameters.BackBufferWidth, Parameters.BackBufferHeight);
            CommandList.SetScissor(0, 0, Parameters.BackBufferWidth, Parameters.BackBufferHeight);

            CommandList.SetPipelineState(PipelineState);
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





        public void TestIMGLoaders()
        {
            Console.WriteLine("-----------------TextureData-----------");

            //Zeckoxe.Image.DDSLoader dDSLoader = new Zeckoxe.Image.DDSLoader("desertcube1024.dds");
            //var data = dDSLoader.TextureData;


            TextureData data = IMGLoader.LoadFromFile("UVCheckerMap08-512.png");
            //TextureData data = DDSLoader.LoadFromFile("desertcube1024.dds");

            Console.WriteLine("Format = {0}", data.Format);
            Console.WriteLine("Width = {0}", data.Width);
            Console.WriteLine("Height = {0}", data.Height);
            Console.WriteLine("Depth = {0}", data.Depth);
            Console.WriteLine("MipMaps = {0}", data.MipMaps);
            Console.WriteLine("Size = {0}", data.Size);
            Console.WriteLine("IsCubeMap = {0}", data.IsCubeMap);
            Console.WriteLine("-------------DataInBytes-----------");

            int i = 0;
            foreach (byte item in data.Data)
            {
                //Console.Write("-{0}", item);
                if (i is 10)
                {
                    //Console.WriteLine();
                    i = 0;
                }

                i++;
            }
            Console.WriteLine("-----------------DataInBytes-----------");
        }

    }
}
