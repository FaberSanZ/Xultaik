using System;
using Zeckoxe.Desktop.GLFWNative;

namespace _02_Hello_Triangle
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var App = new Game())
                App.Run();
        }
    }
}
