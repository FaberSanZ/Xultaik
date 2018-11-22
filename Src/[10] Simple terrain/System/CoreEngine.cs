using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = System.Drawing.Point;


namespace _10__Simple_terrain
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
        public Terrain terrain { get; set; }
        public CameraFP Camera { get; set; }
        public Point LastMousePos = new Point();


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

            if (Camera == null)
            {

                // Create the camera object
                Camera = new CameraFP();

                // Set the initial position of the camera.
                Camera.Position = new Vector3(0, 0, -18);
                Camera.SetLens(0.25f * (float)Math.PI, 1.2f, 1.0f, 1000.0f);

            }


            if (terrain == null)
            {
                terrain = new Terrain();
                terrain.InitializeBuffers(DXE.Device, @"Textures/texture2.bmpp", 50, 50, 10, 10);
            }


            if (Shader == null)
            {
                Shader = new Shader();
                Shader.Initialize(DXE.Device, @"Shaders/VertexShader.hlsl", @"Shaders/PixelShader.hlsl");
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
            Camera.UpdateViewMatrix();

            // Get the world, view, and projection matrices from camera and d3d objects.
            Matrix View = Camera.View;
            Matrix World = Camera.ViewProj;
            Matrix Projection = Camera.Proj;
            Matrix Translation;




            Matrix.RotationYawPitchRoll(0.0f, 0.0f, 0.0f, out World);
            Matrix.Translation(0.0f, -3.5f, 0.0f, out Translation);
            Matrix.Multiply(ref World, ref Translation, out World);
            terrain.Render(DXE.DeviceContext);
            Shader.SetParameters(DXE.DeviceContext, World, View, Projection);
            Shader.Render(DXE.DeviceContext, terrain.IndexCount);
            Shader.LoadTexture(DXE.DeviceContext, terrain.Texture);


            //Present
            DXE.Present();
        }

        public void Update()
        {

            // Check if the user pressed escape and wants to exit the application.
            if (Input.IsKeyDown(Keys.W))
            {
                Camera.Walk(10.0f * 0.09F);
            }

            if (Input.IsKeyDown(Keys.S))
            {
                Camera.Walk(-10.0f * 0.09F);
            }

            if (Input.IsKeyDown(Keys.A))
            {
                Camera.Strafe(-10.0f * 0.09F);
            }

            if (Input.IsKeyDown(Keys.D))
            {
                Camera.Strafe(10.0f * 0.09F);
            }

            // The form must be showing in order for the handle to be used in Input and Graphics objects.
            Windows.Form.MouseUp += (o, e) =>
            {
                Windows.Form.Capture = true;
            };

            Windows.Form.MouseDown += (o, e) =>
            {
                Windows.Form.Capture = false;
                LastMousePos = e.Location;
            };

            Windows.Form.MouseMove += (o, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    float X = ToRadians(0.25f * (e.X - LastMousePos.X));
                    float Y = ToRadians(0.25f * (e.Y - LastMousePos.Y));

                    Camera.Pitch(Y);
                    Camera.Yaw(X);
                    LastMousePos = e.Location;
                }
            };


            if (Input.IsKeyDown(Keys.Escape))
            {
                Windows.AppPaused = true;
                Windows.Running = false;
            }
        }

        public static float ToRadians(float degrees)
        {
            return (float)Math.PI * degrees / 180.0f;
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
