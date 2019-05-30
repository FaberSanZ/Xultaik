using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<GraphicsAdapter> Adapters { get; set; }

        public GraphicsDevice Device { get; set; }

        public GraphicsSwapChain SwapChain { get; set; }

        public CommandList CommandList { get; set; }

        public Texture Texture { get; set; }

        public Buffer[] VertexBuffer { get; set; }

        public Buffer[] IndexBuffer { get; set; }

        public Grid  Grid { get; set; }

        public Mesh  Mesh { get; set; }

        public Shaders  Shaders { get; set; }

        public PipelineState StateWireframe { get; set; }

        public PipelineState StateSolid { get; set; }

        public SamplerState  SamplerState { get; set; }

        public Camera Camera { get; set; }

        public Buffer[] ConstantBuffer { get; set; }

        public Matrix Rotation { get; set; }

        public Matrix Translation { get; set; }

        public Matrix[] World { get; set; }

        public float R { get; set; } = 0.0f;

        public ShaderResourceView[] Textures;



        public RenderSystem(PresentationParameters parameters)
        {
            Adapters = GraphicsAdapter.EnumerateGraphicsAdapter();

            Device = new GraphicsDevice(Adapters[0]);

            SwapChain = new GraphicsSwapChain(parameters, Device);

            CommandList = new CommandList(Device);

            Texture = new Texture(Device, SwapChain);

            Shaders = new Shaders(Device, "Shaders/VertexShader.hlsl", "Shaders/PixelShader.hlsl");

            StateWireframe = new PipelineState(Device, FillMode.Wireframe, CullMode.None);

            StateSolid = new PipelineState(Device, FillMode.Solid, CullMode.None);

            Grid = new Grid(1, 1, 8, 8);

            Mesh = new Mesh("dragon.obj");

            VertexBuffer = new Buffer[2];
            IndexBuffer = new Buffer[2];

            VertexBuffer[0] = new Buffer(Grid.SizeInBytes, Grid.Size, Device, ResourceInfo.VertexBuffer);
            IndexBuffer[0] = new Buffer(Grid.IndexSizeInBytes, Grid.IndexSize, Device, ResourceInfo.IndexBuffer);

            VertexBuffer[1] = new Buffer(Mesh.SizeInBytes, Mesh.Size, Device, ResourceInfo.VertexBuffer);
            IndexBuffer[1] = new Buffer(Mesh.IndexSizeInBytes, Mesh.IndexSize, Device, ResourceInfo.IndexBuffer);

            ConstantBuffer = new Buffer[2];

            ConstantBuffer[0] = new Buffer(Utilities.SizeOf<Transform>(), Utilities.SizeOf<Transform>(), Device, ResourceInfo.ConstantBuffer);
            ConstantBuffer[1] = new Buffer(Utilities.SizeOf<LightBuffer>(), Utilities.SizeOf<LightBuffer>(), Device, ResourceInfo.ConstantBuffer);

            TextureAddressMode Wrap = TextureAddressMode.Wrap;

            SamplerState = new SamplerState(Device, Wrap, Wrap, Wrap, Filter.MinMagMipLinear);

            Camera = new Camera(CameraType.Static);

            Camera.Position = new Vector3(0.0f, 0.15f, -1.5f);

            Camera.SetLens((float)Math.PI / 4, 1.2f, 1.0f, 1000.0f);

            World = new Matrix[2];

            Textures = new ShaderResourceView[2];
            Textures[0] = Texture.LoadFromFile(Device, "Text/UV_Grid_Lrg.jpg");
            Textures[1] = Texture.LoadFromFile(Device, "Text/UV_Grid_Sm.jpg");



        }


        public void Update()
        {
            Camera.Update();

            R += .015f;


            // Reset World[0]
            World[0] = Matrix.Identity;
            // Define world space matrix
            Rotation = Matrix.RotationYawPitchRoll(0, -0.0f, 0.0f);
            Translation = Matrix.Translation(-0.0f, -0.3f, 0.0f);
            // Set world space using the transformations
            World[0] = Rotation * Translation;




            // Reset World[1]
            World[1] = Matrix.Identity;
            // Define world space matrix
            Rotation = Matrix.RotationYawPitchRoll(-R, 0.0f, 0.0f);
            Translation = Matrix.Translation(0.0f, 0.5f, 0.0f);
            // Set world space using the transformations
            World[1] = Rotation * Translation * Matrix.Scaling(.12f);

        }




        public void Draw()
        {
            Device.Reset();

            VertexBuffer[0].Update<Vertex>(Grid.Vertices.ToArray());
            IndexBuffer[0].Update<int>(Grid.Indices.ToArray());

            VertexBuffer[1].Update<Vertex>(Mesh.Vertices.ToArray());
            IndexBuffer[1].Update<int>(Mesh.Indices.ToArray());

            CommandList.ClearDepthStencilView(Texture, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil);

            CommandList.SetRenderTargets(Texture);

            //Clear our backbuffer to the updated color
            CommandList.Clear(Texture, new Color4(0.0f, 0.2f, 0.4f, 1));

            CommandList.SetInputLayout(Shaders.Layout);

            CommandList.SetViewPort(SwapChain.PresentParameters.Width, SwapChain.PresentParameters.Height, 0, 0);

            CommandList.SetVertexShader(Shaders.VertexShader);

            CommandList.SetPixelShader(Shaders.PixelShader);




            //---Draw Grid #2
            CommandList.SetVertexBuffer(VertexBuffer[0]);
            CommandList.SetIndexBuffer(IndexBuffer[0]);
            ConstantBuffer[0].UpdateConstant<Transform>(ShaderType.VertexShader, 0, new Transform(World[0], Camera.View, Camera.Projection)); // cbuffer MatrixBuffer (W V P)   
            ConstantBuffer[1].UpdateConstant<LightBuffer>(ShaderType.PixelShader, 0, new LightBuffer(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), new Vector3(0, 0, 1.05f))); 
            CommandList.SetPrimitiveType(SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            CommandList.SetRasterizerState(StateWireframe);
            CommandList.SetSampler(ShaderType.PixelShader, SamplerState, 0);
            CommandList.SetShaderResource(ShaderType.PixelShader, Textures[0], 0);
            CommandList.DrawIndexed(Grid.IndexCount);




            //---Draw Grid #2
            CommandList.SetVertexBuffer(VertexBuffer[1]);
            CommandList.SetIndexBuffer(IndexBuffer[1]);
            ConstantBuffer[0].UpdateConstant<Transform>(ShaderType.VertexShader, 0, new Transform(World[1], Camera.View, Camera.Projection)); // cbuffer MatrixBuffer (W V P) : register(b0)
            ConstantBuffer[1].UpdateConstant<LightBuffer>(ShaderType.PixelShader, 0, new LightBuffer(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), new Vector3(0, 0, 1.05f)));
            CommandList.SetPrimitiveType(SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            CommandList.SetRasterizerState(StateSolid);
            CommandList.SetSampler(ShaderType.PixelShader, SamplerState, 0);
            CommandList.SetShaderResource(ShaderType.PixelShader, Textures[1], 0);
            CommandList.DrawIndexed(Mesh.IndexCount);

        }



        public void End()
        {
            //Present the backbuffer to the screen
            SwapChain.Present(true);
        }
    }
}
