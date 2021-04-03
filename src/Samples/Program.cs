using Samples.Samples;
using Samples.ClearScreen;
using System;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            using var App = new ClearScreenExample();
            //using var App = new Triangle();
            //using var App = new Transformations();
            //using var App = new LoadGLTF();
            //using var App = new LoadTexture();
            //using var App = new Lighting();

            App.Run();
        }


    }
}
