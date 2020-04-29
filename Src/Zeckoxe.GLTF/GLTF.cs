using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.GLTF
{
    class GLTF
    {
        private readonly IList<System.Numerics.Vector3> _Positions;
        private readonly IList<System.Numerics.Vector3> _NORMAL;
        private readonly IList<System.Numerics.Vector2> _Textute;


        //public GLTF(string FileName)
        //{

        //    Vertices = new ModelPart<Vertex>();

        //    Indices = new ModelPart<int>();



        //    ModelRoot model = ModelRoot.Load(FileName);
        //    Console.WriteLine(model.LogicalTextures[0].Name);

        //    SceneTemplate[] templates = model.LogicalScenes.Select(item => SceneTemplate.Create(item, true)).ToArray();
        //    Dictionary<int, SharpGLTF.Schema2.Mesh> meshes = templates.SelectMany(item => item.LogicalMeshIds).ToDictionary(k => k, k => model.LogicalMeshes[k]);

        //    Console.WriteLine("Logical Images Count {0}", model.LogicalImages.Count);
        //    int im = 0;
        //    int by = 0;
        //    foreach (var image in model.LogicalImages)
        //    {
        //        Console.WriteLine("IMG {0}: Is Jpeg {1}", im, image.IsJpeg);
        //        Console.WriteLine("IMG {0}: Is Png {1}", im, image.IsPng);
        //        Console.WriteLine("IMG {0}: Is DDS {1}", im, image.IsDds);
        //        Console.WriteLine("IMG {0}: Is Webp {1}", im, image.IsWebp);
        //        Console.WriteLine("IMG {0}: Name {1}", im, image.Name);
        //        Console.WriteLine("IMG {0}: File Extension {1}", im, image.FileExtension);
        //        Console.WriteLine("IMG {0}: File Extension {1}", im, image.LogicalParent.LogicalTextures[0].Name);
        //        foreach (var Extension in image.Extensions)
        //        {
        //            Console.WriteLine("IMG {0}: File Extensions {1}", im, Extension.ToString());
        //        }

        //        Console.WriteLine("-------------Bytes------------");
        //        //foreach (var ImageContent in image.GetImageContent())
        //        //{
        //        //    Console.Write(" {0}, ", ImageContent);
        //        //    if (by == 10)
        //        //    {
        //        //        Console.WriteLine();
        //        //        by = 0;
        //        //    }
        //        //    by ++;
        //        //}
        //        Console.WriteLine("========================");
        //        by++;

        //        im++;
        //    }



        //    List<MeshPrimitive> srcPrims = GetValidPrimitives(meshes[0]).ToList();

        //    foreach (MeshPrimitive srcPrim in srcPrims)
        //    {
        //        _Positions = srcPrim.GetVertexAccessor("POSITION")?.AsVector3Array();
        //        _NORMAL = srcPrim.GetVertexAccessor("NORMAL")?.AsVector3Array();
        //        _Textute = srcPrim.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array();
        //        var s = srcPrim.Material.AlphaCutoff;

        //    }

        //    for (int i = 0; i < _Positions.Count; i++)
        //    {
        //        Vertices.Data.Add(new Vertex()
        //        {
        //            Position = _Positions[i],
        //            Normal = _NORMAL[i],
        //            Tex = _Textute[i],
        //        });
        //    }



        //    foreach (MeshPrimitive srcPrim in srcPrims)
        //    {

        //        IEnumerable<(int A, int B, int C)> front = srcPrim.GetTriangleIndices();
        //        IEnumerable<(int A, int C, int B)> back = front.Select(item => (item.A, item.C, item.B));
        //        (int A, int, int)[] _Triangles = front.Concat(back).ToArray();
        //        Indices.Data = CreateIndexBuffer(srcPrim.GetTriangleIndices())/*.Select(item => item).ToArray().ToList()*/;
        //    }






        //    Vertices.Count = Vertices.Data.Count();

        //    Vertices.SizeInBytes = Utilities.SizeOf<Vertex>() * Vertices.Data.Count();

        //    Vertices.Size = Utilities.SizeOf<Vertex>();





        //    Indices.Count = Indices.Data.Count();

        //    Indices.SizeInBytes = Utilities.SizeOf<int>() * Indices.Data.Count();

        //    Indices.Size = Utilities.SizeOf<int>();

        //}




        //private static IEnumerable<SharpGLTF.Schema2.MeshPrimitive> GetValidPrimitives(SharpGLTF.Schema2.Mesh srcMesh)
        //{
        //    foreach (MeshPrimitive srcPrim in srcMesh.Primitives)
        //    {
        //        Accessor ppp = srcPrim.GetVertexAccessor("POSITION");
        //        if (ppp.Count < 3)
        //        {
        //            continue;
        //        }

        //        if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.POINTS)
        //        {
        //            continue;
        //        }

        //        if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.LINES)
        //        {
        //            continue;
        //        }

        //        if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.LINE_LOOP)
        //        {
        //            continue;
        //        }

        //        if (srcPrim.DrawPrimitiveType == SharpGLTF.Schema2.PrimitiveType.LINE_STRIP)
        //        {
        //            continue;
        //        }

        //        yield return srcPrim;
        //    }
        //}


        //private List<int> CreateIndexBuffer(IEnumerable<(int A, int B, int C)> triangles)
        //{
        //    int[] sequence32 = triangles.SelectMany(item => new[] { (int)item.C, (int)item.B, (int)item.A }).ToArray();

        //    int max = sequence32.Max();


        //    return sequence32.ToList();

        //}
    }
}
