using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IzniteSoft.Desktop;

namespace _01__First_Window
{
    public class Core : IDisposable
    {

        public Window Windows { get; set; }
        public D3D11 D3D11 { get; set; }


        public Core() { }

        public void Initialize()
        {
            if (Windows == null)
                // Create the Windows object.
                Windows = new Window();




            if (D3D11 == null)
            {
                // Create the D3D11 object.
                D3D11 = new D3D11();
                // Initialize the D3D11 object.
                D3D11.InitializeD3D11(Windows.Handle, Windows.ClientSize.Width, Windows.ClientSize.Height);
                D3D11.InitScene();
            }
        }


        public void Run()
        {
            RenderLoop.Run(Windows, Render);
        }


        public void Render()
        {
            Update();

            // Clear the buffer to begin the scene.
            D3D11.DrawScene(0.0f, 0.2f, 0.4f, 1.0f);

            // Present the rendered scene to the screen.
            D3D11.EndScene();
        }


        public void Update()
        {
            D3D11.UpdateScene();
        }


        public void Dispose()
        {

            if (Windows != null)
            {
                Windows.Dispose();
                Windows = null;
            }
        }
    }
}
