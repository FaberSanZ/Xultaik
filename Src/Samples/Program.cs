using System;
using Samples.Common;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //using var App = new ClearScreen.ClearScreenExample();
            using var App = new Triangle.TriangleExample();
            //using var App = new Transformations.TransformationsExample();
            //using var App = new PushConstant.PushConstantExample();
            //using var App = new LoadGLTF.LoadGLTFExample();
            //using var App = new LoadTexture.LoadTextureExample();
            //using var App = new Bindless.Bindless();
            //using var App = new DiffuseLighting.DiffuseLightingExample();
            //using var App = new AmbientLighting.AmbientLighting();
            //using var App = new SpecularLighting.SpecularLighting();
            //using var App = new ComputeTexture.ComputeTexture();
            //using var App = new Skinning.SkinningExample();

            App.Run();
        }


    }
}
