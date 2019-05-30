using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class Grid
    {
        public int SizeInBytes { get; set; }

        public int Size { get; set; }


        public int IndexSizeInBytes { get; set; }

        public int IndexSize { get; set; }


        public List<Vertex> Vertices { get; set; }


        public List<int> Indices { get; set; }


        public int VertexCount { get; set; }


        public int IndexCount { get; set; }


        public Random Random = new Random();

        public float NextFloat => (float)Random.NextDouble();





        public Grid(float width, float depth, int m, int n)
        {
            Vertices = new List<Vertex>();

            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 1f / (n - 1);
            float dv = 1f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;
                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    Vertices.Add(new Vertex(new Vector3(x, 0, z), new  Vector2(j * du, i * dv)));
                }
            }

            //
            // Create the indices.
            //

            Indices = new List<int>();

            // Iterate over each quad and compute indices.
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    Indices.Add(i * n + j);
                    Indices.Add(i * n + j + 1);
                    Indices.Add((i + 1) * n + j);

                    Indices.Add((i + 1) * n + j);
                    Indices.Add(i * n + j + 1);
                    Indices.Add((i + 1) * n + j + 1);
                }
            }


            VertexCount = Vertices.Count();

            IndexCount = Indices.Count();

            Size = Utilities.SizeOf<Vertex>();

            SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Count();

            IndexSize = Utilities.SizeOf<int>();

            IndexSizeInBytes = Utilities.SizeOf<int>() * Indices.Count();

        }
    }
}
