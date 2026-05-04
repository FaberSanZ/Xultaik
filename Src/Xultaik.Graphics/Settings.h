#pragma once

#include "RenderDescriptor.h"

#include <string>

namespace Xultaik::Graphics
{
    struct Settings
    {
        std::wstring title = L"Xultaik";
        RenderDescriptor render;
    };
}
