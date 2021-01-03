// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Game.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Zeckoxe.Core;
using Zeckoxe.Desktop;
using Zeckoxe.Engine;
using Zeckoxe.Graphics;

namespace Zeckoxe.Engine
{
    public class Application : ApplicationBase
    {
        public Application() : base()
        {
        }


        public PresentationParameters Parameters { get; set; }

        public Adapter Adapter { get; set; }

        public Device? Device { get; set; }

        public Framebuffer Framebuffer { get; set; }

        public SwapChain SwapChain { get; set; }


        public GraphicsContext Context { get; set; }


        public Matrix4x4 Model { get; set; }
        public List<Matrix4x4> Models { get; set; } = new();

        public Window? Window { get; set; }

        public Input? Input => Window.Input;


        public string EngineVersion => Adapter.EngineVersion.ToString();


        public override void InitializeSettings()
        {
            base.InitializeSettings();

            Window = new Window("Zeckoxe Engine", 1200, 800)
            {
                //StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
            };


            Parameters = new PresentationParameters()
            {
                BackBufferWidth = Window.Width,
                BackBufferHeight = Window.Height,
                //SwapchainSource = Window.GetSwapchainSource(Adapter),
                Settings = new Settings()
                {
                    Validation = ValidationType.None,
                    Fullscreen = false,
                    VSync = false,
                },
            };

        }


        public override void InitializeVulkan()
        {
            base.InitializeVulkan();


            Adapter = new Adapter(Parameters);

            Device = new Device(Adapter);

            SwapChain = new SwapChain(Device, GetSwapchainSource());

            Framebuffer = new Framebuffer(SwapChain);

            Context = new GraphicsContext(Device);
        }



        public SwapchainSource GetSwapchainSource()
        {
            if (Adapter.SupportsSurface)
            {
                if (Adapter.SupportsWin32Surface)
                {
                    return Window.SwapchainWin32;
                }

                if (Adapter.SupportsX11Surface)
                {
                    return Window.SwapchainX11;
                }

                if (Adapter.SupportsWaylandSurface)
                {
                    return Window.SwapchainWayland;
                }

                if (Adapter.SupportsMacOSSurface)
                {
                    return Window.SwapchainNS;
                }
            }

            throw new PlatformNotSupportedException("Cannot create a SwapchainSource.");
        }


        public override void Initialize()
        {
            base.Initialize();

        }

        public override void BeginRun()
        {
            base.BeginRun();

            Window?.Show();


            Window.RenderLoop(OnTickRequested);
        }

        public override void BeginDraw()
        {
            Device.WaitIdle();
            Context.CommandBuffer.Begin(SwapChain);


            base.BeginDraw();
        }
        public override void Update(ApplicationTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void EndDraw()
        {
            base.EndDraw();


            Context.CommandBuffer.Close();
            Context.CommandBuffer.Submit();
            SwapChain.Present();
        }

        public override void EndRun()
        {
            base.EndRun();

            if (Window != null)
            {
                //Window.Dispose();
            }
        }

        public override void ConfigureServices()
        {

        }

        public void OnTickRequested()
        {
            Tick();
        }
    }
}
