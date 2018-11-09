using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _02__First_Drawing_
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
    }
}
