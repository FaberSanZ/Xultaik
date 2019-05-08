using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _13__Framework
{
    class CoreEngine : IDisposable
    {
        public Input Input { get; private set; }
        public Graphics Graphics { get; private set; }
        public Windows Windows { get; private set; }


        public CoreEngine() { }

        public void Initialize()
        {
            if (Windows == null)
            {
                // Create the Windows object.
                Windows = new Windows();
                // Initialize the Windows object.
                Windows.Initialize();
            }



            if (Graphics == null)
            {
                Graphics = new Graphics();
                Graphics.Initialize(Windows.Form);
            }

            if (Input == null)
            {
                Input = new Input();
                Input.Initialize(Windows.Form);
            }
        }

        public void Start()
        {

            Windows.Run(Frame, Update);
        }

        public void Update()
        {
            // Check if the user pressed escape and wants to exit the application.
            if (Input.IsKeyDown(Keys.Escape))
            {
                Windows.AppPaused = true;
                Windows.Running = false;
            }
            Graphics.Update(Windows.Form);
        }

        private void Frame()
        {
            // Do the frame processing for the graphics object.
            Graphics.Frame(Windows.Form);
        }

        public void Dispose()
        {
            if (Graphics != null)
            {
                Graphics.ShutDown();
                Graphics = null;
            }

            if (Windows != null)
            {
                Windows = null;
            }

            if (Input != null)
            {
                Input = null;
            }
        }
    }
}
