using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public enum ValidationType
    {
        None = 1 << 0,
        Default = 1 << 1,
        Console = 1 << 2,
        ImGui = 1 << 3,
        Debug = 1 << 4,

    }
}
