using Samples.Samples;
using System;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            using var App = new ClearScreen();
            //using var App = new Triangle();
            //using var App = new Transformations();
            //using var App = new LoadGLTF();
            //using var App = new LoadTexture();
            
            App.Run();
        }
    }
}
