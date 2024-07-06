// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Xultaik.Graphics
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
