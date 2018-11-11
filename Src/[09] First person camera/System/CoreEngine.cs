using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _08__Load_model_with_Assimp
{
    class CoreEngine : IDisposable
    {
        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }

        public Input Input { get; set; }
        public Windows Windows { get; set; }
        public DX11 DXE { get; set; }
        public Shader Shader { get; set; }
        public LoadMesh Dragon { get; set; }
        public Camera Camera { get; set; }


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


            if (Dragon == null)
            {
                Dragon = new LoadMesh();
                Dragon.InitializeBuffer(DXE.Device, @"Models/dragon.obj", @"Textures/WoodCrate01.dds");
            }
            

            if (Shader == null)
            {
                Shader = new Shader();
                Shader.Initialize(DXE.Device, @"Shaders/VertexShader.hlsl", @"Shaders/PixelShader.hlsl");
            }


            if (Camera == null)
            {
                Camera = new Camera();
                Camera.Initialize(Windows.Form);

                // Set the initial position of the camera.
                Camera.SetPosition(0, 0, -18);
            }
        }

        public void Run()
        {
            Windows.Run(render: Render, update: Update);
        }

        public void Render()
        {
            RX = RX + 0.8F * 0.0174532925f;
            RY = RY + 0.0F * 0.0174532925f;
            RZ = RZ + 0.0F * 0.0174532925f;

            //Clear color
            DXE.Clear(0.0F, 0.2F, 0.4F);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            Matrix View = Camera.View;
            Matrix World = Camera.World;
            Matrix Projection = Camera.Projection;
            Matrix Translation;


            Matrix.RotationYawPitchRoll(RX, RY, RZ, out World);
            Matrix.Translation(0.0f, -3.5f, 0.0f, out Translation);
            World = World * Translation;
            Dragon.RenderBuffers(DXE.DeviceContext);
            Shader.SetParameters(DXE.DeviceContext, World, View, Projection);
            Shader.Render(DXE.DeviceContext, Dragon.IndexCount);
            Shader.LoadTexture(DXE.DeviceContext, Dragon.Texture);

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
