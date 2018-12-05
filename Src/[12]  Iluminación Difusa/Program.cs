using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _12___Iluminación_Difusa
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
