// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.Text;
using Vortice;
using Vortice.DXGI;
using static Vortice.DXGI.DXGI;

namespace Xultaik.Graphics
{
    public class GraphicsAdapter
    {
        public List<string> Description { get; private set; } = new List<string>();

        public List<int> VendorId { get; private set; } = new List<int>();



        internal IDXGIAdapter Adapter;
        internal List<IDXGIAdapter> Adapters = new List<IDXGIAdapter>();
        internal IDXGIFactory4 NativeFactory;

        public GraphicsAdapter()
        {
            InitializeFromImpl();
        }




        public void Recreate()
        {
            InitializeFromImpl();
        }

        internal void InitializeFromImpl()
        {

            if (CreateDXGIFactory2(true, out IDXGIFactory4 tempDXGIFactory4).Failure)
                throw new InvalidOperationException("Cannot create IDXGIFactory4");


            NativeFactory = tempDXGIFactory4;


            NativeFactory.EnumAdapters1(0, out var adapter);

            AdapterDescription1 desc = adapter.Description1;



                Adapters.Add(adapter);
                Description.Add(adapter.Description.Description);
                VendorId.Add(adapter.Description.DeviceId);
            

            Adapter = Adapters[0]; // TODO:

        }
    }
}
