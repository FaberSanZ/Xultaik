#include "GameBase.h"

#include <chrono>
#include <thread>

namespace Xultaik
{
    GameBase::GameBase(const GameBaseSettings& settings)
        : m_window(settings.window),
          m_renderSystem(m_window, settings.render)
    {
    }

    int GameBase::Run()
    {
        OnInitialize();
        m_window.Show();

        using clock = std::chrono::steady_clock;
        auto previous = clock::now();

        while (!m_window.ShouldClose())
        {
            const auto current = clock::now();
            const std::chrono::duration<double> delta = current - previous;
            previous = current;

            m_window.PollEvents();
            OnUpdate(delta.count());

            GetRenderSystem().BeginFrame();
            OnRender();
            GetRenderSystem().EndFrame();

            std::this_thread::sleep_for(std::chrono::milliseconds(1));
        }

        return 0;
    }

    void GameBase::OnInitialize()
    {
    }

    void GameBase::OnUpdate(double deltaSeconds)
    {
        (void)deltaSeconds;
    }

    void GameBase::OnRender()
    {
    }

    Desktop::Window& GameBase::GetWindow() noexcept
    {
        return m_window;
    }

    const Desktop::Window& GameBase::GetWindow() const noexcept
    {
        return m_window;
    }

    RenderSystem& GameBase::GetRenderSystem() noexcept
    {
        return m_renderSystem;
    }

    const RenderSystem& GameBase::GetRenderSystem() const noexcept
    {
        return m_renderSystem;
    }
}
