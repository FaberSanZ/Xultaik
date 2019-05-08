using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems;

namespace _02__Initialize_Direct3D_11
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
