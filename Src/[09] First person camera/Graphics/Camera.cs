using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _09__First_person_camera
{
    public class Camera
    {
        // Properties.
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }

        public Matrix View { get;  set; }
        public Matrix Projection { get;  set; }
        public Matrix World { get;  set; }

        // Constructor
        public Camera() { }


        // Methods.
        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }


        public void Initialize(Control Con)
        {
            // Setup and create the projection matrix.
            Projection = Matrix.PerspectiveFovLH((float)(Math.PI / 4), ((float)Con.ClientSize.Width / (float)Con.ClientSize.Height), 0, 1);

            // Initialize the world matrix to the identity matrix.
            World = Matrix.Identity;
        }
        

        public void Render()
        {
            // Setup the position of the camera in the world.
            Vector3 Position = new Vector3(PositionX, PositionY, PositionZ);

            // Setup where the camera is looking by default.
            Vector3 LookAt = Vector3.UnitZ;

            Vector3 Up = Vector3.UnitY;

            // Finally create the view matrix from the three updated vectors.
            View = Matrix.LookAtLH(Position, LookAt, Up);
        }
    }
}
