using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class CommandList
    {
        private SharpDX.Direct3D11.DeviceContext NativeDeviceContext;


        public CommandList(GraphicsDevice device) 
        {
            NativeDeviceContext = device.NativeDeviceContext;
        }



        public void Draw(int vertexCount, int startVertexLocation = 0)
        {
            NativeDeviceContext.Draw(vertexCount, startVertexLocation);
        }


        public void DrawIndexed(int indexCount, int startVertexLocation = 0, int baseVertexLocation = 0)
        {
            NativeDeviceContext.DrawIndexed(indexCount, startVertexLocation, baseVertexLocation);
        }


        public void SetRenderTarget(Texture texture)
        {
            //set render target
            NativeDeviceContext.OutputMerger.SetRenderTargets(texture.NativeRenderTargetView);
        }



        public void SetPrimitiveType(PrimitiveTopology primitiveType)
        {
            //set primitive type
            NativeDeviceContext.InputAssembler.PrimitiveTopology = primitiveType;
        }



        public void SetVertexBuffer(Buffer buffer)
        {
            //set vertex buffer to input stage
            //create buffer binding
            VertexBufferBinding bufferBinding = new VertexBufferBinding(buffer.Resource as SharpDX.Direct3D11.Buffer, buffer.ElementSize, 0);

            //set vertex buffer
            NativeDeviceContext.InputAssembler.SetVertexBuffers(0, bufferBinding);
        }



        public void SetIndexBuffer(Buffer buffer)
        {
            //set index buffer to input stage
            //set index buffer
            NativeDeviceContext.InputAssembler.SetIndexBuffer(buffer.Resource as SharpDX.Direct3D11.Buffer, SharpDX.DXGI.Format.R32_UInt, 0);
        }



        public void SetInputLayout(InputLayout inputLayout)
        {
            //set input layout Direct3D instance to pipeline
            NativeDeviceContext.InputAssembler.InputLayout = inputLayout;
        }

        public void SetViewPort(float Width, float Height, float X, float Y, float MinDepth = 0.0f, float MaxDepth = 1.0f)
        {

            var viewPortF = new SharpDX.Mathematics.Interop.RawViewportF()
            {
                Width = Width,
                Height = Height,

                X = X,
                Y = Y,

                MinDepth = MinDepth,
                MaxDepth = MaxDepth
            };
            //set view port
            NativeDeviceContext.Rasterizer.SetViewport(viewPortF);
        }

        public void SetVertexShader(VertexShader vertexShader)
        {
            //set vertex shader Direct3D instance to pipeline
            NativeDeviceContext.VertexShader.SetShader(vertexShader, null, 0);
        }

        public void SetPixelShader(PixelShader pixelShader)
        {
            //set pixel shader Direct3D instance to pipeline
            NativeDeviceContext.PixelShader.SetShader(pixelShader, null, 0);
        }


        public  void Clear(Texture renderTarget, Color4 color)
        {
            if (renderTarget == null) throw new ArgumentNullException("RenderTarget");

            NativeDeviceContext.ClearRenderTargetView(renderTarget.NativeRenderTargetView, color);
        }
    }
}
