using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xultaik.Desktop;

namespace Games
{
    public class FPSTest
    {
        public FPSTest()
        {
            Window window = new("Xultaik Game Engine", 800, 600);
            window.Show();
            window.RenderLoop(() => { });
        }
    }
}
