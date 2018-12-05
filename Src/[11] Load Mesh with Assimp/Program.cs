using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11__Load_Mesh_with_Assimp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var App = new Core())
            {
                App.Initialize();
                App.Run();
            }
        }
    }
}
