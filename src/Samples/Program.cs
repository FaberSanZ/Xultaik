using Samples.Samples;
using Samples.Samples.LoadTexture;
using System;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //using var App = new ClearScreen.ClearScreenExample();
            //using var App = new Triangle();
            //using var App = new Transformations();
            //using var App = new LoadGLTF();
            //using var App = new LoadTextureExample();
            using var App = new Lighting();
            //using var App = new Skinning.SkinningExample();
            

            App.Run();
        }


    }
}
