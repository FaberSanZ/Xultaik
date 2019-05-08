using Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _02__Initialize_Direct3D_11
{
    public class Core : IDisposable
    {
        public Input Input { get; set; }

        public Windows Windows { get; set; }

        public PresentationParameters parameters { get; set; }

        public RenderSystem RenderSystem { get; set; }



        public Core() { }

        public void Initialize()
        {

            if (Windows == null)
            {
                // Create the Windows object.
                Windows = new Windows("Initialize Direct3D 11", 720, 510);
                // Initialize the Windows object.
                Windows.Initialize();
            }



            if (Input == null)
            {
                // Create the Input object.
                Input = new Input();
                // Initialize the Input object.
                Input.Initialize();
                Input.CreateInput(Windows.Form);
            }



            if (parameters == null)
            {
                // Create the PresentationParameters object.
                parameters = new PresentationParameters();

                parameters.Height = Windows.Height;
                parameters.Width = Windows.Width;
                parameters.Handle = Windows.Form.Handle;
                parameters.BackBufferFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            }



            if (RenderSystem == null)
            {
                // Create the RenderSystem object.
                RenderSystem = new RenderSystem(parameters);
            }


            Console.WriteLine(RenderSystem.Adapters[0].Description);
            Console.WriteLine(RenderSystem.Adapters[1].Description);
        }


        public void Run()
        {
            Windows.Run(Render);
        }


        public void Render()
        {
            Update();


            // Clear the buffer to begin the scene.
            RenderSystem.Draw();

            // Present the rendered scene to the screen.
            RenderSystem.End();
        }


        public void Update()
        {
            // Check if the user pressed escape and wants to exit the application.
            if (Input.IsKeyDown(Keys.Escape))
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
