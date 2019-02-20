using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace _12___Iluminación_Difusa
{
    public class ConstantBuffer<T> where T : struct
    {

        public Buffer Buffer { get; set; }

        private DataStream resource; // Lock the constant buffer so it can be written to.

        public DataStream Resource => resource;



        public ConstantBuffer(Device Device) 
        {
            BufferDescription BufferDescription = new BufferDescription();
            BufferDescription.Usage = ResourceUsage.Dynamic;
            BufferDescription.SizeInBytes = Utilities.SizeOf<T>();
            BufferDescription.BindFlags = BindFlags.ConstantBuffer;
            BufferDescription.CpuAccessFlags = CpuAccessFlags.Write;
            BufferDescription.OptionFlags = ResourceOptionFlags.None;
            BufferDescription.StructureByteStride = 0;

            Buffer = new Buffer(Device, BufferDescription);
        }




        public void UpdateShader(ShaderType shaderType, DeviceContext deviceContext, T Data)
        {
            switch (shaderType)
            {
                case ShaderType.Pixel:

                    deviceContext.MapSubresource(Buffer, MapMode.WriteDiscard, MapFlags.None, out resource);

                    resource.Write(Data);

                    // Unlock the constant buffer.
                    deviceContext.UnmapSubresource(Buffer, 0);


                    // Finally set the constant buffer in the vertex shader with the updated values.
                    deviceContext.VertexShader.SetConstantBuffer(0, Buffer);
                    break;

                case ShaderType.Vertex:

                    deviceContext.MapSubresource(Buffer, MapMode.WriteDiscard, MapFlags.None, out resource);

                    resource.Write(Data);

                    // Unlock the constant buffer.
                    deviceContext.UnmapSubresource(Buffer, 0);


                    // Finally set the constant buffer in the pixel shader with the updated values.
                    deviceContext.PixelShader.SetConstantBuffer(0, Buffer);
                    break;
            }

        }   
    }
}
