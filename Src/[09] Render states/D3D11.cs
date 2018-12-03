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

namespace _09__Render_states
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }

        public Vertex(Vector3 vector3, Color color)
        {
            Position = vector3;
            Color = color;
        }
    }

    //Create effects constant buffer's structure//
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectConstants
    {
        public Matrix WVP;
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

        public ObjectConstants ObjectCB;

        public Matrix cube1World;

        public Matrix cube2World;

        public Matrix Rotation;

        public Matrix Scale;

        public Matrix Translation;

        public float rot = 0.01f;


        //----New------
        private RasterizerState RasterState { get; set; }




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



            ///////////////**************new**************////////////////////
            // Setup the raster description which will determine how and what polygon will be drawn.
            RasterizerStateDescription rasterDesc = new RasterizerStateDescription();
            rasterDesc.CullMode = CullMode.None;
            rasterDesc.FillMode = FillMode.Wireframe;


            // Create the rasterizer state from the description we just filled out.
            RasterState = new RasterizerState(Device, rasterDesc);

            // Now set the rasterizer state.
            DeviceContext.Rasterizer.State = RasterState;

            ///////////////**************new**************////////////////////





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
            Model();
        }

        
        public void InitCamera()
        {
            ObjectBuffer = Shaders.CreateBuffer<ObjectConstants>(Device);

            //Camera information
            Position = new Vector3(0.0f, 0.0f, -7.0f);
            Target = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);

            //Set the View matrix
            View = Matrix.LookAtLH(Position, Target, Up);

            //Set the Projection matrix
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, 1.2f, 1.0f, 1000.0f);
        }


        public void InitShaders()
        {
            ShaderByte["VS"] = Shaders.CompileShader("Effect.hlsl", "VS", "vs_4_0");
            ShaderByte["PS"] = Shaders.CompileShader("Effect.hlsl", "PS", "ps_4_0");


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
                    SemanticName = "COLOR",
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


        public void Model()
        {
            VertexCount = 8;
            IndexCount = 36;

            Vertex[] Vertices = new Vertex[VertexCount];
            Vertices[0] = new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0F, 0.0f, 0.0f, 1.0f));
            Vertices[1] = new Vertex(new Vector3(-1.0f, +1.0f, -1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            Vertices[2] = new Vertex(new Vector3(+1.0f, +1.0f, -1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[3] = new Vertex(new Vector3(+1.0f, -1.0f, -1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f));

            Vertices[4] = new Vertex(new Vector3(-1.0f, -1.0f, +1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            Vertices[5] = new Vertex(new Vector3(-1.0f, +1.0f, +1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f));
            Vertices[6] = new Vertex(new Vector3(+1.0f, +1.0f, +1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f));
            Vertices[7] = new Vertex(new Vector3(+1.0f, -1.0f, +1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f));




            int[] Indices = new int[] 
            {
                // front face
                0, 1, 2,
                0, 2, 3,

                // back face
                4, 6, 5,
                4, 7, 6,

                // left face
                4, 5, 1,
                4, 1, 0,

                // right face
                3, 2, 6,
                3, 6, 7,

                // top face
                1, 5, 6,
                1, 6, 2,

                // bottom face
                4, 0, 3,
                4, 3, 7
            };


            VertexBuffer = Buffer.Create<Vertex>(Device, BindFlags.VertexBuffer, Vertices);
            IndexBuffer = Buffer.Create<int>(Device, BindFlags.IndexBuffer, Indices); 


            VertexBufferBinding vertexBuffer = new VertexBufferBinding();
            vertexBuffer.Buffer = VertexBuffer;
            vertexBuffer.Stride = Utilities.SizeOf<Vertex>();
            vertexBuffer.Offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            DeviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);
            DeviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0); 

            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }


        public void UpdateScene()
        {
            //Update the colors of our scene
            R = 0.0f;
            G = 0.2f;
            B = 0.4f;

            //Keep the cubes rotating
            rot += .01f;



            //Reset cube1 World
            cube1World = Matrix.Identity;
            //Define cube1's world space matrix
            Rotation = Matrix.RotationYawPitchRoll(rot, rot, rot);
            Translation = Matrix.Translation(1.5f, 0.0f, 0.0f);
            //Set cube1's world space using the transformations
            cube1World = Rotation * Translation;



            //Reset cube2 World
            cube2World = Matrix.Identity;
            //Define cube2's world space matrix
            Rotation = Matrix.RotationYawPitchRoll(-rot, -rot, -rot);
            Translation = Matrix.Translation(-1.5f, 0.0f, 0.0f);
            //Set cube2's world space using the transformations
            cube2World = Rotation * Translation;
        }


        public void DrawScene()
        {
            //Clear our backbuffer to the updated color
            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(R, G, B, A));

            // Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);



            ///////////////**************new**************////////////////////
            //Set the WVP matrix and send it to the constant buffer in effect file
            WVP = cube1World * View * Projection;
            ObjectCB.WVP = Matrix.Transpose(WVP);

            DeviceContext.UpdateSubresource<ObjectConstants>(ref ObjectCB, ObjectBuffer);

            //Pass constant buffer to shader
            DeviceContext.VertexShader.SetConstantBuffer(0, ObjectBuffer);

            //Draw the first cube
            DeviceContext.DrawIndexed(IndexCount, 0, 0);


            //-------------------------------------------

            WVP = cube2World * View * Projection;

            ObjectCB.WVP = Matrix.Transpose(WVP);
            DeviceContext.UpdateSubresource<ObjectConstants>(ref ObjectCB, ObjectBuffer);

            //Pass constant buffer to shader
            DeviceContext.VertexShader.SetConstantBuffer(0, ObjectBuffer);

            ////Draw the second cube
            DeviceContext.DrawIndexed(IndexCount, 0, 0);
            ///////////////**************new**************////////////////////

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

            //----New----
            RasterState.Dispose();
        }
    }
}
