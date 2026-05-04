#include "Window.h"

#include <stdexcept>

namespace Xultaik::Desktop
{
    Window::Window(const WindowSettings& settings)
    {
        m_instance = GetModuleHandleW(nullptr);
        RegisterClass();
        CreateNativeWindow(settings);
    }

    Window::~Window()
    {
        if (m_hwnd != nullptr)
        {
            DestroyWindow(m_hwnd);
            m_hwnd = nullptr;
        }
    }

    HWND Window::NativeHandle() const noexcept
    {
        return m_hwnd;
    }

    bool Window::ShouldClose() const noexcept
    {
        return m_shouldClose;
    }

    SIZE Window::ClientSize() const noexcept
    {
        RECT rect{};
        if (m_hwnd == nullptr || !GetClientRect(m_hwnd, &rect))
        {
            return SIZE{0, 0};
        }

        return SIZE{rect.right - rect.left, rect.bottom - rect.top};
    }

    void Window::Show(int command) const
    {
        if (m_hwnd != nullptr)
        {
            ShowWindow(m_hwnd, command);
            UpdateWindow(m_hwnd);
        }
    }

    void Window::SetTitle(std::wstring_view title)
    {
        if (m_hwnd != nullptr)
        {
            SetWindowTextW(m_hwnd, std::wstring(title).c_str());
        }
    }

    bool Window::PollEvents()
    {
        MSG message{};

        while (PeekMessageW(&message, nullptr, 0, 0, PM_REMOVE))
        {
            if (message.message == WM_QUIT)
            {
                m_shouldClose = true;
                break;
            }

            TranslateMessage(&message);
            DispatchMessageW(&message);
        }

        return !m_shouldClose;
    }

    void Window::RegisterClass()
    {
        m_className = L"XultaikWindowClass";

        WNDCLASSEXW window_class{};
        window_class.cbSize = sizeof(window_class);
        window_class.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC;
        window_class.lpfnWndProc = Window::WndProc;
        window_class.hInstance = m_instance;
        window_class.hCursor = LoadCursorW(nullptr, reinterpret_cast<LPCWSTR>(IDC_ARROW));
        window_class.lpszClassName = m_className.c_str();

        const ATOM atom = RegisterClassExW(&window_class);
        if (atom == 0)
        {
            throw std::runtime_error("RegisterClassExW failed.");
        }
    }

    void Window::CreateNativeWindow(const WindowSettings& settings)
    {
        RECT rect{0, 0, settings.width, settings.height};
        AdjustWindowRectEx(&rect, WS_OVERLAPPEDWINDOW, FALSE, WS_EX_APPWINDOW);

        m_hwnd = CreateWindowExW(
            WS_EX_APPWINDOW,
            m_className.c_str(),
            settings.title.c_str(),
            WS_OVERLAPPEDWINDOW,
            settings.x,
            settings.y,
            rect.right - rect.left,
            rect.bottom - rect.top,
            nullptr,
            nullptr,
            m_instance,
            this
        );

        if (m_hwnd == nullptr)
        {
            throw std::runtime_error("CreateWindowExW failed.");
        }
    }

    LRESULT CALLBACK Window::WndProc(HWND hwnd, UINT message, WPARAM wparam, LPARAM lparam)
    {
        Window* window = reinterpret_cast<Window*>(GetWindowLongPtrW(hwnd, GWLP_USERDATA));

        if (message == WM_NCCREATE)
        {
            const auto* create = reinterpret_cast<CREATESTRUCTW*>(lparam);
            window = reinterpret_cast<Window*>(create->lpCreateParams);
            SetWindowLongPtrW(hwnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(window));
            window->m_hwnd = hwnd;
        }

        if (window != nullptr)
        {
            switch (message)
            {
            case WM_CLOSE:
                window->m_shouldClose = true;
                DestroyWindow(hwnd);
                return 0;
            case WM_DESTROY:
                window->m_hwnd = nullptr;
                PostQuitMessage(0);
                return 0;
            default:
                break;
            }
        }

        return DefWindowProcW(hwnd, message, wparam, lparam);
    }
}
