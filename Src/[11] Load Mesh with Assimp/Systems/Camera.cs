using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphics;
using SharpDX;


namespace Systems
{
    public class Camera
    {
        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }


        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }

        public CameraType Type { get; set; }

        public Camera(CameraType type)
        {
            Type = type;

            Position = new Vector3(0.0f, 0.9f, -2.6f);
            Target = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);

            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;


            //Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, 1.2f, 1.0f, 1000.0f);

        }


        public void SetLens(float Fov, float Aspect, float Zn, float Zf)
        {
            Projection = Matrix.PerspectiveFovLH(Fov, Aspect, Zn, Zf);
        }


        public void Update()
        {
            switch (Type)
            {
                case CameraType.Static:

                    //Set the View matrix
                    View = Matrix.LookAtLH(Position, Target, Up);
                    break;


                case CameraType.Firstperson:


                    break;


                case CameraType.Lookat:


                    break;


            }
        }
    }
}
