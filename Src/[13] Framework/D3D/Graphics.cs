using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = System.Drawing.Point;


namespace _13__Framework
{
    public class Graphics
    {
        public D3D11 DX11 { get; set; }
        public Camera Camera { get; set; }

        public Light[] Light { get; set; }

        public LoadModel dragon { get; set; }
        public LoadModel dragon2 { get; set; }
        private Shaders ColorShader { get; set; }
        public Input Input { get; private set; }
        public float R { get; private set; }

        public Point LastMousePos = new Point();



        public void Initialize(Control Con)
        {
            R = 0.0f;
            // Create the Direct3D object.
            DX11 = new D3D11();

            // Initialize the Direct3D object.
            DX11.Initialize(Con);


            Input = new Input();
            Input.Initialize(Con);

            // Create the camera object
            Camera = new Camera();

            // Set the initial position of the camera.
            Camera.Position = new Vector3(0, 0, -7);
            Camera.SetLens(0.25f * Mathf.PI, 1.2f, 1.0f, 1000.0f);


            // Initialize the model object.
            dragon = new LoadModel();
            dragon.InitializeBuffer(DX11.Device, @"gltf/BrainStem.glb",  TFileName: "gltf/Default_AO.jpg");

            // Initialize the model object.
            dragon2 = new LoadModel();
            //dragon2.InitializeBuffer(DX11.Device, @"Models/dragon.obj", TFileName: "Textures/Color/Rojo.bmp");

            // Create the color shader object.
            ColorShader = new Shaders();

            // Initialize the color shader object.
            ColorShader.Initialize(DX11.Device);


            Light = new Light[1];
            Light[0].DiffuseColour = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
            Light[0].Direction = new Vector3(0, 0, 1.05f);
        }
        
        public void Update(Control Form)
        {
            Form.MouseMove += (o, e) =>
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

            if (Input.IsKeyDown(Keys.W))
            {
                Camera.Walk(10.0f * 0.022F);
            }

            if (Input.IsKeyDown(Keys.S))
            {
                Camera.Walk(-10.0f * 0.022F);
            }

            if (Input.IsKeyDown(Keys.A))
            {
                Camera.Strafe(-10.0f * 0.022F);
            }

            if (Input.IsKeyDown(Keys.D))
            {
                Camera.Strafe(10.0f * 0.022F);
            }

            if (Input.IsKeyDown(Keys.R))
            {
                R = R + 0.05f;
            }

            // The form must be showing in order for the handle to be used in Input and Graphics objects.
            Form.MouseUp += (o, e) => 
            {
                Form.Capture = true;
            };

            Form.MouseDown += (o, e) => 
            {
                Form.Capture = false;
                LastMousePos = e.Location;
            };
        }

        public void Frame(Control con)
        {

            // Clear the buffer to begin the scene.
            DX11.BeginScene(0.0f, 0.2f, 0.4f, 1.0f);

            // Generate the view matrix based on the camera position.
            Camera.UpdateViewMatrix();

            Matrix viewMatrix = Camera.View;
            Matrix worldMatrix = Camera.ViewProj;
            Matrix projectionMatrix = Camera.Proj;
            Matrix translationMatrix;



            //---Dragon #1
            Matrix.RotationYawPitchRoll(R, 0.0f, 0.0f, out worldMatrix);
            Matrix.Translation(-0.0f, 0.0f, 0.0f, out translationMatrix);
            worldMatrix = worldMatrix * translationMatrix;
            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            dragon.RenderBuffers(DX11.DeviceContext);
            // Render the model using the color shader.
            ColorShader.SetShaderParameters(DX11.DeviceContext, worldMatrix, viewMatrix, projectionMatrix, dragon.Texture);
            ColorShader.RenderShader(DX11.DeviceContext, dragon.IndexCount);


            ////---Dragon #2
            //Matrix.RotationYawPitchRoll(-R, 0.0f, 0.0f, out worldMatrix);
            //Matrix.Translation(8.0f, 0.5f, 0.0f, out translationMatrix);
            //worldMatrix = worldMatrix * translationMatrix;
            //// Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            //dragon.RenderBuffers(DX11.DeviceContext);
            //// Render the model using the color shader.
            //ColorShader.SetShaderParameters(DX11.DeviceContext, worldMatrix, viewMatrix, projectionMatrix, dragon2.Texture);
            //ColorShader.RenderShader(DX11.DeviceContext, dragon.IndexCount);



            ColorShader.CreateLight(DX11.DeviceContext, Light[0].Direction, Light[0].DiffuseColour);

            // Present the rendered scene to the screen.
            DX11.EndScene();

            R = R + 0.015f;
        }

        public void ShutDown()
        {
            if (DX11 != null)
            {
                DX11.ShutDown();
                DX11 = null;
            }
        }

        public static float ToRadians(float degrees)
        {
            return (float)Math.PI * degrees / 180.0f;
        }

    }
}
