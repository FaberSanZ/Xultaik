// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Mouse.cs
=============================================================================*/



using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vultaik.Desktop
{
    public class Mouse : ISubscriber, IDisposable
    {
        private static readonly MouseButton[] _buttons = ((MouseButton[])Enum.GetValues(typeof(MouseButton)))
            .Where(x => x is not MouseButton.Unknown)
            .ToArray();

        private unsafe WindowHandle* _handle;
        private GlfwCallbacks.ScrollCallback? _scroll;
        private GlfwCallbacks.CursorPosCallback? _cursorPos;
        private GlfwCallbacks.MouseButtonCallback? _mouseButton;
        private bool _scrollModified = false;
        private Cursor? _cursor;

        public unsafe Mouse()
        {
            ScrollWheels = new ScrollWheel[1];
        }


        public int DoubleClickTime { get; set; } = 500;
        public int DoubleClickRange { get; set; } = 4;


        // Fields
        private MouseButton? _firstClickButton;
        private Vector2 _firstClickPosition = Vector2.Zero;
        private DateTime? _firstClickTime;
        private bool _firstClick = true;

        // Events
        public event Action<Mouse, MouseButton>? MouseDown;
        public event Action<Mouse, MouseButton>? MouseUp;
        public event Action<Mouse, MouseButton, Vector2>? Click;
        public event Action<Mouse, MouseButton, Vector2>? DoubleClick;

        public void HandleMouseDown(Mouse mouse, MouseButton button)
        {
            MouseDown?.Invoke(mouse, button);

            if (_firstClick || (_firstClickButton is not null && _firstClickButton != button))
            {
                // This is the first click with the given mouse button.
                _firstClickTime = null;

                if (!_firstClick && !(_firstClickButton is null))
                {
                    // Only the mouse buttons differ so treat last click as a single click.
                    Click?.Invoke(mouse, _firstClickButton.Value, Position);
                }

                ProcessFirstClick(button);
            }
            else
            {
                // This is the second click with the same mouse button.
                if (_firstClickTime != null &&
                    (DateTime.Now - _firstClickTime.Value).TotalMilliseconds <= DoubleClickTime)
                {
                    // Within the maximum double click time.
                    _firstClickTime = null;

                    Vector2 position = Position;
                    if (Math.Abs(position.X - _firstClickPosition.X) < DoubleClickRange &&
                        Math.Abs(position.Y - _firstClickPosition.Y) < DoubleClickRange)
                    {
                        // Second click was in time and in range -> double click.
                        _firstClick = true;
                        DoubleClick?.Invoke(mouse, button, position);
                    }
                    else
                    {
                        // Second click was in time but outside range -> single click.
                        // The second click is another "first click".
                        Click?.Invoke(mouse, button, position);
                        ProcessFirstClick(button);
                    }
                }
                else
                {
                    // The double click time elapsed.

                    // If Update() would have detected the time elapse before,
                    // it would have set _firstClick back to true and we won't be here.
                    // Therefore Update() has not detected time elapse here and we have
                    // to handle it.
                    HandleDoubleClickTimeElapse();

                    // Now process the second click as another "first click".
                    ProcessFirstClick(button);
                }
            }
        }

        protected void ProcessFirstClick(MouseButton button)
        {
            _firstClick = false;
            _firstClickButton = button;
            _firstClickPosition = Position;
            _firstClickTime = DateTime.Now;
        }

        public void HandleDoubleClickTimeElapse()
        {
            _firstClickTime = null;
            _firstClick = true;
            if (!(_firstClickButton is null))
            {
                Click?.Invoke(this, _firstClickButton.Value, Position);
            }
        }

        protected void HandleUpdate()
        {
            if (_firstClickTime is not null &&
                (DateTime.Now - _firstClickTime.Value).TotalMilliseconds > DoubleClickTime)
            {
                // No second click in maximum double click time.
                HandleDoubleClickTimeElapse();
            }
        }

        public void HandleMouseUp(Mouse mouse, MouseButton btn)
        {
            MouseUp?.Invoke(mouse, btn);
        }

        public string Name { get; } = "Mouse (via GLFW)";
        public int Index { get; } = 0;
        public bool IsConnected { get; } = true;
        public IReadOnlyList<MouseButton> SupportedButtons { get; } = _buttons;
        public IReadOnlyList<ScrollWheel> ScrollWheels { get; }

        public unsafe Vector2 Position
        {
            get
            {
                GlfwProvider.GLFW.Value.GetCursorPos(_handle, out double x, out double y);
                return new Vector2((float)x, (float)y);
            }
            set => GlfwProvider.GLFW.Value.SetCursorPos(_handle, value.X, value.Y);
        }

        public Cursor Cursor => _cursor!;

        public unsafe bool IsButtonPressed(MouseButton button)
        {
            MouseButton index = GetButton(button);

            if (index == MouseButton.Unknown)
            {
                return false;
            }

            return GlfwProvider.GLFW.Value.GetMouseButton(_handle, (int)index) == (int)InputAction.Press;
        }

        public event Action<Mouse, Vector2>? MouseMove;
        public event Action<Mouse, ScrollWheel>? Scroll;

        public unsafe void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)_handle);
        }

        public unsafe void Subscribe(Events events)
        {
            _handle = events.Handle;
            events.Scroll += _scroll = (_, x, y) =>
            {
                ScrollWheel val = new ScrollWheel((float)x, (float)y);
                if (ScrollWheels[0].X != val.X || ScrollWheels[0].Y != val.Y)
                {
                    _scrollModified = true;
                }

                ((ScrollWheel[])ScrollWheels)[0] = val;
                Scroll?.Invoke(this, val);
            };
            events.CursorPos += _cursorPos = (_, x, y) => MouseMove?.Invoke(this, new Vector2((float)x, (float)y));
            events.MouseButton += _mouseButton = (_, btn, action, mods) =>
            {
                switch (action)
                {
                    case InputAction.Press:
                            HandleMouseDown(this, GetButton((MouseButton)btn));
                            break;

                    case InputAction.Release:
                            HandleMouseUp(this, GetButton((Vultaik.Desktop.MouseButton)btn));
                            break;
                }
            };
            _cursor = new Cursor(_handle);
        }

        public void Unsubscribe(Events events)
        {
            events.Scroll -= _scroll;
            events.CursorPos -= _cursorPos;
            events.MouseButton -= _mouseButton;
        }

        public void Update()
        {
            if (!_scrollModified)
            {
                ((ScrollWheel[])ScrollWheels)[0] = default;
            }

            _scrollModified = false;
            HandleUpdate();
        }

        private static MouseButton GetButton(Vultaik.Desktop.MouseButton btn)
        {
            return btn switch
            {
                MouseButton.Left => MouseButton.Left,
                MouseButton.Right => MouseButton.Right,
                MouseButton.Middle => MouseButton.Middle,
                MouseButton.Button4 => MouseButton.Button4,
                MouseButton.Button5 => MouseButton.Button5,
                MouseButton.Button6 => MouseButton.Button6,
                MouseButton.Button7 => MouseButton.Button7,
                MouseButton.Button8 => MouseButton.Button8,

                _ => MouseButton.Unknown
            };
        }
    }
}
