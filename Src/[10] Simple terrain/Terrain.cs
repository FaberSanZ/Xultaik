using Assimp;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;


namespace _10__Simple_terrain
{
    public class Terrain
    {


        // Variables

        // Properties
        private Buffer VertexBuffer { get; set; }
        private Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public ShaderResourceView Texture { get; set; }

        // Constructor
        public Terrain() { }

        // Methods.
        public bool InitializeBuffers(Device Device, string TFileName, float width, float depth, int m, int n)
        {
            try
            {
                List<Vertex> vertices = new List<Vertex>();
                List<int> indices = new List<int>();

                //
                // Create the vertices.
                //

                float halfWidth = 0.5f * width;
                float halfDepth = 0.5f * depth;

                float dx = width / (n - 1);
                float dz = depth / (m - 1);

                float du = 1f*2 / (n - 1);
                float dv = 1f*2 / (m - 1);

                for (int i = 0; i < m; i++)
                {
                    float z = halfDepth - i * dz;
                    for (int j = 0; j < n; j++)
                    {
                        float x = -halfWidth + j * dx;

                        vertices.Add(new Vertex(new Vector3(x, 0, z), new Vector2(j * du, i * dv)));

                    }
                }

                //
                // Create the indices.
                //

                // Iterate over each quad and compute indices.
                for (int i = 0; i < m - 1; i++)
                {
                    for (int j = 0; j < n - 1; j++)
                    {
                        indices.Add(i * n + j);
                        indices.Add(i * n + j + 1);
                        indices.Add((i + 1) * n + j);

                        indices.Add((i + 1) * n + j);
                        indices.Add(i * n + j + 1);
                        indices.Add((i + 1) * n + j + 1);
                    }
                }


                IndexCount = indices.Count();

                // Create the vertex buffer.
                VertexBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, vertices.ToArray());

                // Create the index buffer.
                IndexBuffer = Buffer.Create(Device, BindFlags.IndexBuffer, indices.ToArray());



                string ext = Path.GetExtension(TFileName);

                if (ext.ToLower() == ".dds")
                {

                    Texture = DDSLoader.LoadTextureFromFile(Device, Device.ImmediateContext, TFileName);
                }
                else
                {

                    Texture = BitmapLoader.LoadTextureFromFile(Device, TFileName);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ShutDown()
        {
            // Release the vertex and index buffers.
            ShutdownBuffers();
        }
        private void ShutdownBuffers()
        {
            // Return the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        public void Render(DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
            RenderBuffers(deviceContext);
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<Vertex>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
