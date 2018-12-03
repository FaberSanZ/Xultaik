using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _04__Color_
{
    public class Core : IDisposable
    {
        public Input Input { get; set; }
        public Windows Windows { get; set; }
        public D3D11 D3D11 { get; set; }


        public Core() { }

        public void Initialize()
        {
            if (Windows == null)
            {
                // Create the Windows object.
                Windows = new Windows("Color! ", 720, 510);
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


            if (D3D11 == null)
            {
                // Create the D3D11 object.
                D3D11 = new D3D11();
                // Initialize the D3D11 object.
                D3D11.InitializeD3D11(Windows.Form);
                D3D11.InitScene();
                D3D11.Model();
            }
        }


        public void Run()
        {
            Windows.Run(render: Render, update: Update);
        }


        public void Render()
        {
            // Clear the buffer to begin the scene.
            D3D11.DrawScene();

            // Present the rendered scene to the screen.
            D3D11.EndScene();
        }


        public void Update()
        {
            D3D11.UpdateScene();


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
