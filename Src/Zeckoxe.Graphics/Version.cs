// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Version.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class Version
    {
        private readonly uint value;

        public Version(uint major, uint minor, uint patch)
        {
            value = major << 22 | minor << 12 | patch;
        }

        public uint Major => value >> 22;

        public uint Minor => (value >> 12) & 0x3ff;

        public uint Patch => (value >> 22) & 0xfff;

        public static implicit operator uint(Version version)
        {
            return version.value;
        }

    }
}
