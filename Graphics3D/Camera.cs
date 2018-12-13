using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Graphics3D
{
    public class Camera
    {
        private Vector3 cameraPosition;
        private Vector3 cameraLookAt;
        private Vector3 cameraUpVector;


        float angleX;
        float angleY;

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

        public Vector3 LookAt
        {
            get { return cameraLookAt; }
        }

        public Matrix View
        {
            get
            {
                if (angleX != 0.0f)
                {
                    cameraLookAt = Vector3.Transform(ForwardVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleX));
                    cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleX));

                    cameraLookAt = Vector3.Transform(ForwardVector, Matrix.CreateFromAxisAngle(cameraUpVector, angleY));
                }

                if (angleY != 0.0f)
                {
                    cameraLookAt = Vector3.Transform(ForwardVector, Matrix.CreateFromAxisAngle(cameraUpVector, angleY));

                    cameraLookAt = Vector3.Transform(ForwardVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleX));
                    cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(HorizontalVector, angleX));

                    
                }


                //cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUpVector, cameraLookAt), angleX));


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
            }
            #endregion

            #region look around

            if (newState.IsKeyDown(Keys.Up))
            {
                angleX = MathHelper.ToRadians(-0.007f);
            }

            else if (newState.IsKeyDown(Keys.Down))
            {
                angleX = MathHelper.ToRadians(0.007f);
            }
            else
            {
                angleX = 0.0f;
            }

            if (newState.IsKeyDown(Keys.Left))
            {
                angleY = MathHelper.ToRadians(2.0f);
            }
            else if (newState.IsKeyDown(Keys.Right))
            {
                angleY = MathHelper.ToRadians(-2.0f);
            }
            else
            {
                angleY = 0.0f;
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
                Vector3 right = HorizontalVector;
                right.Normalize();
                cameraPosition += right;
                cameraLookAt += right;
            }
            if (newState.IsKeyDown(Keys.D))
            {
                Vector3 right = HorizontalVector;
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