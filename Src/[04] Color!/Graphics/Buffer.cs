using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class Buffer
    {
        public GraphicsDevice Device { get; }

        public Resource Resource { get; set; }

        public ResourceInfo ResourceInfo { get; }

        public int SizeInBytes { get; }

        public int ElementSize { get; }


        public Buffer(int sizeInBytes, int elementSize, GraphicsDevice device, ResourceInfo resourceInfo)
        {
            ElementSize = elementSize;
            SizeInBytes = sizeInBytes;
            Device = device;
            ResourceInfo = resourceInfo;


            BufferDescription description = new BufferDescription()
            {
                BindFlags = resourceInfo.BindFlags,
                CpuAccessFlags = resourceInfo.AccessFlag,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = SizeInBytes,
                StructureByteStride = ElementSize,
                Usage = resourceInfo.UsageType
            };

            Resource = new SharpDX.Direct3D11.Buffer(Device.NativeDevice, description);
        }


        public void Update<T>(params T[] Data) where T : struct
        {
            Device.NativeDeviceContext.UpdateSubresource<T>(Data, Resource);
        }

    }
}
