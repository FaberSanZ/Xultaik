using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _03__First_Drawing_;
using Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Windows;
using Buffer = Graphics.Buffer;
using CommandList = Graphics.CommandList;

namespace Systems
{
    public class RenderSystem
    {
        public List<GraphicsAdapter> Adapters { get; set; }

        public GraphicsDevice Device { get; set; }

        public GraphicsSwapChain SwapChain { get; set; }

        public CommandList CommandList { get; set; }

        public Texture Texture { get; set; }


        //---New---
        public Buffer VertexBuffer { get; set; }

        public Mesh  Triangle { get; set; }

        public Shaders  Shaders { get; set; }




        public RenderSystem(PresentationParameters parameters)
        {
            Adapters = GraphicsAdapter.EnumerateGraphicsAdapter();

            Device = new GraphicsDevice(Adapters[0]);

            SwapChain = new GraphicsSwapChain(parameters, Device);

            CommandList = new CommandList(Device);

            Texture = new Texture(Device, SwapChain);

            Shaders = new Shaders(Device, "Shaders/VertexShader.hlsl", "Shaders/PixelShader.hlsl");

            Triangle = new Mesh();

            VertexBuffer = new Buffer(Triangle.SizeInBytes, Triangle.Size, Device, ResourceInfo.VertexBuffer());
        }




        public void Draw()
        {
            Device.Reset();

            VertexBuffer.Update<Vertex>(Triangle.Vertices);

            CommandList.SetRenderTarget(Texture);

            //Clear our backbuffer to the updated color
            CommandList.Clear(Texture, new Color4(0.0f, 0.2f, 0.4f, 1));

            CommandList.SetInputLayout(Shaders.Layout);

            CommandList.SetVertexBuffer(VertexBuffer);

            CommandList.SetPrimitiveType(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            CommandList.SetVertexShader(Shaders.VertexShader);

            CommandList.SetPixelShader(Shaders.PixelShader);

            CommandList.SetViewPort(SwapChain.PresentParameters.Width, SwapChain.PresentParameters.Height, 0, 0);

            CommandList.Draw(Triangle.VertexCount);
        }



        public void End()
        {
            //Present the backbuffer to the screen
            SwapChain.Present(true);
        }
    }
}
