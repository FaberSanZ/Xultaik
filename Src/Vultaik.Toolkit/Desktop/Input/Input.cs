// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Input.cs
=============================================================================*/

using Silk.NET.GLFW;
using System;
using System.Collections.Generic;


namespace Vultaik.Desktop
{
    public unsafe class Input
    {
        private readonly Gamepad[] _gamepads = new Gamepad[16];
        private readonly Joystick[] _joysticks = new Joystick[16];
        private readonly Keyboard[] _keyboards = new Keyboard[1];
        private readonly Mouse[] _mice = new Mouse[1];
        private readonly ISubscriber[] _subscribers = new ISubscriber[2];
        private readonly Action<double> _update;
        private readonly Window _window;
        private readonly Dictionary<IntPtr, Events> _subs = new Dictionary<IntPtr, Events>();


        public Input(Window window)
        {
            void OnConnectionChanged(IInputDevice a, bool b)
            {
                ConnectionChanged?.Invoke(a, b);
            }

            Handle = (IntPtr)window.pWindowHandle;

            for (int i = 0; i < _gamepads.Length; i++)
            {
                _gamepads[i] = new(i)
                {
                    OnConnectionChanged = OnConnectionChanged
                };
            }

            for (int i = 0; i < _joysticks.Length; i++)
            {
                _joysticks[i] = new(i)
                {
                    OnConnectionChanged = OnConnectionChanged
                };
            }

            _subscribers[0] = _keyboards[0] = new();
            _subscribers[1] = _mice[0] = new();

            Gamepads = new IsConnectedWrapper<Gamepad>(_gamepads);
            Joysticks = new IsConnectedWrapper<Joystick>(_joysticks);
            Keyboards = _keyboards;
            Mice = _mice;

            RegisterWindow((WindowHandle*)Handle, _subscribers);

            window.Update += _update = _ =>
            {
                foreach (Mouse updatable in _mice)
                {
                    updatable.Update();
                }

                foreach (Gamepad updatable in _gamepads)
                {
                    updatable.Update();
                }

                foreach (Joystick updatable in _joysticks)
                {
                    updatable.Update();
                }
            };

            _window = window;
        }



        /// <summary>
        /// A handle to the underlying window.
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// A list of all available gamepads.
        /// </summary>
        public IReadOnlyList<Gamepad> Gamepads { get; }

        /// <summary>
        /// A list of all available joysticks.
        /// </summary>
        public IReadOnlyList<Joystick> Joysticks { get; }

        /// <summary>
        /// A list of all available keyboards.
        /// <remarks>
        /// On some backends, this list may only contain 1 item. This is most likely because the underlying API doesn't
        /// support multiple keyboards.
        /// </remarks>
        /// </summary>
        public IReadOnlyList<Keyboard> Keyboards { get; }

        /// <summary>
        /// A list of all available mice.
        /// </summary>
        /// <remarks>
        /// On some backends, this list may only contain 1 item. This is most likely because the underlying API doesn't
        /// support multiple mice.
        /// </remarks>
        public IReadOnlyList<Mouse> Mice { get; }

        /// <summary>
        /// A list of all other available input devices.
        /// </summary>
        /// <remarks>
        /// On some backends, this list might be empty. This is most likely because the underlying API doesn't
        /// support other devices.
        /// </remarks>
        public IReadOnlyList<IInputDevice> OtherDevices { get; }

        /// <summary>
        /// Called when the connection status of a device changes.
        /// </summary>
        public event Action<IInputDevice, bool>? ConnectionChanged;








        internal void RegisterWindow(WindowHandle* handle, IEnumerable<ISubscriber> subscribers)
        {
            Events events = _subs.ContainsKey((IntPtr)handle) ? _subs[(IntPtr)handle] : _subs[(IntPtr)handle] = new Events(handle);

            foreach (ISubscriber subscriber in subscribers)
            {
                subscriber.Subscribe(events);
            }
        }

        internal void UnregisterWindow(WindowHandle* handle, IEnumerable<ISubscriber> subscribers)
        {
            Events events = _subs.ContainsKey((IntPtr)handle) ? _subs[(IntPtr)handle] : _subs[(IntPtr)handle] = new Events(handle);

            foreach (ISubscriber subscriber in subscribers)
            {
                subscriber.Unsubscribe(events);
            }
        }

        public void Dispose()
        {
            _window.Update -= _update;
            UnregisterWindow((WindowHandle*)Handle, _subscribers);

            foreach (Gamepad gamepad in _gamepads)
            {
                gamepad.Dispose();
            }

            foreach (Joystick joystick in _joysticks)
            {
                joystick.Dispose();
            }

            foreach (Mouse mouse in _mice)
            {
                mouse.Dispose();
            }
        }

    }
}
