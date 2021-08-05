// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.Text;


namespace Vultaik
{
    public static class DeviceLimits 
    {

        public static int DescriptorSets => 8;

        public static int Bindings => 16;

        public static  int BindingsBindlessVarying => 64 * 1024;

        public static int BindingsBindless => 4 * 1024;

        public static int Attachments => 8;

        public static int VertexAttribs => 16;

        public static int VertexBuffers => 4;

        public static int ConstantSize => 128;

        public static int MaxUboSize => 16 * 1024;

        public static int SpecConstants => 8;

    }
}
