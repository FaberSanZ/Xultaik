using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics
{
    public class PresentationParameters
    {
        public PresentationParameters() { }


        public PresentationParameters(int width, int height, IntPtr handle, Format backBufferFomat, Format depthBufferFormat)
        {
            Width = width;
            Height = height;
            Handle = handle;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public IntPtr Handle { get; set; }
        public Control Con { get; internal set; }
    }
}
