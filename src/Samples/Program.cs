using System;
using Samples.Common;
using Samples.Samples;
using Samples.LoadTexture;
using Samples.Triangle;
using Samples.ClearScreen;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //using var App = new ClearScreenExample();
            using var App = new TriangleExample();
            //using var App = new Transformations();
            //using var App = new LoadGLTF();
            //using var App = new LoadTextureExample();
            //using var App = new Lighting();
            //using var App = new SkinningExample();


            App.Run();

        }


    }
}
