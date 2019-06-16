using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class ModelPart<T> where T : struct
    {
        public List<T> Data { get; set; } = new List<T>();

        public int Count { get; set; }


        public int SizeInBytes { get; set; }

        public int Size { get; set; }

    }
}
