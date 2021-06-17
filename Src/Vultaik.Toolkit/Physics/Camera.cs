// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

using System;
using System.Numerics;

namespace Vultaik.Physics
{
    public class Camera
    {
        internal float fov, aspectRatio, zNear = 0.1f, zFar = 128f, zoom = 1.0f;
        internal float moveSpeed = 0.1f, rotSpeed = 0.01f, zoomSpeed = 0.01f;
        internal Vector3 rotation = Vector3.Zero;
        internal Vector3 position = Vector3.Zero;
        internal Matrix4x4 model = Matrix4x4.Identity;

        public Camera()
        {

        }



        public Camera(float fieldOfView, float aspectRatio, float nearPlane = 0.1f, float farPlane = 16f)
        {
            fov = DegreesToRadians(fieldOfView);
            this.aspectRatio = aspectRatio;
            zNear = nearPlane;
            zFar = farPlane;
            Update();
        }


        public Vector3 Position => position;
        public Vector3 Rotation => rotation;
        public float NearPlane => zNear;
        public float FarPlane => zFar;

        public CameraType Type { get; set; }
        public Matrix4x4 Projection { get; set; }
        public Matrix4x4 View { get; set; }

        public Matrix4x4 Perspective => Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, zNear, zFar);
        public Matrix4x4 SkyboxView => Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z) *
                                       Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                                       Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X);

        public float AspectRatio
        {
            get => aspectRatio; 
            set
            {
                aspectRatio = value;
                Update();
            }
        }

        public float FieldOfView
        {
            get => fov; 
            set
            {
                fov = value;
                Update();
            }
        }
        

        public float Zoom
        {
            get => zoom;

            set
            {
                zoom = value;
                Update();
            }
        }


        public Matrix4x4 Model
        {
            get => model; 
            set
            {
                model = value;
                Update();
            }
        }


            
        


        public float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        public void Rotate(float x, float y, float z = 0)
        {
            rotation.X += rotSpeed * x;
            rotation.Y += rotSpeed * y;
            rotation.Z += rotSpeed * z;
            Update();
        }

        public void SetRotation(float x, float y, float z = 0)
        {
            rotation.X = x;
            rotation.Y = y;
            rotation.Z = z;
            Update();
        }
        public void SetPosition(float x, float y, float z = 0)
        {
            position.X = x;
            position.Y = y;
            position.Z = z;
            Update();
        }
        public void Move(float x, float y, float z = 0)
        {
            position.X += moveSpeed * x;
            position.Y += moveSpeed * y;
            position.Z += moveSpeed * z;
            Update();
        }
        public void SetZoom(float factor)
        {
            zoom += zoomSpeed * factor;
            Update();
        }



        public Matrix4x4 CreatePerspectiveFieldOfView(float fov, float aspectRatio, float zNear, float zFar)
        {
            float f = (float)(1.0 / System.Math.Tan(0.5 * fov));
            return new Matrix4x4(
                f / aspectRatio, 0, 0, 0,
                0, -f, 0, 0,
                0, 0, zFar / (zNear - zFar), -1,
                0, 0, zNear * zFar / (zNear - zFar), 0
            );
        }

        public void Update()
        {
            Projection = CreatePerspectiveFieldOfView(fov, aspectRatio, zNear, zFar);

            Matrix4x4 translation = Matrix4x4.CreateTranslation(position * zoom);

            if (Type == CameraType.LookAt)
            {
                View =
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X) *
                        translation;
            }
            else
            {
                View = translation *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y) *
                        Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z);
            }
        }
    }
}
