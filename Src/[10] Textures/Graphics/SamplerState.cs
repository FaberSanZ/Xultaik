using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class SamplerState
    {
        public SharpDX.Direct3D11.SamplerState Sampler { get; set; }

        public GraphicsDevice GraphicsDevice { get; }


        public TextureAddressMode AddressU { get; }
        public TextureAddressMode AddressV { get; }
        public TextureAddressMode AddressW { get; }

        public Filter Filter { get; }

        public SamplerState(GraphicsDevice device, TextureAddressMode addressU, TextureAddressMode addressV, TextureAddressMode addressW, Filter filter)
        {
            GraphicsDevice = device;

            //set address mode 
            AddressU = addressU;
            AddressV = addressV;
            AddressW = addressW;

            Filter = filter;


            SamplerStateDescription SamplerDesc = new SamplerStateDescription();
            SamplerDesc.Filter = Filter.MinMagMipLinear;
            SamplerDesc.AddressU = AddressU;
            SamplerDesc.AddressV = AddressV;
            SamplerDesc.AddressW = AddressW;
            SamplerDesc.MipLodBias = 0;
            SamplerDesc.MaximumAnisotropy = 1;
            SamplerDesc.ComparisonFunction = Comparison.Always;
            SamplerDesc.BorderColor = new  SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0);  // Black Border.
            SamplerDesc.MinimumLod = 0;
            SamplerDesc.MaximumLod = float.MaxValue;

            Sampler = new SharpDX.Direct3D11.SamplerState(GraphicsDevice.NativeDevice, SamplerDesc);
        }

    }
}
