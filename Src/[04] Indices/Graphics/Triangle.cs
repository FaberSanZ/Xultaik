using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace _04__Indices
{
    public class Triangle
    {
        // Properties
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }

        public int VertexCount { get; set; }
        public int IndexCount { get; set; }

        // Methods.
        public void InitializeBuffer(Device Device)
        {
            VertexCount = 4;
            IndexCount = 6;


            Vertex[] Vertices = new Vertex[VertexCount];
            Vertices[0] = new Vertex(new Vector3(-0.7f, +0.7f, 0.0f), Color.Red);
            Vertices[1] = new Vertex(new Vector3(+0.7f, +0.7f, 0.0f), Color.Blue);
            Vertices[2] = new Vertex(new Vector3(-0.7f, -0.7f, 0.0f), Color.Green);
            Vertices[3] = new Vertex(new Vector3(+0.7f, -0.7f, 0.0f), Color.Yellow);

            int[] Indices = new int[]
            {
                0, 1, 2, 
                2, 1, 3,

                //0, 3, 2,
                //0, 1, 3,
            };


            VertexBuffer = Buffer.Create<Vertex>(Device, BindFlags.VertexBuffer, Vertices);
            IndexBuffer = Buffer.Create<int>(Device, BindFlags.IndexBuffer, Indices);

        }


        public void RenderBuffers(DeviceContext deviceContext)
        {
            VertexBufferBinding vertexBuffer = new VertexBufferBinding();
            vertexBuffer.Buffer = VertexBuffer;
            vertexBuffer.Stride = Utilities.SizeOf<Vertex>();
            vertexBuffer.Offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);

            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);


            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
