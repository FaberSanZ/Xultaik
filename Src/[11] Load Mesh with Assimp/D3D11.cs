using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

using Assimp;
using System.IO;

namespace _11__Load_Mesh_with_Assimp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector2 TexC { get; set; }

        public Vertex(Vector3 pos, Vector2 uv)
            : this()
        {
            Position = pos;
            TexC = uv;
        }
    }

    //Create effects constant buffer's structure//
    [StructLayout(LayoutKind.Sequential)]
    public struct MatrixBuffer
    {
        public Matrix W;
        public Matrix V;
        public Matrix P;
    }

    public class D3D11
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }


        public Device Device { get; set; }

        public DeviceContext DeviceContext { get; set; }

        public SwapChain SwapChain { get; set; }

        public RenderTargetView RenderTargetView { get; set; }

        public InputLayout Layout { get; set; }

        public Buffer VertexBuffer { get; set; }

        public Buffer IndexBuffer { get; set; }

        public int VertexCount { get; set; }

        public int IndexCount { get; set; }

        public Dictionary<string, ShaderBytecode> ShaderByte = new Dictionary<string, ShaderBytecode>();

        public DepthStencilView DepthStencilView { get; set; }

        public Texture2D DepthStencilBuffer { get; set; }
        
        public Buffer ObjectBuffer { get; set; }

        public Matrix WVP;

        public Matrix World;

        public Matrix View;

        public Matrix Projection;

        public Vector3 Position;

        public Vector3 Target;

        public Vector3 Up;

        public MatrixBuffer ObjectCB;

        public Matrix cube1World;

        public Matrix cube2World;

        public Matrix Rotation;

        public Matrix Scale;

        public Matrix Translation;

        public float rot = 0.01f;

        private RasterizerState RasterState { get; set; }



        //----New------
        public ShaderResourceView CubesTexture1;

        public ShaderResourceView CubesTexture2;

        public SamplerState SamplerState { get; set; }




        public D3D11() { }

        public void InitializeD3D11(Control Con)
        {
            //Describe our Buffer
            ModeDescription BackBufferDesc = new ModeDescription();
            BackBufferDesc.Width = Con.ClientSize.Width;
            BackBufferDesc.Height = Con.ClientSize.Height;
            BackBufferDesc.Format = Format.R8G8B8A8_UNorm;
            BackBufferDesc.RefreshRate = new Rational(60, 1);
            BackBufferDesc.ScanlineOrdering = DisplayModeScanlineOrder.Unspecified;
            BackBufferDesc.Scaling = DisplayModeScaling.Unspecified;

            // Initialize the swap chain description.
            SwapChainDescription swapChainDesc = new SwapChainDescription();
            // Set to a single back buffer.
            swapChainDesc.BufferCount = 1;
            // Set the width and height of the back buffer.
            swapChainDesc.ModeDescription = BackBufferDesc;
            // Set the usage of the back buffer.
            swapChainDesc.Usage = Usage.RenderTargetOutput;
            // Set the handle for the window to render to.
            swapChainDesc.OutputHandle = Con.Handle;
            // Turn multisampling off.
            swapChainDesc.SampleDescription = new SampleDescription()
            {
                Count = 1,
                Quality = 0
            };
            // Set to full screen or windowed mode.
            swapChainDesc.IsWindowed = !false;
            // Don't set the advanced flags.
            swapChainDesc.Flags = SwapChainFlags.None;
            // Discard the back buffer content after presenting.
            swapChainDesc.SwapEffect = SwapEffect.Discard;

            // Create the swap chain, Direct3D device, and Direct3D device context.
            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);

            Device = device;
            SwapChain = swapChain;

            DeviceContext = Device.ImmediateContext;

            // Create render target view for back buffer
            Texture2D backBuffer = SwapChain.GetBackBuffer<Texture2D>(0);
            RenderTargetView = new RenderTargetView(Device, backBuffer);
            backBuffer.Dispose();

            // Initialize and set up the description of the depth buffer.
            Texture2DDescription depthBufferDesc = new Texture2DDescription();
            depthBufferDesc.Width = Con.ClientSize.Width;
            depthBufferDesc.Height = Con.ClientSize.Height;
            depthBufferDesc.MipLevels = 1;
            depthBufferDesc.ArraySize = 1;
            depthBufferDesc.Format = Format.D24_UNorm_S8_UInt;
            depthBufferDesc.SampleDescription = new SampleDescription(1, 0);
            depthBufferDesc.Usage = ResourceUsage.Default;
            depthBufferDesc.BindFlags = BindFlags.DepthStencil;
            depthBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            depthBufferDesc.OptionFlags = ResourceOptionFlags.None;

            // Create the texture for the depth buffer using the filled out description.
            DepthStencilBuffer = new Texture2D(device, depthBufferDesc);

            // Create the depth stencil view.
            DepthStencilView = new DepthStencilView(device, DepthStencilBuffer);

            // Bind the render target view and depth stencil buffer to the output render pipeline.
            DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);


            // Setup the raster description which will determine how and what polygon will be drawn.
            RasterizerStateDescription rasterDesc = new RasterizerStateDescription();
            rasterDesc.CullMode = CullMode.None;
            rasterDesc.FillMode = FillMode.Solid;

            // Create the rasterizer state from the description we just filled out.
            RasterState = new RasterizerState(Device, rasterDesc);


            //------------New--------------

            // Create a texture sampler state description.
            SamplerStateDescription SamplerDesc = new SamplerStateDescription();
            SamplerDesc.Filter = Filter.MinMagMipLinear;
            SamplerDesc.AddressU = TextureAddressMode.Wrap;
            SamplerDesc.AddressV = TextureAddressMode.Wrap;
            SamplerDesc.AddressW = TextureAddressMode.Wrap;
            SamplerDesc.MipLodBias = 0;
            SamplerDesc.MaximumAnisotropy = 1;
            SamplerDesc.ComparisonFunction = Comparison.Always;
            SamplerDesc.BorderColor = new Color4(0, 0, 0, 0);  // Black Border.
            SamplerDesc.MinimumLod = 0;
            SamplerDesc.MaximumLod = float.MaxValue;

            // Create the texture sampler state.
            SamplerState = new SamplerState(Device, SamplerDesc);




            // Now set the rasterizer state.
            DeviceContext.Rasterizer.State = RasterState;

            Viewport Viewport = new Viewport();
            Viewport.Width = Con.ClientSize.Width;
            Viewport.Height = Con.ClientSize.Height;
            Viewport.X = 0;
            Viewport.Y = 0;
            Viewport.MaxDepth = 1;
            Viewport.MinDepth = 0;

            // Setup and create the viewport for rendering.
            DeviceContext.Rasterizer.SetViewport(Viewport);
        }


        public void InitScene()
        {
            InitShaders();
            InitCamera();
            Model("lara.obj");
        }

        
        public void InitCamera()
        {
            ObjectBuffer = Shaders.CreateBuffer<MatrixBuffer>(Device);

            //Camera information
            Position = new Vector3(0.0f, 0.0f, -3.0f);
            Target = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);

            //Set the View matrix
            View = Matrix.LookAtLH(Position, Target, Up);

            //Set the Projection matrix
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, 1.2f, 1.0f, 1000.0f);
        }


        public void InitShaders()
        {
            ShaderByte["VS"] = Shaders.CompileShader("Shaders/VertexShader.hlsl", "VS", "vs_4_0");
            ShaderByte["PS"] = Shaders.CompileShader("Shaders/PixelShader.hlsl", "PS", "ps_4_0");


            // Now setup the layout of the data that goes into the shader.
            // This setup needs to match the VertexType structure in the Model and in the shader.
            InputElement[] inputElements = new InputElement[]
            {
                new InputElement()
                {
                    SemanticName = "POSITION",
                    SemanticIndex = 0,
                    Format = Format.R32G32B32_Float,
                    Slot = 0,
                    AlignedByteOffset = 0,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                },
                new InputElement()
                {
                    SemanticName = "TEXCOORD",
                    SemanticIndex = 0,
                    Format = Format.R32G32B32A32_Float,
                    Slot = 0,
                    AlignedByteOffset = InputElement.AppendAligned,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            // Create the vertex input the layout.
            Layout = new InputLayout(Device, ShaderByte["VS"].Data, inputElements);
            

            DeviceContext.InputAssembler.InputLayout = Layout;

            DeviceContext.VertexShader.Set(new VertexShader(Device, ShaderByte["VS"]));
            DeviceContext.PixelShader.Set(new PixelShader(Device, ShaderByte["PS"]));
        }


        public void Model(string FileName, bool flipUv = false)
        {

            AssimpContext importer = new AssimpContext();

            if (!importer.IsImportFormatSupported(Path.GetExtension(FileName)))
            {
                throw new ArgumentException("Model format " + Path.GetExtension(FileName) + " is not supported!  Cannot load {1}", "filename");
            }


            ConsoleLogStream logStream = new ConsoleLogStream();
            logStream.Attach();

            PostProcessSteps postProcessFlags = PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace;
            if (flipUv)
            {
                postProcessFlags |= PostProcessSteps.FlipUVs;
            }


            Scene model = importer.ImportFile(FileName, postProcessFlags);

            List<Vertex> Vertices = new List<Vertex>();
            List<int> Indices = new List<int>();
            int[] indices = new int[model.Meshes.Sum(m => m.FaceCount * 3)];

            int vertexOffSet = 0;

            int indexOffSet = 0;

            foreach (Mesh mesh in model.Meshes)
            {
                List<Vertex> verts = new List<Vertex>();

                List<Face> faces = mesh.Faces;

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    Vector3 pos = mesh.HasVertices ? new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z) : new Vector3();

                    Vector3D texC = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D();
                    Vector2 TeC = new Vector2(texC.X, texC.Y);
                    Vertex v = new Vertex(pos, TeC);
                    verts.Add(v);
                }

                Vertices.AddRange(verts);

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    indices[i * 3 + 0] = (int)faces[i].Indices[2];
                    indices[i * 3 + 1] = (int)faces[i].Indices[1];
                    indices[i * 3 + 2] = (int)faces[i].Indices[0];
                    Indices.Add(indices[i * 3 + 2] + vertexOffSet);
                    Indices.Add(indices[i * 3 + 1] + vertexOffSet);
                    Indices.Add(indices[i * 3 + 0] + vertexOffSet);

                }
                vertexOffSet += mesh.VertexCount;
                indexOffSet += mesh.FaceCount * 3;
            }


            IndexCount = Indices.Count();


            VertexBuffer = Buffer.Create<Vertex>(Device, BindFlags.VertexBuffer, Vertices.ToArray());
            IndexBuffer = Buffer.Create<int>(Device, BindFlags.IndexBuffer, Indices.ToArray()); 


            VertexBufferBinding vertexBuffer = new VertexBufferBinding();
            vertexBuffer.Buffer = VertexBuffer;
            vertexBuffer.Stride = Utilities.SizeOf<Vertex>();
            vertexBuffer.Offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            DeviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);
            DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0); 

            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            CubesTexture1 = BitmapLoader.LoadTextureFromFile(Device, "Text/Texture.png");
            CubesTexture2 = DDSLoader.LoadTextureFromFile(Device, DeviceContext, "Text/WoodCrate01.dds");
        }


        public void UpdateScene()
        {
            //Update the colors of our scene
            R = 0.0f;
            G = 0.2f;
            B = 0.4f;

            //Keep the cubes rotating
            rot += .009f;


            //Reset cube1 World
            cube1World = Matrix.Identity;
            //Define cube1's world space matrix
            Rotation = Matrix.RotationYawPitchRoll(rot, 0, 0);
            Translation = Matrix.Translation(0.5f, -0.7f, 0.0f);
            //Set cube1's world space using the transformations
            cube1World = Rotation * Translation;



            //Reset cube2 World
            cube2World = Matrix.Identity;
            //Define cube2's world space matrix
            Rotation = Matrix.RotationYawPitchRoll(rot, -0, -0);
            Translation = Matrix.Translation(-0.5f, -0.7f, 0.0f);
            //Set cube2's world space using the transformations
            cube2World = Rotation * Translation;
        }





        public void DrawScene()
        {
            //Clear our backbuffer to the updated color
            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(R, G, B, A));

            // Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);



            //Set the WVP matrix and send it to the constant buffer in effect file
            ObjectCB.W = Matrix.Transpose(cube1World);
            ObjectCB.V = Matrix.Transpose(View);
            ObjectCB.P = Matrix.Transpose(Projection);

            DeviceContext.UpdateSubresource<MatrixBuffer>(ref ObjectCB, ObjectBuffer);

            //Pass constant buffer to shader
            DeviceContext.VertexShader.SetConstantBuffer(0, ObjectBuffer);

            DeviceContext.Rasterizer.State = new RasterizerState(Device, new RasterizerStateDescription()
            {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.None,
            });
            // Set the sampler state in the pixel shader.
            DeviceContext.PixelShader.SetSampler(0, SamplerState);////**new**

            DeviceContext.PixelShader.SetShaderResources(0, CubesTexture1);////**new**

            //Draw the first cube
            DeviceContext.DrawIndexed(IndexCount, 0, 0);


            //-------------------------------------------

            ObjectCB.W = Matrix.Transpose(cube2World);
            ObjectCB.V = Matrix.Transpose(View);
            ObjectCB.P = Matrix.Transpose(Projection);

            DeviceContext.UpdateSubresource<MatrixBuffer>(ref ObjectCB, ObjectBuffer);

            //Pass constant buffer to shader
            DeviceContext.VertexShader.SetConstantBuffer(0, ObjectBuffer);

            DeviceContext.Rasterizer.State = new RasterizerState(Device, new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
            });

            // Set the sampler state in the pixel shader.
            DeviceContext.PixelShader.SetSampler(0, SamplerState);////**new**

            DeviceContext.PixelShader.SetShaderResources(0, CubesTexture2);////**new**

            ////Draw the second cube
            DeviceContext.DrawIndexed(IndexCount, 0, 0);

        }


        public void EndScene()
        {
            //Present the backbuffer to the screen
            SwapChain.Present(1, PresentFlags.None);
        }


        public void ReleaseObjects()
        {
            RenderTargetView.Dispose();
            SwapChain.Dispose();
            Device.Dispose();
            DeviceContext.Dispose();
            Layout.Dispose();
            VertexBuffer.Dispose();
            ShaderByte["VS"].Dispose();
            ShaderByte["PS"].Dispose();
            IndexBuffer.Dispose();
            ObjectBuffer.Dispose();
            RasterState.Dispose();


            //----New----
            SamplerState.Dispose();
            CubesTexture1.Dispose();
            CubesTexture2.Dispose();
        }
    }
}
