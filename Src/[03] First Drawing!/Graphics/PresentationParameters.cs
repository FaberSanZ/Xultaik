using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class PresentationParameters
    {
        public PresentationParameters() { }


        public PresentationParameters(int width, int height, IntPtr handle, Format backBufferFomat = Format.B8G8R8A8_UNorm)
        {
            Width = width;
            Height = height;
            BackBufferFormat = backBufferFomat;
            Handle = handle;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public Format BackBufferFormat { get; set; }

        public IntPtr Handle { get; set; }

    }
}
