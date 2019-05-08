using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using Device = SharpDX.Direct3D11.Device;

namespace Graphics
{
    public class GraphicsDevice
    {
        public Device NativeDevice { get; set; }

        public DeviceContext NativeDeviceContext { get; set; }

        public GraphicsAdapter Adapter { get; }

        public GraphicsDevice(GraphicsAdapter adapter)
        {
            //set the adapter
            Adapter = adapter;


            //creation flag, we use debug flag
            DeviceCreationFlags creationFlags = DeviceCreationFlags.Debug;

            //fetuares level: 11_0, 12_0
            FeatureLevel[] fetuares = new FeatureLevel[2]
            {
                 FeatureLevel.Level_11_0,
                 FeatureLevel.Level_12_0
            };

            //create device with current adapter
            NativeDevice = new Device(Adapter.Adapter, creationFlags, fetuares);

            NativeDeviceContext = NativeDevice.ImmediateContext;


        }



        public void Reset()
        {
            NativeDeviceContext.ClearState();
        }
    }
}