#pragma once

namespace Xultaik::Graphics
{
    enum class BufferFlags : unsigned int
    {
        None = 0,
        ConstantBuffer = 1u << 0,
        VertexBuffer = 1u << 1,
        IndexBuffer = 1u << 2,
        Structured = 1u << 3
    };

    inline BufferFlags operator|(BufferFlags lhs, BufferFlags rhs)
    {
        return static_cast<BufferFlags>(static_cast<unsigned int>(lhs) | static_cast<unsigned int>(rhs));
    }

    inline bool operator&(BufferFlags lhs, BufferFlags rhs)
    {
        return (static_cast<unsigned int>(lhs) & static_cast<unsigned int>(rhs)) != 0;
    }
}
