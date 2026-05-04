#pragma once

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class GraphicsResource
    {
    public:
        explicit GraphicsResource(GraphicsDevice* graphicsDevice) noexcept;
        virtual ~GraphicsResource() = default;

        GraphicsResource(const GraphicsResource&) = delete;
        GraphicsResource& operator=(const GraphicsResource&) = delete;

    protected:
        GraphicsDevice* graphicsDevice() const noexcept;

    private:
        GraphicsDevice* graphicsDevice_;
    };
}
