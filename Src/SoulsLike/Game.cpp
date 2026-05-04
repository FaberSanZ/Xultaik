#include "Game.h"

namespace SoulsLike
{
    namespace
    {
        Xultaik::GameBaseSettings make_settings()
        {
            Xultaik::GameBaseSettings settings{};
            settings.window.title = L"SoulsLike";
            settings.window.width = 1600;
            settings.window.height = 900;
            settings.render.width = 1600;
            settings.render.height = 900;
            settings.render.vsync = true;
            return settings;
        }
    }

    Game::Game() : GameBase(make_settings())
    {
    }

    void Game::OnInitialize()
    {
        auto& device = GetRenderSystem().GraphicsDevice();
        auto& swapChain = GetRenderSystem().SwapChain();
    }

    void Game::OnUpdate(double deltaSeconds)
    {
        m_elapsedTime += deltaSeconds;

        auto& device = GetRenderSystem().GraphicsDevice();
    }

    void Game::OnRender()
    {
        auto& swapChain = GetRenderSystem().SwapChain();
    }
}
