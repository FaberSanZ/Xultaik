using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xultaik.Desktop;
using Xultaik.Graphics;

namespace Games
{
    public class FPSTest :IDisposable
    {
        public Window Window { get; set; }

        public RenderDescriptor Parameters { get; set; }

        public GraphicsAdapter Adapter { get; set; }

        public GraphicsDevice Device { get; set; }

        public SwapChain SwapChain { get; set; }

        public CommandList CommandList { get; set; }





        public FPSTest()
        {
            Window = new Window(WindowSettings.Default);



            Parameters = new RenderDescriptor()
            {
                BackBufferWidth = Window.ClientSize.Width,
                BackBufferHeight = Window.ClientSize.Height,
                DeviceHandle = Window.WindowHandler.Value,
                Settings = new Settings()
                {
                    Fullscreen = false,
                    VSync = false,
                    AllowTearing = true,
                },
            };
        }

        public void Initialize()
        {

            Adapter = new GraphicsAdapter();

            Device = new GraphicsDevice(Adapter, Parameters);

            SwapChain = new SwapChain(Device);

            CommandList = new CommandList(Device);
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

            //CommandList.SetViewport(0, 0, 800, 600);
            //CommandList.SetScissor(0, 0, 800, 600);
            CommandList.ClearTargetColor(SwapChain, 0.0f, 0.2f, 0.4f, 1.0f);

            CommandList.ExecuteCommandList();

            SwapChain.Present();

        }

        public void Dispose()
        {
            //Device.Dispose();
        }
    }
}
