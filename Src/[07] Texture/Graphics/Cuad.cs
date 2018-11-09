using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace _07__Texture
{
    public class Cuad
    {
        // Properties
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }

        public int VertexCount { get; set; }
        public int IndexCount { get; set; }

        public ShaderResourceView Texture { get; set; }


        // Methods.
        public void InitializeBuffer(Device Device, string TFileName)
        {
            VertexCount = 8;
            IndexCount = 12;


            Vertex[] Vertices = new Vertex[VertexCount];
            Vertices[0] = new Vertex(new Vector3(-1.0f, +1.0f, 0.0f), new Vector2(1, 0));
            Vertices[1] = new Vertex(new Vector3(+1.0f, +1.0f, 0.0f), new Vector2(0, 0));
            Vertices[2] = new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), new Vector2(1, 1));
            Vertices[3] = new Vertex(new Vector3(+1.0f, -1.0f, 0.0f), new Vector2(0, 1));

            //Vertices[4] = new Vertex(new Vector3(-1.0f, +1.0f, -1.0f), new Vector2(1, 0));
            //Vertices[5] = new Vertex(new Vector3(+1.0f, +1.0f, -1.0f), new Vector2(0, 0));
            //Vertices[6] = new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 1));
            //Vertices[7] = new Vertex(new Vector3(+1.0f, -1.0f, -1.0f), new Vector2(0, 1));

            int[] Indices = new int[]
            {
                0, 1, 2,
                2, 1, 3,

                //4, 5, 6,
                //6, 5, 7,
            };


            VertexBuffer = Buffer.Create<Vertex>(Device, BindFlags.VertexBuffer, Vertices);
            IndexBuffer = Buffer.Create<int>(Device, BindFlags.IndexBuffer, Indices);

            string ext = Path.GetExtension(TFileName);

            if (ext.ToLower() == ".dds")
            {

                Texture = DDSLoader.LoadTextureFromFile(Device, Device.ImmediateContext, TFileName);
            }
            else
            {

                Texture = BitmapLoader.LoadTextureFromFile(Device, TFileName);
            }

        }


        public void RenderBuffers(DeviceContext deviceContext, PrimitiveTopology PrimitiveTopology = PrimitiveTopology.TriangleList)
        {
            VertexBufferBinding vertexBuffer = new VertexBufferBinding();
            vertexBuffer.Buffer = VertexBuffer;
            vertexBuffer.Stride = Utilities.SizeOf<Vertex>();
            vertexBuffer.Offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);

            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);


            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology;
        }

    }
}
