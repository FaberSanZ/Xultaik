// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsAdapter.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Vortice;
using Vortice.DXGI;
using static Vortice.DXGI.DXGI;

namespace Zeckoxe.Graphics
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


            foreach (IDXGIAdapter1 adapter in NativeFactory.EnumAdapters1())
            {
                AdapterDescription1 desc = adapter.Description1;

                //// Don't select the Basic Render Driver adapter.
                //if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None)
                //    continue;


                Adapters.Add(adapter);
                Description.Add(adapter.Description.Description);
                VendorId.Add(adapter.Description.DeviceId);
            }

            Adapter = Adapters[0]; // TODO:

        }
    }
}
