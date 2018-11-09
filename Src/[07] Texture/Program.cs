using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07__Texture
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main
            using(var App = new CoreEngine())
            {
                App.Initialize();
                App.Run();
            }
        }
    }
}
