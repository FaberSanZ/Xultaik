using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06__Render_states
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
