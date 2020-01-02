// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	RenderLoop.cs
=============================================================================*/




using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Zeckoxe.Desktop
{
    public unsafe class RenderLoop : IDisposable
    {
        internal IntPtr controlHandle;
        internal Control control;
        internal bool isControlAlive;
        internal bool switchControl;


        public RenderLoop()
        {

        }



        public RenderLoop(Control control) => Control = control;


        public Control Control
        {
            get => control;

            set
            {
                if (control == value)
                    return;

                // Remove any previous control
                if (control != null && !switchControl)
                {
                    isControlAlive = false;
                    control.Disposed -= ControlDisposed;
                    controlHandle = IntPtr.Zero;
                }

                if (value != null && value.IsDisposed)
                    throw new InvalidOperationException("Control is already disposed");


                control = value;
                switchControl = true;
            }
        }


        public bool UseApplicationDoEvents { get; set; }

        public bool AllowWindowssKeys { get; set; }


        public bool NextFrame()
        {
            // Setup new control
            // TODO this is not completely thread-safe. We should use a lock to handle this correctly
            if (switchControl && control != null)
            {
                controlHandle = control.Handle;
                control.Disposed += ControlDisposed;
                isControlAlive = true;
                switchControl = false;
            }

            if (isControlAlive)
            {
                if (UseApplicationDoEvents)
                    // Revert back to Application.DoEvents in order to support Application.AddMessageFilter
                    // Seems that DoEvents is compatible with Mono unlike Application.Run that was not running
                    // correctly.
                    Application.DoEvents();


                else
                {
                    IntPtr localHandle = controlHandle;

                    if (localHandle != IntPtr.Zero)
                    {
                        // Previous code not compatible with Application.AddMessageFilter but faster then DoEvents
                        Win32Native.NativeMessage msg;

                        while (Win32Native.PeekMessage(&msg, (void*)0 , 0, 0, Win32Native.PM_REMOVE) != 0)
                        {
                            // NCDESTROY event?
                            if (msg.msg == 130)
                                isControlAlive = false;


                            Message message = new Message()
                            {
                                HWnd = (IntPtr)msg.handle,
                                LParam = (IntPtr)msg.lParam,
                                Msg = (int)msg.msg,
                                WParam = (IntPtr)/*(void*)*/msg.wParam
                            };


                            if (!Application.FilterMessage(ref message))
                            {
                                Win32Native.TranslateMessage(&msg);
                                Win32Native.DispatchMessage(&msg);
                            }
                        }
                    }
                }
            }

            return isControlAlive || switchControl;
        }

        private void ControlDisposed(object sender, EventArgs e) => isControlAlive = false;


        public void Dispose() => Control = null;

    }
}
