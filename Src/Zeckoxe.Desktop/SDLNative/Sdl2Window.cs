using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeckoxe.Graphics;
using static Zeckoxe.Sdl2.Sdl2Native;

namespace Zeckoxe.Sdl2
{
    //public struct WindowCreateInfo
    //{
    //    public int X;
    //    public int Y;
    //    public int WindowWidth;
    //    public int WindowHeight;
    //    public WindowState WindowInitialState;
    //    public string WindowTitle;

    //    public WindowCreateInfo(
    //        int x,
    //        int y,
    //        int windowWidth,
    //        int windowHeight,
    //        WindowState windowInitialState,
    //        string windowTitle)
    //    {
    //        X = x;
    //        Y = y;
    //        WindowWidth = windowWidth;
    //        WindowHeight = windowHeight;
    //        WindowInitialState = windowInitialState;
    //        WindowTitle = windowTitle;
    //    }
    //}






    public struct MouseState
    {
        public readonly int X;
        public readonly int Y;

        private readonly bool _mouseDown0;
        private readonly bool _mouseDown1;
        private readonly bool _mouseDown2;
        private readonly bool _mouseDown3;
        private readonly bool _mouseDown4;
        private readonly bool _mouseDown5;
        private readonly bool _mouseDown6;
        private readonly bool _mouseDown7;
        private readonly bool _mouseDown8;
        private readonly bool _mouseDown9;
        private readonly bool _mouseDown10;
        private readonly bool _mouseDown11;
        private readonly bool _mouseDown12;

        public MouseState(
            int x, int y,
            bool mouse0, bool mouse1, bool mouse2, bool mouse3, bool mouse4, bool mouse5, bool mouse6,
            bool mouse7, bool mouse8, bool mouse9, bool mouse10, bool mouse11, bool mouse12)
        {
            X = x;
            Y = y;
            _mouseDown0 = mouse0;
            _mouseDown1 = mouse1;
            _mouseDown2 = mouse2;
            _mouseDown3 = mouse3;
            _mouseDown4 = mouse4;
            _mouseDown5 = mouse5;
            _mouseDown6 = mouse6;
            _mouseDown7 = mouse7;
            _mouseDown8 = mouse8;
            _mouseDown9 = mouse9;
            _mouseDown10 = mouse10;
            _mouseDown11 = mouse11;
            _mouseDown12 = mouse12;
        }

        public bool IsButtonDown(MouseButton button)
        {
            uint index = (uint)button;
            switch (index)
            {
                case 0:
                    return _mouseDown0;
                case 1:
                    return _mouseDown1;
                case 2:
                    return _mouseDown2;
                case 3:
                    return _mouseDown3;
                case 4:
                    return _mouseDown4;
                case 5:
                    return _mouseDown5;
                case 6:
                    return _mouseDown6;
                case 7:
                    return _mouseDown7;
                case 8:
                    return _mouseDown8;
                case 9:
                    return _mouseDown9;
                case 10:
                    return _mouseDown10;
                case 11:
                    return _mouseDown11;
                case 12:
                    return _mouseDown12;
            }

            throw new ArgumentOutOfRangeException(nameof(button));
        }
    }

    public struct MouseWheelEventArgs
    {
        public MouseState State { get; }
        public float WheelDelta { get; }
        public MouseWheelEventArgs(MouseState mouseState, float wheelDelta)
        {
            State = mouseState;
            WheelDelta = wheelDelta;
        }
    }

    public struct MouseMoveEventArgs
    {
        public MouseState State { get; }
        public Vector2 MousePosition { get; }
        public MouseMoveEventArgs(MouseState mouseState, Vector2 mousePosition)
        {
            State = mouseState;
            MousePosition = mousePosition;
        }
    }

    [DebuggerDisplay("{DebuggerDisplayString,nq}")]
    public class BufferedValue<T> where T : struct
    {
        public T Value
        {
            get => Current.Value;
            set
            {
                Back.Value = value;
                Back = Interlocked.Exchange(ref Current, Back);
            }
        }

        private ValueHolder Current = new ValueHolder();
        private ValueHolder Back = new ValueHolder();

        public static implicit operator T(BufferedValue<T> bv) => bv.Value;

        private string DebuggerDisplayString => $"{Current.Value}";

        private class ValueHolder
        {
            public T Value;
        }
    }

    public delegate void SDLEventHandler(ref SDL_Event ev);
}
