using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10__Simple_terrain
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var App = new CoreEngine())
            {
                App.Initialize();
                App.Run();
            }
        }
    }
}
