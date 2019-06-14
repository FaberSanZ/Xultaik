using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Desktop;


namespace Systems
{
    public class Core : IDisposable
    {


        public Windows Windows { get; set; }



        public RenderSystem RenderSystem { get; set; }



        public Core() { }

        public void Initialize()
        {

            if (Windows == null)
                // Create the Windows object.
                Windows = new Windows("First Window", 800, 600);
            




            if (RenderSystem == null)
                // Create the RenderSystem object.
                RenderSystem = new RenderSystem();
            

        }


        public void Run()
        {
            RenderLoop.Run(Windows, Render);
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
