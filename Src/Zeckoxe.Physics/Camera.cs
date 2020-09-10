// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. Faber.reach@gmail.com

/*===================================================================================
	Camera.cs
====================================================================================*/


using System;
using System.Numerics;
using Zeckoxe.Games;

namespace Zeckoxe.Physics
{
    // TODO: Camera
    public class Camera : IDisposable
    {
        private Vector3 position;
        private Vector3 interest;
        private float fieldOfView = MathUtil.PiOverFour;
        private float nearPlaneDistance = 1f;
        private float farPlaneDistance = 100f;
        private float aspectRelation = 1f;
        private float viewportWidth = 0f;
        private float viewportHeight = 0f;
        private CameraType mode;
        private IsometricAxis isometricAxis = IsometricAxis.NW;
        private Vector3 isoMetricForward = new Vector3(-1f, 0f, -1f);
        private Vector3 isoMetricBackward = new Vector3(1f, 0f, 1f);
        private Vector3 isoMetricLeft = new Vector3(1f, 0f, -1f);
        private Vector3 isoMetricRight = new Vector3(-1f, 0f, 1f);
        private CameraTranslations translationMode = CameraTranslations.None;
        private Vector3 translationInterest = Vector3.Zero;
        private readonly float translationRadius = 10f;
        private readonly float translationOutOfRadius = 0.2f;
        private readonly float translationIntoRadius = 0.01f;

        public Camera()
        {
            position = Vector3.One;
            interest = Vector3.Zero;

            View = Matrix4x4.CreateLookAt(position,interest,Vector3.UnitY);

            Projection = Matrix4x4.Identity;

            InvertY = false;
        }

        public Vector3 Forward
        {
            get
            {
                if (mode is CameraType.FreeIsometric)
                {
                    return isoMetricForward;
                }
                else if (mode is CameraType.Free)
                {
                    return Vector3.Normalize(Interest - Position);
                }
                else if (mode is CameraType.FirstPerson || mode is CameraType.ThirdPerson)
                {
                    Vector3 v = Interest - Position;

                    return Vector3.Normalize(new Vector3(v.X, 0, v.Z));
                }
                else
                {
                    return Vector3.Normalize(Interest - Position);
                }
            }
        }

        public Vector3 Backward
        {
            get
            {
                if (mode is CameraType.FreeIsometric)
                {
                    return isoMetricBackward;
                }
                else if (mode is CameraType.Free)
                {
                    return -Forward;
                }
                else if (mode is CameraType.FirstPerson || mode is CameraType.ThirdPerson)
                {
                    return -Forward;
                }
                else
                {
                    return -Forward;
                }
            }
        }

        public Vector3 Left
        {
            get
            {
                if (mode is CameraType.FreeIsometric)
                {
                    return isoMetricLeft;
                }
                else if (mode is CameraType.Free)
                {
                    return Vector3.Cross(Forward, Vector3.UnitY);
                }
                else if (mode is CameraType.FirstPerson || mode is CameraType.ThirdPerson)
                {
                    return Vector3.Cross(Forward, Vector3.UnitY);
                }
                else
                {
                    return Vector3.Cross(Forward, Vector3.UnitY);
                }
            }
        }

        public Vector3 Right
        {
            get
            {
                if (mode is CameraType.FreeIsometric)
                {
                    return isoMetricRight;
                }
                else if (mode is CameraType.Free)
                {
                    return -Left;
                }
                else if (mode is CameraType.FirstPerson || mode is CameraType.ThirdPerson)
                {
                    return -Left;
                }
                else
                {
                    return -Left;
                }
            }
        }

        public Vector3 Up
        {
            get
            {
                if (mode is CameraType.FreeIsometric)
                {
                    return new Vector3(0f, 1f, 0f);
                }
                else if (mode is CameraType.Free)
                {
                    return Vector3.Cross(Left, Forward);
                }
                else if (mode is CameraType.FirstPerson || mode is CameraType.ThirdPerson)
                {
                    return Vector3.UnitY;
                }
                else
                {
                    return Vector3.Cross(Left, Forward);
                }
            }
        }

        public Vector3 Down
        {
            get
            {
                if (mode is CameraType.FreeIsometric)
                {
                    return new Vector3(0f, -1f, 0f);
                }
                else if (mode is CameraType.Free)
                {
                    return -Up;
                }
                else if (mode is CameraType.FirstPerson || mode is CameraType.ThirdPerson)
                {
                    return -Up;
                }
                else
                {
                    return -Up;
                }
            }
        }

        public CameraType Mode
        {
            get => mode;
            set
            {
                mode = value;

                if (mode is CameraType.FreeIsometric)
                {
                    SetIsometric(IsometricAxis.SE, Vector3.Zero, ZoomMin * 2f);
                }
                else if (mode is CameraType.Free)
                {
                    SetFree(position, interest);
                }
                else if (mode is CameraType.Ortho)
                {
                    SetOrtho();
                }
            }
        }

        public Vector3 Position
        {
            get => Following?.Position ?? position;
            set => position = value;
        }

        public Vector3 Interest
        {
            get => Following?.Interest ?? interest;
            set => interest = value;
        }

        public float FieldOfView
        {
            get => fieldOfView;
            set
            {
                fieldOfView = value;

                UpdateLens();
            }
        }

        public float NearPlaneDistance
        {
            get => nearPlaneDistance;
            set
            {
                nearPlaneDistance = value;

                UpdateLens();
            }
        }

        public float FarPlaneDistance
        {
            get => farPlaneDistance;
            set
            {
                farPlaneDistance = value;

                UpdateLens();
            }
        }

        public float AspectRelation
        {
            get => aspectRelation;
            set
            {
                aspectRelation = value;

                UpdateLens();
            }
        }

        public Vector3 Velocity { get; private set; }

        public Vector3 Direction => Vector3.Normalize(Interest - Position);

        public float MovementDelta { get; set; } = 20.5f;

        public float SlowMovementDelta { get; set; } = 1.0f;

        public float RotationDelta { get; set; } = 0.25f;

        public float SlowRotationDelta { get; set; } = 0.10f;

        public float ZoomMax { get; set; } = 200f;

        public float ZoomMin { get; set; } = 15f;

        public float ZoomDelta { get; set; } = 100f;

        public float SlowZoomDelta { get; set; } = 40f;

        public Matrix4x4 View { get; private set; }

        public Matrix4x4 Projection { get; private set; }

        public IFollower Following { get; set; }

        public bool InvertY { get; set; }

        public float CameraRadius { get; set; } = 1f;

        public void SetLens(int width, int height)
        {
            viewportWidth = width;
            viewportHeight = height;

            aspectRelation = viewportWidth / viewportHeight;

            UpdateLens();
        }

        public void Update(GameTime gameTime)
        {
            Velocity = Vector3.Zero;

            UpdateTranslations(gameTime);

            if (mode == CameraType.Ortho)
            {
                View = Matrix4x4.CreateLookAt(
                    Position,
                    Interest,
                    Vector3.UnitZ);
            }
            else
            {
                View = Matrix4x4.CreateLookAt(
                    Position,
                    Interest,
                    Vector3.UnitY);
            }

            //Frustum = new BoundingFrustum(View * Projection);
        }

        public void PreviousIsometricAxis()
        {
            if (mode == CameraType.FreeIsometric)
            {
                int current = (int)isometricAxis;
                int previous;

                if (current <= 0)
                {
                    previous = 3;
                }
                else
                {
                    previous = current - 1;
                }

                SetIsometric((IsometricAxis)previous, Interest, Vector3.Distance(Position, Interest));
            }
        }

        public void NextIsometricAxis()
        {
            if (mode == CameraType.FreeIsometric)
            {
                int current = (int)isometricAxis;
                int next;

                if (current >= 3)
                {
                    next = 0;
                }
                else
                {
                    next = current + 1;
                }

                SetIsometric((IsometricAxis)next, Interest, Vector3.Distance(Position, Interest));
            }
        }

        public void MoveForward(GameTime gameTime, bool slow)
        {
            Move(gameTime, Forward, slow);
        }

        public void MoveBackward(GameTime gameTime, bool slow)
        {
            Move(gameTime, Backward, slow);
        }

        public void MoveLeft(GameTime gameTime, bool slow)
        {
            Move(gameTime, Left, slow);
        }

        public void MoveRight(GameTime gameTime, bool slow)
        {
            Move(gameTime, Right, slow);
        }

        public void RotateUp(GameTime gameTime, bool slow)
        {
            Rotate(gameTime, Left, slow);
        }

        public void RotateDown(GameTime gameTime, bool slow)
        {
            Rotate(gameTime, Right, slow);
        }

        public void RotateLeft(GameTime gameTime, bool slow)
        {
            Rotate(gameTime, Down, slow);
        }

        public void RotateRight(GameTime gameTime, bool slow)
        {
            Rotate(gameTime, Up, slow);
        }

        public void RotateMouse(GameTime gameTime, float deltaX, float deltaY)
        {
            if (deltaX != 0f)
            {
                Rotate(Up, gameTime.ElapsedSeconds * deltaX * 10f);
            }
            if (deltaY != 0f)
            {
                if (InvertY)
                {
                    deltaY = -deltaY;
                }

                Rotate(Left, gameTime.ElapsedSeconds * -deltaY * 10f, true, -85, 85);
            }
        }

        public void ZoomIn(GameTime gameTime, bool slow)
        {
            Zoom(gameTime, true, slow);
        }

        public void ZoomOut(GameTime gameTime, bool slow)
        {
            Zoom(gameTime, false, slow);
        }


        private void SetFree(Vector3 newPosition, Vector3 newInterest)
        {
            StopTranslations();

            Position = newPosition;
            Interest = newInterest;

            mode = CameraType.Free;
        }

        private void SetIsometric(IsometricAxis axis, Vector3 newInterest, float distance)
        {
            StopTranslations();

            Vector3 tmpPosition = Vector3.Zero;

            if (axis is IsometricAxis.NW)
            {
                tmpPosition = new Vector3(1, 1, 1);
                isoMetricForward = new Vector3(-1f, 0f, -1f);
                isoMetricBackward = new Vector3(1f, 0f, 1f);
                isoMetricLeft = new Vector3(1f, 0f, -1f);
                isoMetricRight = new Vector3(-1f, 0f, 1f);
            }
            else if (axis is IsometricAxis.NE)
            {
                tmpPosition = new Vector3(-1, 1, 1);
                isoMetricForward = new Vector3(1f, 0f, -1f);
                isoMetricBackward = new Vector3(-1f, 0f, 1f);
                isoMetricLeft = new Vector3(1f, 0f, 1f);
                isoMetricRight = new Vector3(-1f, 0f, -1f);
            }
            else if (axis is IsometricAxis.SW)
            {
                tmpPosition = new Vector3(1, 1, -1);
                isoMetricForward = new Vector3(-1f, 0f, 1f);
                isoMetricBackward = new Vector3(1f, 0f, -1f);
                isoMetricLeft = new Vector3(-1f, 0f, -1f);
                isoMetricRight = new Vector3(1f, 0f, 1f);
            }
            else if (axis is IsometricAxis.SE)
            {
                tmpPosition = new Vector3(-1, 1, -1);
                isoMetricForward = new Vector3(1f, 0f, 1f);
                isoMetricBackward = new Vector3(-1f, 0f, -1f);
                isoMetricLeft = new Vector3(-1f, 0f, 1f);
                isoMetricRight = new Vector3(1f, 0f, -1f);
            }

            mode = CameraType.FreeIsometric;
            isometricAxis = axis;

            Position = Vector3.Normalize(tmpPosition) * distance;
            Position += newInterest;
            Interest = newInterest;
        }

        private void SetOrtho()
        {
            StopTranslations();

            Position = Vector3.UnitY;
            Interest = Vector3.Zero;

            mode = CameraType.Ortho;
        }

        private void UpdateLens()
        {
            if (mode == CameraType.Ortho)
            {
                Projection = Matrix4x4.CreateOrthographic(
                    viewportWidth,
                    viewportHeight,
                    nearPlaneDistance,
                    farPlaneDistance);
            }
            else
            {
                Projection = Matrix4x4.CreatePerspectiveFieldOfView(
                    fieldOfView,
                    aspectRelation,
                    nearPlaneDistance,
                    farPlaneDistance);
            }
        }

        public void Goto(float x, float y, float z, CameraTranslations translation = CameraTranslations.None)
        {
            Goto(new Vector3(x, y, z), translation);
        }

        public void Goto(Vector3 newPosition, CameraTranslations translation = CameraTranslations.None)
        {
            Vector3 diff = newPosition - Position;

            if (translation != CameraTranslations.None)
            {
                StartTranslations(translation, Interest + diff);
            }
            else
            {
                StopTranslations();

                Position += diff;
                Interest += diff;
            }
        }

        public void LookTo(float x, float y, float z, CameraTranslations translation = CameraTranslations.None)
        {
            LookTo(new Vector3(x, y, z), translation);
        }

        public void LookTo(Vector3 newInterest, CameraTranslations translation = CameraTranslations.None)
        {
            if (translation != CameraTranslations.None)
            {
                StartTranslations(translation, newInterest);
            }
            else
            {
                StopTranslations();

                if (mode == CameraType.FreeIsometric)
                {
                    Vector3 diff = newInterest - Interest;

                    Position += diff;
                    Interest += diff;
                }
                else
                {
                    Interest = newInterest;
                }
            }
        }

        private void Move(GameTime gameTime, Vector3 vector, bool slow)
        {
            StopTranslations();

            float delta = (slow) ? SlowMovementDelta : MovementDelta;

            Velocity = vector * delta * gameTime.ElapsedSeconds;
            if (Velocity != Vector3.Zero)
            {
                Position += Velocity;
                Interest += Velocity;
            }
        }

        private void Rotate(GameTime gameTime, Vector3 axis, bool slow)
        {
            float degrees = (slow) ? SlowRotationDelta : RotationDelta;

            Rotate(axis, degrees * gameTime.ElapsedSeconds);
        }



        private void Rotate(Vector3 axis, float degrees, bool clampY = false, float clampFrom = 0, float clampTo = 0)
        {
            StopTranslations();

            // Smooth rotation
            Quaternion sourceRotation = Quaternion.RotationAxis(axis, 0);
            Quaternion targetRotation = Quaternion.RotationAxis(axis, MathUtil.DegreesToRadians(degrees));
            Quaternion r = Quaternion.Lerp(sourceRotation, targetRotation, 0.5f);

            Vector3 curDir = Vector3.Normalize(Interest - Position);
            Vector3 newDir = MathUtil.Transform(curDir, r);

            if (clampY)
            {
                float newAngle = MathUtil.Angle(Vector3.UnitY, newDir) - MathUtil.PiOverTwo;
                if (newAngle >= MathUtil.DegreesToRadians(clampFrom) && newAngle <= MathUtil.DegreesToRadians(clampTo))
                {
                    Interest = position + newDir;
                }
            }
            else
            {
                Interest = position + newDir;
            }
        }

        private void Zoom(GameTime gameTime, bool zoomIn, bool slow)
        {
            StopTranslations();

            float delta = (slow) ? SlowZoomDelta : ZoomDelta;
            float zooming = (zoomIn) ? +delta : -delta;

            if (zooming != 0f)
            {
                float s = gameTime.ElapsedSeconds;

                Vector3 newPosition = Position + (Direction * zooming * s);

                float distance = Vector3.Distance(newPosition, Interest);
                if (distance < ZoomMax && distance > ZoomMin)
                {
                    Position = newPosition;
                }
            }
        }


        private void StartTranslations(CameraTranslations translation, Vector3 newInterest)
        {
            translationMode = translation;
            translationInterest = newInterest;
        }
        /// <summary>
        /// Stops all current automatic translations
        /// </summary>
        private void StopTranslations()
        {
            if (translationMode != CameraTranslations.None)
            {
                translationMode = CameraTranslations.None;
                translationInterest = Vector3.Zero;
            }
        }

        private void UpdateTranslations(GameTime gameTime)
        {
            if (translationMode != CameraTranslations.None)
            {
                Vector3 diff = translationInterest - Interest;
                Vector3 pos = Position + diff;
                Vector3 dir = pos - Position;

                float distanceToTarget = dir.Length();
                float distanceThisMove = 0f;

                if (translationMode == CameraTranslations.UseDelta)
                {
                    distanceThisMove = MovementDelta * gameTime.ElapsedSeconds;
                }
                else if (translationMode == CameraTranslations.UseSlowDelta)
                {
                    distanceThisMove = SlowMovementDelta * gameTime.ElapsedSeconds;
                }
                else if (translationMode == CameraTranslations.Quick)
                {
                    distanceThisMove = distanceToTarget * translationOutOfRadius;
                }

                Vector3 movingVector;
                if (distanceThisMove >= distanceToTarget)
                {
                    //This movement goes beyond the destination.
                    movingVector = Vector3.Normalize(dir) * distanceToTarget * translationIntoRadius;
                }
                else if (distanceToTarget < translationRadius)
                {
                    //Into slow radius
                    movingVector = Vector3.Normalize(dir) * distanceThisMove * (distanceToTarget / translationRadius);
                }
                else
                {
                    //On flight
                    movingVector = Vector3.Normalize(dir) * distanceThisMove;
                }

                if (movingVector != Vector3.Zero)
                {
                    Position += movingVector;
                    Interest += movingVector;
                }

                // TODO:  epsilon
                if (MathUtil.WithinEpsilon(Interest.X, translationInterest.X, 0.1f) &&
                    MathUtil.WithinEpsilon(Interest.Y, translationInterest.Y, 0.1f) &&
                    MathUtil.WithinEpsilon(Interest.Z, translationInterest.Z, 0.1f))
                {
                    StopTranslations();

                }


            }
        }



        public static Camera CreateIsometric(IsometricAxis axis, Vector3 interest, float distance)
        {
            Camera cam = new Camera();

            cam.SetIsometric(axis, interest, distance);

            return cam;
        }

        public static Camera CreateFree(Vector3 position, Vector3 interest)
        {
            return new Camera
            {
                mode = CameraType.Free,
                position = position,
                interest = interest
            };
        }

        public static Camera CreateOrtho(Vector3 position, Vector3 interest, int width, int height)
        {
            Camera camera = new Camera
            {
                Position = position,
                Interest = interest
            };

            camera.SetLens(width, height);

            return camera;
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }



    }
}
