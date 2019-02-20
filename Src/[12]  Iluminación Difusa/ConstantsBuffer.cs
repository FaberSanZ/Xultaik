using System;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace _12___Iluminación_Difusa
{
    public class ShaderBuffer<T> where T : struct
    {
        private Buffer buffer;

        public Buffer Buffer => buffer;

        private bool mappable;

        /// <summary>
        /// Gets the capacity of the buffer.
        /// </summary>
        public int Capacity { get; private set; }


        /// <summary>
        /// Creates a constants buffer.
        /// </summary>
        /// <param name="device">Device used to create the buffer.</param>
        /// <param name="debugName">Name to associate with the buffer.</param>
        /// <param name="mappable">If true, the buffer will be mapped with WriteDiscard when updated. If false, UpdateSubresource will be used.</param>
        public ShaderBuffer(Device device)
        {
            this.mappable = mappable;

            int size = Utilities.SizeOf<T>();
            int alignedSize = (size >> 4) << 4;
            if (alignedSize < size)
                alignedSize += 16;


            BufferDescription BufferDesc = new BufferDescription();
            BufferDesc.Usage = ResourceUsage.Dynamic;
            BufferDesc.SizeInBytes = size;
            BufferDesc.BindFlags = BindFlags.ConstantBuffer;
            BufferDesc.CpuAccessFlags = CpuAccessFlags.Write;
            BufferDesc.OptionFlags = ResourceOptionFlags.None;
            BufferDesc.StructureByteStride = 0;
                
            



            buffer = new Buffer(device, BufferDesc);
        }


        public void Write(DeviceContext context, ref T bufferData, bool sh =  true)
        {
            //DataStream mappedResource;

            //// Lock the light constant buffer so it can be written to.
            //context.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);


            //mappedResource.Write(bufferData);

            // Unlock the constant buffer.
            context.UpdateSubresource<T>(ref bufferData, buffer);

            // Set the position of the light constant buffer in the pixel shader.

            if (sh)
            {
                context.PixelShader.SetConstantBuffer(0, buffer);
            }
            else
            {
                context.VertexShader.SetConstantBuffer(0, buffer);
            }
            // Finally set the light constant buffer in the pixel shader with the updated values.
        }


        public void Dispose()
        {
            buffer.Dispose();
        }

    }
}
