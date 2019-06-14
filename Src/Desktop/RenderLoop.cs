using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Desktop
{
    public class RenderLoop : IDisposable
    {
        private IntPtr controlHandle;
        private Control control;
        private bool isControlAlive;
        private bool switchControl;


        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMessageLoop"/> class.
        /// </summary>
        public RenderLoop()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMessageLoop"/> class.
        /// </summary>
        public RenderLoop(Control control) => Control = control;
        

        /// <summary>
        /// Gets or sets the control to associate with the current render loop.
        /// </summary>
        /// <value>The control.</value>
        /// <exception cref="System.InvalidOperationException">Control is already disposed</exception>
        public Control Control
        {
            get => control;
            
            set
            {
                if (control == value) return;

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

        /// <summary>
        /// Gets or sets a value indicating whether the render loop should use the default <see cref="Application.DoEvents"/> instead of a custom window message loop lightweight for GC. Default is false.
        /// </summary>
        /// <value><c>true</c> if the render loop should use the default <see cref="Application.DoEvents"/> instead of a custom window message loop (default false); otherwise, <c>false</c>.</value>
        /// <remarks>By default, RenderLoop is using a custom window message loop that is more lightweight than <see cref="Application.DoEvents" /> to process windows event message. 
        /// Set this parameter to true to use the default <see cref="Application.DoEvents"/>.</remarks>
        public bool UseApplicationDoEvents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow windowss keys].
        /// </summary>
        /// <value><c>true</c> if [allow windowss keys]; otherwise, <c>false</c>.</value>
        public bool AllowWindowssKeys { get; set; }

        /// <summary>
        /// Calls this method on each frame.
        /// </summary>
        /// <returns><c>true</c> if if the control is still active, <c>false</c> otherwise.</returns>
        /// <exception cref="System.InvalidOperationException">An error occurred </exception>
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
                        while (Win32Native.PeekMessage(out msg, IntPtr.Zero, 0, 0, Win32Native.PM_REMOVE) != 0)
                        {
                            // NCDESTROY event?
                            if (msg.msg == 130)
                                isControlAlive = false;


                            Message message = new Message()
                            {
                                HWnd = msg.handle,
                                LParam = msg.lParam,
                                Msg = (int)msg.msg,
                                WParam = msg.wParam
                            };


                            if (!Application.FilterMessage(ref message))
                            {
                                Win32Native.TranslateMessage(ref msg);
                                Win32Native.DispatchMessage(ref msg);
                            }
                        }
                    }
                }
            }

            return isControlAlive || switchControl;
        }

        private void ControlDisposed(object sender, EventArgs e) => isControlAlive = false;
        

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => Control = null;
        



        /// <summary>
        /// Runs the specified main loop in the specified context.
        /// </summary>
        public static void Run(ApplicationContext context, Action renderCallback) => Run(context.MainForm, renderCallback);
        

        /// <summary>
        /// Runs the specified main loop for the specified windows form.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="renderCallback">The rendering callback.</param>
        /// <param name="useApplicationDoEvents">if set to <c>true</c> indicating whether the render loop should use the default <see cref="Application.DoEvents"/> instead of a custom window message loop lightweight for GC. Default is false.</param>
        /// <exception cref="System.ArgumentNullException">form
        /// or
        /// renderCallback</exception>
        public static void Run(Control Windows, Action renderCallback, bool useApplicationDoEvents = false)
        {
            if (Windows == null)
                throw new ArgumentNullException("form");

            if (renderCallback == null)
                throw new ArgumentNullException("renderCallback");

            Windows.Show();

            using (var renderLoop = new RenderLoop(Windows) { UseApplicationDoEvents = useApplicationDoEvents })
                while (renderLoop.NextFrame())
                    renderCallback();
                
        }


    }
}
