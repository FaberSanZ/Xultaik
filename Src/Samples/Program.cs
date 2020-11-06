using Samples.Samples;
using System;

namespace Samples
{



    class Program
    {
        static void Main(string[] args)
        {
            //using var App = new ClearScreen();
            //using var App = new Triangle();
            using var App = new Transformations();
            //using var App = new LoadTexture();
            //using var App = new LoadGLTF();

            App.Run();

            //Window window = new Window("Zeckoxe", 100, 100, 900, 600, WindowState.Normal);

            //window.RenderLoop(() => 
            //{
            //    Console.WriteLine(window.Height);
            //});
        }


    }
}
