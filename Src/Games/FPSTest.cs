using System;
using System.Collections.Generic;
using System.Drawing;
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
            Window window = new(new WindowSettings()
            {
                Border = WindowBorder.Hidden,
                Position = Point.Empty,
                Size = new Size(800, 600),
                State = WindowState.Normal,
                CursorMode = CursorMode.Normal,
                Title = "Test",
                UpdateFrequency = null,
                Icon = WindowResourcePtr.LoadIcon("Content\\win.ico"),
            });
            window.Show();
            window.RenderLoop(() => { });
        }
    }
}
