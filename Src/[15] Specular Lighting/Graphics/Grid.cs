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

        public ModelPart<Vertex> Vertices { get; set; }


        public ModelPart<int> Indices { get; set; }







        public Grid(float width, float depth, int m, int n)
        {
            Vertices = new ModelPart<Vertex>();

            Indices = new ModelPart<int>();


            float halfWidth = 0.5f * width;
            float halfDepth = 0.5f * depth;

            float dx = width / (n - 1);
            float dz = depth / (m - 1);

            float du = 6f / (n - 1);
            float dv = 6f / (m - 1);

            for (int i = 0; i < m; i++)
            {
                float z = halfDepth - i * dz;
                for (int j = 0; j < n; j++)
                {
                    float x = -halfWidth + j * dx;

                    Vertices.Data.Add(new Vertex(
                        new Vector3(x, 0, z),
                        new Vector3(0, 1, 0),
                        new Vector2(j * du, i * dv))); // Stretch texture over grid.
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
                    Indices.Data.Add(i * n + j);
                    Indices.Data.Add(i * n + j + 1);
                    Indices.Data.Add((i + 1) * n + j);

                    Indices.Data.Add((i + 1) * n + j);
                    Indices.Data.Add(i * n + j + 1);
                    Indices.Data.Add((i + 1) * n + j + 1);
                }
            }



            Vertices.Count = Vertices.Data.Count();

            Vertices.SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Data.Count();

            Vertices.Size = Utilities.SizeOf<Vertex>();





            Indices.Count = Indices.Data.Count();

            Indices.SizeInBytes = Utilities.SizeOf<int>() * Indices.Data.Count();

            Indices.Size = Utilities.SizeOf<int>();



        }
    }
}
