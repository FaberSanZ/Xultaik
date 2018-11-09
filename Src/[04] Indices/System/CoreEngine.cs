using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _04__Indices
{
    class CoreEngine : IDisposable
    {
        public Input Input { get; set; }
        public Windows Windows { get; set; }
        DX11 DXE { get; set; }
        Shader Shader { get; set; }
        Triangle Triangle { get; set; }


        public CoreEngine() { }

        public void Initialize()
        {
            if (Windows == null)
            {
                // Create the Windows object.
                Windows = new Windows();
                // Initialize the Windows object.
                Windows.Initialize();
            }


            if (Input == null)
            {
                Input = new Input();
                Input.Initialize(Windows.Form);
            }


            if (DXE == null)
            {
                DXE = new DX11();
                DXE.Initialize(Windows.Form);
            }


            if (Triangle == null)
            {
                Triangle = new Triangle();
                Triangle.InitializeBuffer(DXE.Device);
            }


            if (Shader == null)
            {
                Shader = new Shader();
                Shader.Initialize(DXE.Device, @"Shaders/ColorShader.hlsl", @"Shaders/ColorShader.hlsl");
            }
        }

        public void Run()
        {
            Windows.Run(render: Render, update: Update);
        }

        public void Render()
        {
            //Clear color
            DXE.Clear(0.0F, 0.2F, 0.4F);

            Triangle.RenderBuffers(DXE.DeviceContext);

            Shader.Render(DXE.DeviceContext, Triangle.IndexCount);

            //Present
            DXE.Present();
        }

        public void Update()
        {
            if (Input.IsKeyDown(Keys.Escape))
            {
                Windows.AppPaused = true;
                Windows.Running = false;
            }
        }


        public void Dispose()
        {
            if (Windows != null)
            {
                Windows = null;
            }

            if (Input != null)
            {
                Input = null;
            }

            if (DXE != null)
            {
                DXE.Dispose();
                DXE = null;
            }
        }
    }
}
