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

        public void UpdateConstant<T>(ShaderType Type,int Slot, params T[] Data) where T : struct
        {
            Device.NativeDeviceContext.UpdateSubresource<T>(Data, Resource);

            switch (Type)
            {
                case ShaderType.None:
                    break;

                case ShaderType.VertexShader:
                    // Finally set the constant buffer in the vertex shader with the updated values.
                    Device.NativeDeviceContext.VertexShader.SetConstantBuffer(Slot, Resource as SharpDX.Direct3D11.Buffer);
                    break;

                case ShaderType.PixelShader:
                    // Finally set the constant buffer in the PixelShader shader with the updated values.
                    Device.NativeDeviceContext.PixelShader.SetConstantBuffer(Slot, Resource as SharpDX.Direct3D11.Buffer);
                    break;

                case ShaderType.HullShader:
                    // Finally set the constant buffer in the HullShader shader with the updated values.
                    Device.NativeDeviceContext.HullShader.SetConstantBuffer(Slot, Resource as SharpDX.Direct3D11.Buffer);
                    break;

                case ShaderType.GeometryShader:
                    // Finally set the constant buffer in the GeometryShader shader with the updated values.
                    Device.NativeDeviceContext.GeometryShader.SetConstantBuffer(Slot, Resource as SharpDX.Direct3D11.Buffer);
                    break;

                case ShaderType.DomainShader:
                    // Finally set the constant buffer in the DomainShader shader with the updated values.
                    Device.NativeDeviceContext.DomainShader.SetConstantBuffer(Slot, Resource as SharpDX.Direct3D11.Buffer);
                    break;

                case ShaderType.ComputeShader:
                    // Finally set the constant buffer in the ComputeShader shader with the updated values.
                    Device.NativeDeviceContext.ComputeShader.SetConstantBuffer(Slot, Resource as SharpDX.Direct3D11.Buffer);
                    break;
            }

        }

    }
}
