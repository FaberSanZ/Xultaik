using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _00__First_Window
{
    class CoreEngine : IDisposable
    {
        public Input Input { get; set; }
        public Windows Windows { get; set; }


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



            if (Input == null)
            {
                Input = new Input();
                Input.Initialize();
                Input.CreateInput(Windows.Form);
            }
        }


        public void Run()
        {
            Windows.Run(render: Render, update: Update);
        }

        public void Render()
        {

        }



        public void Update()
        {
            // Check if the user pressed escape and wants to exit the application.
            if (Input.IsKeyDown(Keys.S))
            {
                Windows.AppPaused = true;
                Windows.Running = false;
            }
        }



        public void Dispose()
        {

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
