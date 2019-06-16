using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class PipelineState
    {
        public RasterizerState RasterizerState { get; set; }

        public RasterizerStateDescription Description;

        protected GraphicsDevice Device { get; }

        public FillMode FillMode { get; }
        public CullMode CullMode { get; }


        public PipelineState(GraphicsDevice device, FillMode fillMode, CullMode cullMode)
        {
            Device = device;

            FillMode = fillMode;
            CullMode = cullMode;

            Description = new RasterizerStateDescription();
            Description.IsAntialiasedLineEnabled = false;
            Description.DepthBias = 0;
            Description.DepthBiasClamp = 0.0f;
            Description.IsDepthClipEnabled = true;
            Description.IsFrontCounterClockwise = false;
            Description.IsMultisampleEnabled = false;
            Description.IsScissorEnabled = false;
            Description.SlopeScaledDepthBias = 0.0f;

            Description.FillMode = FillMode;
            Description.CullMode = CullMode;

            RasterizerState = new RasterizerState(Device.NativeDevice, Description);
        }


    }
}
