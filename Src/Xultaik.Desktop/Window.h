#pragma once

#include <windows.h>

#include <string>
#include <string_view>

namespace Xultaik::Desktop
{
    enum class WindowState
    {
        Normal,
        Maximized,
        FullScreen
    };

    enum class WindowBorder
    {
        Resizable,
        Fixed,
        Hidden
    };

    struct WindowSettings
    {
        std::wstring title = L"Xultaik";
        int x = CW_USEDEFAULT;
        int y = CW_USEDEFAULT;
        int width = 1280;
        int height = 720;
        WindowState state = WindowState::Normal;
        WindowBorder border = WindowBorder::Resizable;
    };

    class Window
    {
    public:
        explicit Window(const WindowSettings& settings);
        ~Window();

        Window(const Window&) = delete;
        Window& operator=(const Window&) = delete;

        [[nodiscard]] HWND NativeHandle() const noexcept;
        [[nodiscard]] bool ShouldClose() const noexcept;
        [[nodiscard]] SIZE ClientSize() const noexcept;

        void Show(int command = SW_SHOWDEFAULT) const;
        void SetTitle(std::wstring_view title);
        bool PollEvents();

    private:
        static LRESULT CALLBACK WndProc(HWND hwnd, UINT message, WPARAM wparam, LPARAM lparam);
        void RegisterClass();
        void CreateNativeWindow(const WindowSettings& settings);

        HWND m_hwnd = nullptr;
        HINSTANCE m_instance = nullptr;
        std::wstring m_className;
        bool m_shouldClose = false;
    };
}
