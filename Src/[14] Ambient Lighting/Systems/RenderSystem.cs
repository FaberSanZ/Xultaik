using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = Graphics.Buffer;
using CommandList = Graphics.CommandList;
using SamplerState = Graphics.SamplerState;

namespace Systems
{
    public class RenderSystem
    {
        private PresentationParameters parameter;

        public List<GraphicsAdapter> Adapters { get; set; }

        public GraphicsDevice Device { get; set; }

        public GraphicsSwapChain SwapChain { get; set; }

        public CommandList CommandList { get; set; }

        public Texture Texture { get; set; }


        public Mesh  Mesh { get; set; }
        public Grid  Grid { get; set; }

        public Shaders  Shaders { get; set; }

        public PipelineState StateSolid { get; set; }

        public SamplerState  SamplerState { get; set; }

        public Camera Camera { get; set; }

        public RenderItem RGrid { get; set; }

        public RenderItem RMesh { get; set; }

        Input.Input Input { get; set; }


        public RenderSystem(PresentationParameters parameters)
        {
            parameter = parameters;

            Adapters = GraphicsAdapter.EnumerateGraphicsAdapter();

            Device = new GraphicsDevice(Adapters[0]);

            SwapChain = new GraphicsSwapChain(parameters, Device);

            CommandList = new CommandList(Device);

            Texture = new Texture(Device, SwapChain);

            Shaders = new Shaders(Device, "Shaders/VertexShader.hlsl", "Shaders/PixelShader.hlsl");

            StateSolid = new PipelineState(Device, FillMode.Solid, CullMode.None);

            Mesh = new Mesh("Models/dragon.obj");

            TextureAddressMode Wrap = TextureAddressMode.Wrap;

            SamplerState = new SamplerState(Device, Wrap, Wrap, Wrap, Filter.MinMagMipLinear);

            Camera = new Camera();

            Camera.Position = new Vector3(0, 5, -33);

            Camera.SetLens((float)Math.PI / 4, 1.2f, 1.0f, 1000.0f);

            Grid = new Grid(30, 30, 2, 2);

            System.Drawing.Point LastMousePos = new System.Drawing.Point();

            parameter.Con.MouseDown += (o, e) =>
            {
                LastMousePos = e.Location;
            };

            parameter.Con.MouseMove += (o, e) =>
            {
                if (e.Button == MouseButtons.Middle)
                {
                    Camera.Pitch((float)Math.PI * (0.25f * (e.Y - LastMousePos.Y)) / 180.0f);
                    Camera.Yaw((float)Math.PI * (0.25f * (e.X - LastMousePos.X)) / 180.0f);

                    LastMousePos = e.Location;
                }
            };

            BuildRenderItems();

            Input = new Input.Input();

            Input.Initialize(parameter.Con);
        }




        private void BuildRenderItems()
        {
            RGrid = new RenderItem()
            {
                // Reset World
                Worlds = new Matrix[1],

                Light = new LightBuffer()
                {
                    AmbientColor = new Vector4(0.15f, 0.15f, 0.15f, 1.0f),

                    Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                    LightDirection = new Vector3(0, -1, 0),
                },

                VertexBuffer = new Buffer(Grid.Vertices.SizeInBytes, Grid.Vertices.Size, Device, ResourceInfo.VertexBuffer),

                IndexBuffer = new Buffer(Grid.Indices.SizeInBytes, Grid.Indices.Size, Device, ResourceInfo.IndexBuffer),

                ConstantBuffer = new Buffer[]
                {
                    new Buffer(Utilities.SizeOf<Transform>(), Utilities.SizeOf<Transform>(), Device, ResourceInfo.ConstantBuffer),
                    new Buffer(Utilities.SizeOf<LightBuffer>(), Utilities.SizeOf<LightBuffer>(), Device, ResourceInfo.ConstantBuffer)
                },

                Texture = Texture.LoadFromFile(Device, "Text/test.bmp")
            };





            RMesh = new RenderItem()
            {
                // Reset World
                Worlds = new Matrix[20],

                Light = new LightBuffer()
                {
                    AmbientColor = new Vector4(0.18f, 0.18f, 0.18f, 1.0f),
                    Diffuse = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                    LightDirection = new Vector3(0, 0, -1.05f),
                },

                VertexBuffer = new Buffer(Mesh.Vertices.SizeInBytes, Mesh.Vertices.Size, Device, ResourceInfo.VertexBuffer),

                IndexBuffer = new Buffer(Mesh.Indices.SizeInBytes, Mesh.Indices.Size, Device, ResourceInfo.IndexBuffer),

                ConstantBuffer = new Buffer[]
                {
                    new Buffer(Utilities.SizeOf<Transform>(), Utilities.SizeOf<Transform>(), Device, ResourceInfo.ConstantBuffer),
                    new Buffer(Utilities.SizeOf<LightBuffer>(), Utilities.SizeOf<LightBuffer>(), Device, ResourceInfo.ConstantBuffer)
                },

                Texture = Texture.LoadFromFile(Device, "Text/UVCheckerMap08-512.png")
            };
        }

        float r = 0.0f;

        public void Update()
        {
            if (Input.IsKeyDown(Keys.W))
                Camera.Walk(10.0f * 0.022F);

            if (Input.IsKeyDown(Keys.S))
                Camera.Walk(-10.0f * 0.022F);

            if (Input.IsKeyDown(Keys.A))
                Camera.Strafe(-10.0f * 0.022F);

            if (Input.IsKeyDown(Keys.D))
                Camera.Strafe(10.0f * 0.022F);

            r += 0.02f;


            RMesh.Worlds[0] = Matrix.RotationYawPitchRoll(r, 0.0f, 0.0f) * Matrix.Translation(0, 1.0f, 0);

            RGrid.Worlds[0] = Matrix.RotationYawPitchRoll(3.14f, 0.0f, 0.0f) * Matrix.Translation(0.0f, -0.5f, 0.0f);


            Camera.UpdateViewMatrix();
        }



        public void Draw()
        {
            Device.Reset();


            CommandList.ClearDepthStencilView(Texture, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil);

            CommandList.SetRenderTargets(Texture);

            //Clear our backbuffer to the updated color
            CommandList.Clear(Texture, new Color4(0.2f, 0.4f, 0.8f, 1.0f));

            CommandList.SetInputLayout(Shaders.Layout);

            CommandList.SetViewPort(SwapChain.PresentParameters.Width, SwapChain.PresentParameters.Height, 0, 0);

            CommandList.SetVertexShader(Shaders.VertexShader);

            CommandList.SetPixelShader(Shaders.PixelShader);



            for (int i = 0; i < RMesh.Worlds.Count(); i++)
            {
                RMesh.VertexBuffer.Update<Vertex>(Mesh.Vertices.Data.ToArray());
                RMesh.IndexBuffer.Update<int>(Mesh.Indices.Data.ToArray());
                RMesh.ConstantBuffer[0].UpdateConstant<Transform>(ShaderType.VertexShader, 0, new Transform(RMesh.Worlds[i], Camera.View, Camera.Proj)); // cbuffer MatrixBuffer (W V P) : register(b0)
                RMesh.ConstantBuffer[1].UpdateConstant<LightBuffer>(ShaderType.PixelShader, 0, RGrid.Light);
                CommandList.SetVertexBuffer(RMesh.VertexBuffer);
                CommandList.SetIndexBuffer(RMesh.IndexBuffer);
                CommandList.SetPrimitiveType(SharpDX.Direct3D.PrimitiveTopology.TriangleList);
                CommandList.SetRasterizerState(StateSolid);
                CommandList.SetSampler(ShaderType.PixelShader, SamplerState, 0);
                CommandList.SetShaderResource(ShaderType.PixelShader, RMesh.Texture, 0);
                CommandList.DrawIndexed(Mesh.Indices.Count);
            }





            RGrid.VertexBuffer.Update<Vertex>(Grid.Vertices.Data.ToArray());
            RGrid.IndexBuffer.Update<int>(Grid.Indices.Data.ToArray());
            RGrid.ConstantBuffer[0].UpdateConstant<Transform>(ShaderType.VertexShader, 0, new Transform(RGrid.Worlds[0], Camera.View, Camera.Proj)); // cbuffer MatrixBuffer (W V P) : register(b0)
            RGrid.ConstantBuffer[1].UpdateConstant<LightBuffer>(ShaderType.PixelShader, 0, RGrid.Light);
            CommandList.SetVertexBuffer(RGrid.VertexBuffer);
            CommandList.SetIndexBuffer(RGrid.IndexBuffer);
            CommandList.SetPrimitiveType(SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            CommandList.SetRasterizerState(StateSolid);
            CommandList.SetSampler(ShaderType.PixelShader, SamplerState, 0);
            CommandList.SetShaderResource(ShaderType.PixelShader, RGrid.Texture, 0);
            CommandList.DrawIndexed(Grid.Indices.Count);

        }



        public void End()
        {
            //Present the backbuffer to the screen
            SwapChain.Present(true);
        }
    }
}
