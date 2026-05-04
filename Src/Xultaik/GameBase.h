#pragma once

#include "RenderSystem.h"

#include "../Xultaik.Desktop/Window.h"
#include "../Xultaik.Graphics/RenderDescriptor.h"

namespace Xultaik
{
    struct GameBaseSettings
    {
        Desktop::WindowSettings window;
        Graphics::RenderDescriptor render;
    };

    class GameBase
    {
    public:
        explicit GameBase(const GameBaseSettings& settings = {});
        virtual ~GameBase() = default;

        GameBase(const GameBase&) = delete;
        GameBase& operator=(const GameBase&) = delete;

        int Run();

    protected:
        virtual void OnInitialize();
        virtual void OnUpdate(double deltaSeconds);
        virtual void OnRender();

        Desktop::Window& GetWindow() noexcept;
        const Desktop::Window& GetWindow() const noexcept;

        Xultaik::RenderSystem& GetRenderSystem() noexcept;
        const Xultaik::RenderSystem& GetRenderSystem() const noexcept;

    private:
        Desktop::Window m_window;
        RenderSystem m_renderSystem;
    };
}
