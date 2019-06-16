using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems;
using Buffer = Graphics.Buffer;

namespace Graphics
{
    public class RenderItem
    {
        public Matrix[] Worlds { get; set; }


        public LightBuffer Light { get; set; }


        public ShaderResourceView Texture { get; set; }


        public Buffer VertexBuffer { get; set; }


        public Buffer IndexBuffer { get; set; }


        public Buffer[] ConstantBuffer { get; set; }

    }
}
