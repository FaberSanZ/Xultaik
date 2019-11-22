// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsResource.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class GraphicsResource
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        // ShaderResource 
        public const int ComponentMappingMask = 0x7;
        public const int ComponentMappingShift = 3;
        public const int ComponentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << (ComponentMappingShift * 4));

        protected GraphicsResource(GraphicsDevice device)
        {
            GraphicsDevice = device;
        }






        protected static int ComponentMapping(int src0, int src1, int src2, int src3) => ((src0) & ComponentMappingMask) |
            (((src1) & ComponentMappingMask) << ComponentMappingShift) |
            (((src2) & ComponentMappingMask) << (ComponentMappingShift * 2)) |
            (((src3) & ComponentMappingMask) << (ComponentMappingShift * 3)) |
            ComponentMappingAlwaysSetBitAvoidingZeromemMistakes;

        protected static int DefaultComponentMapping() => ComponentMapping(0, 1, 2, 3);

        protected static int ComponentMapping(int ComponentToExtract, int Mapping) => Mapping >> (ComponentMappingShift * ComponentToExtract) & ComponentMappingMask;
        
    }
}
