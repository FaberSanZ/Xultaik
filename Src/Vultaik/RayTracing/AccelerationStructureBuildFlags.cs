// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public enum AccelerationStructureBuildFlags
    {
        None = 0,

        AllowUpdate = 0x1,

        AllowCompaction = 0x2,

        PreferFastTrace = 0x4,

        PreferFastBuild = 0x8,

        MinimizeMemory = 0x10,

        PerformUpdate = 0x20,
    }
}
