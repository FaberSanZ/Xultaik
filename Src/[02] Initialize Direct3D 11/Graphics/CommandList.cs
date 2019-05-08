using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class CommandList
    {
        private SharpDX.Direct3D11.DeviceContext NativeDeviceContext;


        public CommandList(GraphicsDevice device) 
        {
            NativeDeviceContext = device.NativeDeviceContext;
        }





        public  void Clear(Texture renderTarget, Color4 color)
        {
            if (renderTarget == null) throw new ArgumentNullException("RenderTarget");

            NativeDeviceContext.ClearRenderTargetView(renderTarget.NativeRenderTargetView, color);
        }
    }
}
