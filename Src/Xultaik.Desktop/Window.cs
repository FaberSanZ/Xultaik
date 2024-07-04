// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.UI.WindowsAndMessaging.PEEK_MESSAGE_REMOVE_TYPE;
using static Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_EX_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_STYLE;

namespace Xultaik.Desktop;

public unsafe class Window : IDisposable
{
    public static WindowProcessEvents.ExternalWindowDelegate? ExternalWindowEvents { get; set; }

    internal readonly WindowProcessEvents.WindowDelegate WindowDelegateEvent;
    internal HWND handler;

    private readonly RegisterParams registerParams;
    private MSG msg;

    private readonly TimerState _timerState = new TimerState();
    private readonly KeyboardState _keyboardState = new KeyboardState();
    private readonly MouseState _mouseState = new MouseState();

    private bool _disposed;
    private bool _ClosedWin;
    private bool _focus;
    private int _countTitle;
    private Point _wheel = Point.Empty;
    private MONITORINFO _MONITORINFO1;
    private WindowState _state;
    private WindowBorder _Border;
    private CursorMode _cursorMode;
    private bool _cursorLock;

    public Window(in WindowSettings settings)
    {
        WindowDelegateEvent = ProccessEvents;

        registerParams = RegisterWin32Class.Register(settings.Icon);



        var tempState = settings.State;
        var tempBorder = tempState is WindowState.FullScreen ? WindowBorder.Hidden : settings.Border;

        handler = PInvoke.CreateWindowEx(
            WS_EX_APPWINDOW,
            registerParams.HashName,
            settings.Title,
            tempState is WindowState.Normal ? GetStyleBorder(tempBorder) : GetStyleBorder(tempBorder) | GetStyleState(tempState),
            settings.Position.X,
            settings.Position.Y,
            settings.Size.Width,
            settings.Size.Height,
            default,
            default,
            registerParams.Handler,
            null
        );

        if (handler == IntPtr.Zero)
        {
            throw new Exception($"Failed to create window. Error::{Marshal.GetLastWin32Error()}");
        }

        WindowProcessEvents.Include(this);

        _MONITORINFO1.cbSize = (uint)Unsafe.SizeOf<MONITORINFO>();
        PInvoke.GetMonitorInfo(PInvoke.MonitorFromWindow(handler, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY), ref _MONITORINFO1);


        UpdateFrequency = settings.UpdateFrequency;
        Title = settings.Title;
        Size = settings.Size;
        Position = settings.Position;
        _cursorMode = settings.CursorMode;
        _state = tempState;
        _Border = tempBorder;
    }

    #region Props
    public WindowResourcePtr WindowHandler => new (this.handler.Value);
    public KeyboardState KeyboardState => this._keyboardState;
    public MouseState MouseState => this._mouseState;

    public bool Focused
    {
        get => _focus; 
        set
        {
            _focus = value;
            PInvoke.SetWindowPos(handler, HWND.HWND_TOP, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW | SWP_DRAWFRAME);
        }
    }
    
    public string Title
    {
        get
        {
            char[] Text = new char[_countTitle];
            fixed (char* Text_ptr = Text)
            {
                PInvoke.GetWindowText(handler, Text_ptr, _countTitle);
                return new string(Text_ptr);
            }
        }
        set
        {
            _countTitle = value.Length + 1;
            fixed (char* Text = value)
            {
                PInvoke.SetWindowText(handler, Text);
            }
        }
    }
    public Point MousePosition
    {
        get => this._mouseState.Position;
        set
        {
            this._mouseState.Position = value;
            PInvoke.SetCursorPos(value.X, value.Y);
        }
    }

    public Point PreviousMousePosition => _mouseState.PreviousPosition;

    public Point MouseDelta => _mouseState.Delta;

    public Point MouseWheel
    {
        get
        {
            Point currentWheel = _wheel;

            _wheel = new Point(0, 0);

            int wheelV = 0;
            switch (currentWheel.X)
            {
                case 360:
                    wheelV = 1;
                    break;
                case -360:
                    wheelV = -1;
                    break;
            }

            int wheelH = 0;
            switch (currentWheel.Y)
            {
                case 360:
                    wheelH = 1;
                    break;
                case -360:
                    wheelH = -1;
                    break;
            }

            return new Point(wheelV, wheelH);
        }
        set
        {
            _wheel = value;
        }
    }

    public Rectangle ClientRect
    {
        get
        {
            PInvoke.GetClientRect(handler, out var rect);
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

    public Size ClientSize
    {
        get
        {
            var rect = this.ClientRect;
            return new Size(rect.Right, rect.Bottom);
        }
    }

    public Size ClientPosition
    {
        get
        {
            var rect = this.ClientRect;
            return new Size(rect.Left, rect.Top);
        }
    }

    public float AspectRatio
    {
        get
        {
            var size = ClientSize;
            return size.Width / (float)size.Height;
        }
    }

    public Rectangle Rect
    {
        get
        {
            PInvoke.GetWindowRect(handler, out var rect);
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        set
        {
            PInvoke.SetWindowPos(handler, default, value.Left, value.Top, value.Right, value.Bottom,
                SWP_NOZORDER |
                SWP_SHOWWINDOW);
        }
    }

    public Point Position
    {
        get
        {
            var rect = Rect;
            return new Point(rect.Left, rect.Top);
        }
        set
        {
            PInvoke.SetWindowPos(handler, default, value.X, value.Y, 0, 0,
                SWP_NOSIZE |
                SWP_NOZORDER |
                SWP_SHOWWINDOW);
        }
    }

    public Size Size
    {
        get
        {
            var rect = this.ClientRect;
            return new Size(rect.Right, rect.Bottom);
        }
        set
        {
            PInvoke.SetWindowPos(handler, default, 0, 0, value.Width, value.Height,
                SWP_NOMOVE |
                SWP_NOZORDER |
                SWP_SHOWWINDOW);
        }
    }


    public CursorMode CursorMode
    {
        get => this._cursorMode;
        set
        {
            if(this._cursorMode != value)
            {
                this._cursorMode = value;

                // That's the only way for this shit to work
                this.AdjustCursorWin();
                this.AdjustDimensionsWin();
                this.AdjustCursorWin();
            }

        }
    }

    public Point Center
    {
        get
        {
            PInvoke.GetWindowRect(handler, out var rect);
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }

    public WindowState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (State != value)
            {
                _state = value;
                this.AdjustDimensionsWin();
                this.AdjustCursorWin();
            }
        }
    }

    public WindowBorder Border
    {
        get
        {
            return _Border;
        }
        set
        {
            if (Border != value)
            {
                _Border = value;
                this.AdjustDimensionsWin();
                this.AdjustCursorWin();
            }

        }
    }

    public bool IsRuning
    {
        get
        {
            unsafe
            {
                if (PInvoke.PeekMessage(out msg, default, 0, 0, PM_REMOVE) != false)
                {
                    fixed (MSG* ptr_mgs = &msg)
                    {
                        _ = PInvoke.DispatchMessage(ptr_mgs);
                        _ = PInvoke.TranslateMessage(ptr_mgs);
                    }

                    if (msg.message == PInvoke.WM_QUIT)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    /// <summary>
    /// Defines the update rate, for <c>variable</c> mode pass a <c>null</c> value or one that is less than 1.
    /// </summary>
    public double? UpdateFrequency
    {
        get => this._timerState.UpdateFrequency == 0.0 ? null : this._timerState.UpdateFrequency;
        set
        {
            if (value != null)
            {
                this._timerState.UpdateFrequency = (double)value;
            }
            else
            {
                this._timerState.UpdateFrequency = 0.0;
            }
        }
    }

    /// <summary>
    /// Get elapsed time since the previous Update call.
    /// </summary>
    public ulong ElapsedTicks => _timerState.ElapsedTicks;

    /// <summary>
    /// Get elapsed time since the previous Update call.
    /// </summary>
    public double ElapsedSeconds => _timerState.ElapsedSeconds;

    /// <summary>
    /// Get total time since the start of the program.
    /// </summary>
    public ulong TotalTicks => _timerState.TotalTicks;

    /// <summary>
    /// Get total time since the start of the program.
    /// </summary>
    public double TotalSeconds => _timerState.TotalSeconds;

    /// <summary>
    /// Get total number of updates since start of the program.
    /// </summary>
    public ulong FrameCount => _timerState.FrameCount;

    /// <summary>
    /// Get the current framerate.
    /// </summary>
    public uint FramesPerSecond => _timerState.FramesPerSecond;

    /// <summary>
    /// Gets a value indicating whether or not the time is in fixed mode.
    /// </summary>
    public bool UpdateIsVariable => _timerState.IsFixedTimeStep;

    #endregion

    #region Events
    public event Action<FrameEventArgs>? UpdateFrame;

    public event Action<FrameEventArgs>? RenderFrame;

    public event Action? UnLoad;

    public event Action<ResizeEventArgs>? Resize;

    public event Action<MoveEventArgs>? Move;

    public event Action<FocusEventArgs>? Focus;

    public event Action<KeyboardKeyEventArgs>? KeyDown;

    public event Action<KeyboardKeyEventArgs>? KeyUp;

    public event Action<MouseButtonEventArgs>? MouseDown;

    public event Action<MouseButtonEventArgs>? MouseUp;

    public event Action<MouseMoveEventArgs>? MouseMove;

    public event Action<MouseWheelEventArgs>? MouseWheelAction;

    protected virtual void OnUpdateFrame(FrameEventArgs eventArgs)
    {
        this.UpdateFrame?.Invoke(eventArgs);
    }
    protected virtual void OnRenderFrame(FrameEventArgs eventArgs)
    {
        this.RenderFrame?.Invoke(eventArgs);
    }
    protected virtual void OnUnload()
    {
        this.UnLoad?.Invoke();
    }
    protected virtual void OnResize(ResizeEventArgs eventArgs)
    {
        this.Resize?.Invoke(eventArgs);
    }
    protected virtual void OnMove(MoveEventArgs eventArgs)
    {
        this.Move?.Invoke(eventArgs);
    }
    protected virtual void OnFocus(FocusEventArgs eventArgs)
    {
        this.Focus?.Invoke(eventArgs);
    }
    protected virtual void OnKeyboardKeyDown(KeyboardKeyEventArgs eventArgs)
    {
        this.KeyDown?.Invoke(eventArgs);
    }
    protected virtual void OnKeyboardKeyUp(KeyboardKeyEventArgs eventArgs)
    {
        this.KeyUp?.Invoke(eventArgs);
    }
    protected virtual void OnMouseButtonDown(MouseButtonEventArgs eventArgs)
    {
        this.MouseDown?.Invoke(eventArgs);
    }
    protected virtual void OnMouseButtonUp(MouseButtonEventArgs eventArgs)
    {
        this.MouseUp?.Invoke(eventArgs);
    }
    protected virtual void OnMouseMove(MouseMoveEventArgs eventArgs)
    {
        this.MouseMove?.Invoke(eventArgs);
    }
    protected virtual void OnMouseWheel(MouseWheelEventArgs eventArgs)
    {
        this.MouseWheelAction?.Invoke(eventArgs);
    }
    #endregion

    #region Mouse and keys Inputs
    public bool IsKeyDown(Keys Key)
    {
        return _keyboardState.IsKeyDown(Key);
    }
    public bool IsKeyUp(Keys Key)
    {
        return _keyboardState.IsKeyUp(Key);

    }
    public bool IsKeyPress(Keys Key)
    {
        return _keyboardState.IsKeyPress(Key);
    }

    public bool IsMouseButtonDown(MouseButton MouseButton)
    {
        return _mouseState.IsButtonDown(MouseButton);
    }
    public bool IsMouseButtonUp(MouseButton MouseButton)
    {
        return _mouseState.IsButtonUp(MouseButton);
    }
    public bool IsMouseButtonPress(MouseButton MouseButton)
    {
        return _mouseState.IsButtonPress(MouseButton);
    }
    #endregion
    public void Show()
    {
        this.AdjustDimensionsWin();
        this.AdjustCursorWin();

        PInvoke.ShowWindow(handler, SHOW_WINDOW_CMD.SW_NORMAL);
        PInvoke.UpdateWindow(handler);
    }



    public void RenderLoop(Action render)
    {


        this.OnResize(new ResizeEventArgs(this.ClientSize));

        while (this.IsRuning)
        {
            render();
        }

        this.OnUnload();
    }



    public void Close()
    {
        if(!_ClosedWin)
        {
            PInvoke.PostQuitMessage(0);
            RegisterWin32Class.Unregister(this.registerParams);
            PInvoke.DestroyWindow(handler);
            WindowProcessEvents.Remove(this);
            this._ClosedWin = true;
        }
    }

    public void Dispose()
    {
        this.OnDispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {
        if(!this._disposed)
        {
            this.Close();
            this._disposed = true;
        }
    }

    private void ProccessEvents(HWND hWnd, uint message, WPARAM wParam, LPARAM lParam)
    {
        ExternalWindowEvents?.Invoke(&hWnd, message, (IntPtr)wParam.Value, lParam.Value);

        switch (message)
        {
            #region Window Resources
            case PInvoke.WM_CREATE:
                break;

            case PInvoke.WM_DESTROY:
                {
                    PInvoke.PostQuitMessage(0);
                }
                break;

            case PInvoke.WM_SIZE:
                {
                    if (wParam == PInvoke.SIZE_MINIMIZED)
                    {
                        this.OnResize(new ResizeEventArgs(Size.Empty));

                    }
                    else
                    {
                        this.OnResize(new ResizeEventArgs(new Size(Win32Helper.LOWORD((nuint)lParam.Value), Win32Helper.HIWORD((nuint)lParam.Value))));
                    }
                }
                break;

            case PInvoke.WM_MOVE:
                {
                    this.OnMove(new MoveEventArgs(this.Position));
                }
                break;


            case PInvoke.WM_SETFOCUS:
                {
                    _focus = true;
                    this.OnFocus(new FocusEventArgs(true));
                }
                break;

            case PInvoke.WM_KILLFOCUS:
                {
                    _focus = false;
                    this.OnFocus(new FocusEventArgs(false));
                }
                break;
            #endregion

            #region Keyboard
            case PInvoke.WM_KEYDOWN:
                {
                    var scanCode = (int)wParam.Value;
                    var args = new KeyboardKeyEventArgs(scanCode, InputAction.Press, false);

                    this._keyboardState[scanCode] = true;

                    this.OnKeyboardKeyDown(args);
                }
                break;

            case PInvoke.WM_KEYUP:
                {
                    var scanCode = (int)wParam.Value;
                    var args = new KeyboardKeyEventArgs(scanCode, InputAction.Release, false);

                    this._keyboardState[scanCode] = false;

                    this.OnKeyboardKeyUp(args);
                }
                break;

            case PInvoke.WM_SYSKEYDOWN:
                {
                    var scanCode = (int)wParam.Value;

                    this._keyboardState[Keys.Alt] = true;
                    this._keyboardState.AltPressed = true;

                    if((Keys)scanCode != Keys.Alt)
                    {
                        var args = new KeyboardKeyEventArgs(scanCode, InputAction.Press, true);
                        this._keyboardState[scanCode] = true;
                        this.OnKeyboardKeyDown(args);
                    }

                }
                break;
            case PInvoke.WM_SYSKEYUP:
                {
                    var scanCode = (int)wParam.Value;
                    this._keyboardState[Keys.Alt] = false;
                    this._keyboardState.AltPressed = false;

                    if ((Keys)scanCode != Keys.Alt)
                    {
                        var args = new KeyboardKeyEventArgs(scanCode, InputAction.Release, true);
                        this._keyboardState[scanCode] = false;
                        this.OnKeyboardKeyUp(args);
                    }
                }
                break;
            #endregion

            #region Mouse
            case PInvoke.WM_MOUSEMOVE:
                {
                    this._mouseState.PreviousPosition = this._mouseState.Position;
                    this._mouseState.Position = new Point(Win32Helper.GET_X_LPARAM(lParam), Win32Helper.GET_Y_LPARAM(lParam));
                    OnMouseMove(new MouseMoveEventArgs(this._mouseState.Position, this._mouseState.PreviousPosition));
                }
                break;

            case PInvoke.WM_MOUSEWHEEL:
                {
                    if (Win32Helper.GET_KEYSTATE_WPARAM(wParam) == Win32Helper.MK_SHIFT)
                    {
                        _wheel.X = Win32Helper.GET_WHEEL_DELTA_WPARAM(wParam);
                    }
                    else
                    {
                        _wheel.Y = Win32Helper.GET_WHEEL_DELTA_WPARAM(wParam);
                    }

                    var wheel = MouseWheel;

                    this._mouseState.Wheel = wheel;
                    this.OnMouseWheel(new MouseWheelEventArgs(wheel));
                }
                break;

            case PInvoke.WM_LBUTTONDOWN:
                {
                    var scanCode = (int)MouseButton.Button1;
                    var args = new MouseButtonEventArgs(scanCode, InputAction.Press);

                    _mouseState[scanCode] = true;

                    this.OnMouseButtonDown(args);
                }
                break;
            case PInvoke.WM_LBUTTONUP:
                {
                    var scanCode = (int)MouseButton.Button1;
                    var args = new MouseButtonEventArgs(scanCode, InputAction.Release);

                    _mouseState[scanCode] = false;

                    this.OnMouseButtonUp(args);
                }
                break;
            case PInvoke.WM_RBUTTONDOWN:
                {
                    var scanCode = (int)MouseButton.Button2;
                    var args = new MouseButtonEventArgs(scanCode, InputAction.Press);

                    _mouseState[scanCode] = true;

                    this.OnMouseButtonDown(args);
                }
                break;
            case PInvoke.WM_RBUTTONUP:
                {
                    var scanCode = (int)MouseButton.Button2;
                    var args = new MouseButtonEventArgs(scanCode, InputAction.Release);

                    _mouseState[scanCode] = false;

                    this.OnMouseButtonUp(args);
                }
                break;
            case PInvoke.WM_MBUTTONDOWN:
                {
                    var scanCode = (int)MouseButton.Button3;
                    var args = new MouseButtonEventArgs(scanCode, InputAction.Press);

                    _mouseState[scanCode] = true;

                    this.OnMouseButtonDown(args);
                }
                break;
            case PInvoke.WM_MBUTTONUP:
                {
                    var scanCode = (int)MouseButton.Button3;
                    var args = new MouseButtonEventArgs(scanCode, InputAction.Release);

                    _mouseState[scanCode] = false;

                    this.OnMouseButtonUp(args);
                }
                break;

            case PInvoke.WM_XBUTTONDOWN:
                switch (Win32Helper.GET_XBUTTON_WPARAM(wParam))
                {
                    case PInvoke.XBUTTON1:
                        {
                            var scanCode = (int)MouseButton.Button5;
                            var args = new MouseButtonEventArgs(scanCode, InputAction.Press);

                            _mouseState[scanCode] = true;

                            this.OnMouseButtonDown(args);
                        }
                        break;

                    case PInvoke.XBUTTON2:
                        {
                            var scanCode = (int)MouseButton.Button4;
                            var args = new MouseButtonEventArgs(scanCode, InputAction.Press);

                            _mouseState[scanCode] = true;

                            this.OnMouseButtonDown(args);
                        }
                        break;
                }
                break;

            case PInvoke.WM_XBUTTONUP:
                switch (Win32Helper.GET_XBUTTON_WPARAM(wParam))
                {
                    case PInvoke.XBUTTON1:
                        {
                            var scanCode = (int)MouseButton.Button5;
                            var args = new MouseButtonEventArgs(scanCode, InputAction.Release);

                            _mouseState[scanCode] = false;

                            this.OnMouseButtonUp(args);
                        }
                        break;

                    case PInvoke.XBUTTON2:
                        {
                            var scanCode = (int)MouseButton.Button4;
                            var args = new MouseButtonEventArgs(scanCode, InputAction.Release);

                            _mouseState[scanCode] = false;

                            this.OnMouseButtonUp(args);
                        }
                        break;
                }
                break;
                #endregion
        }
    }

    #region Internals
    private void AdjustDimensionsWin()
    {
        switch (State)
        {
            case WindowState.Normal:
                WINDOW_STYLE NorStyle = WS_VISIBLE | GetStyleBorder(Border);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)NorStyle);
                PInvoke.SetWindowPos(handler, default, 0, 0, 0, 0,
                    SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;

            case WindowState.Minimized:

                WINDOW_STYLE MinStyle = WS_VISIBLE | GetStyleState(State) | GetStyleBorder(Border);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)MinStyle);
                PInvoke.SetWindowPos(handler, default, 0, 0, 0, 0,
                     SWP_NOZORDER | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;

            case WindowState.Maximixed:

                WINDOW_STYLE MaxStyle = WS_VISIBLE | GetStyleState(State) | GetStyleBorder(Border);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)MaxStyle);
                PInvoke.SetWindowPos(handler, HWND.HWND_TOP,
                    _MONITORINFO1.rcMonitor.left, _MONITORINFO1.rcMonitor.top,
                    _MONITORINFO1.rcMonitor.right - _MONITORINFO1.rcMonitor.left,
                    _MONITORINFO1.rcMonitor.bottom - _MONITORINFO1.rcMonitor.top,
                    SWP_NOZORDER | SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;

            case WindowState.FullScreen:
                _Border = WindowBorder.Hidden;
                WINDOW_STYLE FullScreenStyle = WS_VISIBLE | GetStyleState(State);
                PInvoke.SetWindowLong(handler, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)FullScreenStyle);
                PInvoke.SetWindowPos(handler, HWND.HWND_TOP,
                    _MONITORINFO1.rcMonitor.left, _MONITORINFO1.rcMonitor.top,
                    _MONITORINFO1.rcMonitor.right - _MONITORINFO1.rcMonitor.left,
                    _MONITORINFO1.rcMonitor.bottom - _MONITORINFO1.rcMonitor.top,
                    SWP_NOZORDER | SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_DRAWFRAME);
                break;
        }

    }
    private void AdjustCursorWin()
    {
        switch (this.CursorMode)
        {
            case CursorMode.Normal:
                {
                    RECT? rect = null;
                    PInvoke.ClipCursor(rect);
                    PInvoke.ShowCursor(true);
                    this._cursorLock = false;
                    break;
                }
            case CursorMode.Hidden:
                {
                    RECT? rect = null;
                    PInvoke.ClipCursor(rect);
                    PInvoke.ShowCursor(false);
                    this._cursorLock = false;
                    break;
                }
            case CursorMode.GrabbedWindow:
                {
                    PInvoke.GetWindowRect(handler, out var rect);
                    PInvoke.ClipCursor(rect);
                    PInvoke.ShowCursor(true);
                    this._cursorLock = false;
                    break;
                }
            case CursorMode.HiddenGrabbedWindow:
                {
                    PInvoke.GetWindowRect(handler, out var rect);
                    PInvoke.ClipCursor(rect);
                    PInvoke.ShowCursor(false);
                    this._cursorLock = false;
                    break;
                }
            case CursorMode.HiddenGrabbedCenter:
                {
                    PInvoke.ShowCursor(false);

                    RECT? rectEmpty = null;
                    PInvoke.ClipCursor(rectEmpty);

                    var pos = Center;
                    this.MousePosition = pos;

                    this._cursorLock = true;
                    break;
                }
        }

    }
    private static WINDOW_STYLE GetStyleState(WindowState windowState)
    {
        return windowState switch
        {
            WindowState.Maximixed => WS_MAXIMIZE,
            WindowState.Minimized => WS_MINIMIZE,
            WindowState.FullScreen => WS_POPUP,
            _ => throw new Exception()
        };

    }
    private static WINDOW_STYLE GetStyleBorder(WindowBorder windowBorder)
    {
        return windowBorder switch
        {
            WindowBorder.Hidden => WS_OVERLAPPEDWINDOW | WS_SIZEBOX,
            WindowBorder.Resizable => WS_OVERLAPPEDWINDOW,
            WindowBorder.Fixed => WS_BORDER,
            _ => throw new Exception()
        };

    }

    private static void AccurateSleep(double seconds, int expectedSchedulerPeriod)
    {
        // FIXME: Make this a parameter?
        const double TOLERANCE = 0.02;

        long t0 = Stopwatch.GetTimestamp();
        long target = t0 + (long)(seconds * Stopwatch.Frequency);

        double ms = (seconds * 1000) - (expectedSchedulerPeriod * TOLERANCE);
        int ticks = (int)(ms / expectedSchedulerPeriod);
        if (ticks > 0)
        {
            Thread.Sleep(ticks * expectedSchedulerPeriod);
        }

        while (Stopwatch.GetTimestamp() < target)
        {
            Thread.Yield();
        }
    }
    #endregion
}
