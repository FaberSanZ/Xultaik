// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Events.cs
=============================================================================*/



using Silk.NET.GLFW;
using System;
using System.Collections.Generic;

namespace Vultaik.Desktop
{
    public unsafe class Gamepad : IInputDevice, IDisposable
    {
        private const int GamepadButtonCount = 15;
        private const int GamepadThumbstickCount = 2;
        private const int GamepadTriggerCount = 2;
        private bool _connected;

        public Gamepad(int i)
        {
            bool hasState = GlfwProvider.GLFW.Value.GetGamepadState(i, out GamepadState state);

            Index = i;
            Buttons = new Button[GamepadButtonCount];
            Thumbsticks = new Thumbstick[GamepadThumbstickCount];
            Triggers = new Trigger[GamepadTriggerCount];

            _connected = hasState;

            for (int j = 0; j < GamepadButtonCount; j++)
            {
                ((Button[])Buttons)[j] = new Button
                    ((ButtonName)j, j, hasState && state.Buttons[j] == (int)InputAction.Press);
            }

            for (int j = 0; j < GamepadThumbstickCount; j++)
            {
                ((Thumbstick[])Thumbsticks)[j] = new Thumbstick(j, 0, 0);
            }

            for (int j = 0; j < GamepadTriggerCount; j++)
            {
                ((Trigger[])Triggers)[j] = new Trigger(j, 0);
            }
        }

        public string Name => GlfwProvider.GLFW.Value.GetGamepadName(Index) ?? "Gamepad (via GLFW)";
        public int Index { get; }
        public bool IsConnected => GlfwProvider.GLFW.Value.JoystickIsGamepad(Index);
        public IReadOnlyList<Button> Buttons { get; }
        public IReadOnlyList<Thumbstick> Thumbsticks { get; }
        public IReadOnlyList<Trigger> Triggers { get; }
        public IReadOnlyList<Monitor> VibrationMotors { get; } = new Monitor[0];
        public Deadzone Deadzone { get; set; }
        public event Action<Gamepad, Button>? ButtonDown;
        public event Action<Gamepad, Button>? ButtonUp;
        public event Action<Gamepad, Thumbstick>? ThumbstickMoved;
        public event Action<Gamepad, Trigger>? TriggerMoved;

        public void Update()
        {
            if (!GlfwProvider.GLFW.Value.GetGamepadState(Index, out GamepadState state))
            {
                // Detect when this gamepad disconnects
                if (_connected)
                {
                    OnConnectionChanged?.Invoke(this, false);
                    _connected = false;
                }

                return;
            }

            // Detect when this gamepad connects
            if (!_connected)
            {
                OnConnectionChanged?.Invoke(this, true);
                _connected = true;
            }

            // Buttons
            for (int i = 0; i < GamepadButtonCount; i++)
            {
                if ((Buttons[i].Pressed ? 1 : 0) != state.Buttons[i])
                {
                    (Buttons[i].Pressed ? ButtonUp : ButtonDown)?.Invoke
                        (this, ((Button[])Buttons)[i] = new Button((ButtonName)i, i, state.Buttons[i] == 1));
                }

                ((Button[])Buttons)[i] = new Button((ButtonName)i, i, state.Buttons[i] == 1);
            }

            // Left Thumbstick
            Thumbstick thumbstick0 = new Thumbstick(0, Deadzone.Apply(state.Axes[0]), Deadzone.Apply(state.Axes[1]));
            if (Thumbsticks[0].X != thumbstick0.X || Thumbsticks[0].Y != thumbstick0.Y)
            {
                ThumbstickMoved?.Invoke
                (
                    this,
                    thumbstick0
                );
            }

            ((Thumbstick[])Thumbsticks)[0] = thumbstick0;

            // Right Thumbstick
            Thumbstick thumbstick1 = new Thumbstick(1, Deadzone.Apply(state.Axes[2]), Deadzone.Apply(state.Axes[3]));
            if (Thumbsticks[1].X != thumbstick1.X || Thumbsticks[1].Y != thumbstick1.Y)
            {
                ThumbstickMoved?.Invoke
                (
                    this,
                    thumbstick1
                );
            }

            ((Thumbstick[])Thumbsticks)[1] = thumbstick1;

            // Left Trigger
            Trigger trigger0 = new Trigger(0, Deadzone.Apply(state.Axes[4]));
            if (Triggers[0].Position != trigger0.Position)
            {
                TriggerMoved?.Invoke(this, trigger0);
            }

            ((Trigger[])Triggers)[0] = trigger0;

            // Right Trigger
            Trigger trigger1 = new Trigger(1, Deadzone.Apply(state.Axes[5]));
            if (Triggers[1].Position != trigger1.Position)
            {
                TriggerMoved?.Invoke(this, trigger1);
            }

            ((Trigger[])Triggers)[1] = trigger1;
        }

        public void Dispose()
        {
        }

        public Action<IInputDevice, bool> OnConnectionChanged { get; set; }
    }
}
