using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graphics3D
{
    class Camera
    {
        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        private Vector3 cameraLookAt;
        private Vector3 cameraUpVector;

        private float angleX = 0;
        private float angleY = 0;
        private float angleZ = 0;

        private GraphicsDevice _graphicsDevice;

        private MouseState currentMouseState, previousMouseState;

        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), _graphicsDevice.Viewport.AspectRatio, 0.01f, 1000f); }
        }

        public Vector3 Position
        {
            get { return cameraPosition; }
        }

        public Vector3 UpVector
        {
            get { return cameraUpVector; }
        }

        public Vector3 ForwardVector
        {
            get { return cameraPosition - cameraLookAt; }
        }

        public Vector3 HorizontalVector
        {
            get { return Vector3.Cross(ForwardVector, cameraUpVector); }
        }

        public Vector3 Rotation
        {
            get { return cameraRotation; }
        }

        public Vector3 LookAt
        {
            get { return cameraLookAt; }
        }

        public Matrix View
        {
            get
            {
                cameraLookAt = Vector3.Transform(ForwardVector, Matrix.CreateFromAxisAngle(cameraUpVector, angleX));

                cameraLookAt = Vector3.Transform(ForwardVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleY));
                //cameraLookAt -= cameraPosition;

                //cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUpVector, cameraLookAt), angleY));


                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, cameraUpVector);
            }
        }

        public Camera(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            cameraPosition = new Vector3(0.0f, 0.0f, 200.0f);
            cameraLookAt = Vector3.Zero;
            cameraUpVector = Vector3.Up;
        }

        public void Update()
        {
            KeyboardState newState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            #region reset
            if (newState.IsKeyDown(Keys.R))
            {
                cameraPosition = new Vector3(0.0f, 0.0f, 200.0f);
                cameraLookAt = Vector3.Zero;
                cameraUpVector = Vector3.Up;
                angleX = 0;
                angleY = 0;
                angleZ = 0;
            }
            if (newState.IsKeyDown(Keys.C))
            {
                cameraLookAt = Vector3.Zero;
            }
            if (newState.IsKeyDown(Keys.U))
            {
                cameraUpVector = Vector3.Up;
            }
            #endregion

            #region look around

            if (newState.IsKeyDown(Keys.Up))
            {
                //cameraLookAt = Vector3.Transform(cameraLookAt, Matrix.CreateFromAxisAngle(HorizontalVector, MathHelper.ToRadians(0.1f)));
                //cameraUpVector = Vector3.Cross(HorizontalVector, ForwardVector);

                //var vec = ForwardVector;
                //var vecSpher = CoordsHelper.CartesianToSpherical(vec);
                //vecSpher.Theta += MathHelper.ToRadians(0.5f);
                //var vecCart = CoordsHelper.SphericalToCartesian(vecSpher);
                //cameraLookAt = vecCart + cameraPosition;

                //angleY = MathHelper.ToRadians(0.5f);
                //cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleY));

                //Vector3 axis = cameraPosition - cameraLookAt;
                //Vector3 axisUp = new Vector3(axis.X, -axis.Y, axis.Z);
                //Vector3 right = Vector3.Cross(axis, axisUp);
                //cameraLookAt = Vector3.Transform(cameraLookAt, Matrix.CreateFromAxisAngle(right, MathHelper.ToRadians(0.3f)));

                //primitive, cause all others don't work
                cameraLookAt += Vector3.Up;
            }

            else if (newState.IsKeyDown(Keys.Down))
            {
                //var vec = cameraLookAt - cameraPosition;
                //var vecSpher = CoordsHelper.CartesianToSpherical(vec);
                //vecSpher.Theta -= MathHelper.ToRadians(0.5f);
                //var vecCart = CoordsHelper.SphericalToCartesian(vecSpher);
                //cameraLookAt = vecCart + cameraPosition;

                //angleY = MathHelper.ToRadians(-0.5f);
                //cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleY));

                //Vector3 axis = cameraPosition - cameraLookAt;
                //Vector3 axisUp = new Vector3(axis.X, -axis.Y, axis.Z);
                //Vector3 right = Vector3.Cross(axis, axisUp);
                //cameraLookAt = Vector3.Transform(cameraLookAt, Matrix.CreateFromAxisAngle(right, MathHelper.ToRadians(-0.3f))) ;

                //primitive, cause all others don't work
                cameraLookAt -= Vector3.Up;
            }
            else
            {
                angleY = 0.0f;
            }
            if (newState.IsKeyDown(Keys.Left))
            {
                angleX = MathHelper.ToRadians(0.5f);
            }
            else if (newState.IsKeyDown(Keys.Right))
            {
                angleX = MathHelper.ToRadians(-0.5f);
            }
            else
            {
                angleX = 0.0f;
            }
            #endregion

            #region zoom
            if (newState.IsKeyDown(Keys.X))
            {
                Vector3 axis = cameraPosition - cameraLookAt;
                axis *= 0.99f;
                cameraPosition = axis + cameraLookAt;
            }

            if (newState.IsKeyDown(Keys.Z))
            {
                Vector3 axis = cameraPosition - cameraLookAt;
                axis *= 1.01f;
                cameraPosition = axis + cameraLookAt;
            }
            #endregion

            #region move around
            if (newState.IsKeyDown(Keys.W))
            {
                Vector3 up = cameraUpVector;
                up.Normalize();

                cameraPosition += up;
                cameraLookAt += up;
            }
            if (newState.IsKeyDown(Keys.S))
            {
                Vector3 up = cameraUpVector;
                up.Normalize();

                cameraPosition -= up;
                cameraLookAt -= up;
            }
            if (newState.IsKeyDown(Keys.A))
            {
                Vector3 right = Vector3.Cross(ForwardVector, cameraUpVector);
                right.Normalize();
                cameraPosition += right;
                cameraLookAt += right;
            }
            if (newState.IsKeyDown(Keys.D))
            {
                Vector3 right = Vector3.Cross(ForwardVector, cameraUpVector);
                right.Normalize();
                cameraPosition -= right;
                cameraLookAt -= right;
            }
            #endregion

            #region rotate clock
            if (newState.IsKeyDown(Keys.Q))
            {
                cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(ForwardVector, MathHelper.ToRadians(-0.01f)));
            }
            if (newState.IsKeyDown(Keys.E))
            {
                cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(ForwardVector, MathHelper.ToRadians(0.01f)));
            }
            #endregion

        }
    }
}