using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Voxel.Engine.Camera
{
    public class Camera
    {
        protected GraphicsDevice graphicsDevice;

        public Vector3 cPosition = new Vector3(0, 0, 0);
        public Vector3 cAngles = new Vector3(0, 0, 0);

        public Vector3 cUp = new Vector3(0, 1, 0);
        public Vector3 cRight = new Vector3(1, 0, 0);
        public Vector3 cForward = new Vector3(0, 0, 1);

        public virtual Matrix ViewMatrix
        {
            get
            { 
                return Matrix.CreateLookAt(cPosition, cForward, cUp);
            }
        }

        public virtual Matrix ProjectionMatrix
        {
            get
            {
                float fieldOfView = MathHelper.ToRadians(90f);
                float nearClipPlane = 0.001f;
                float farClipPlane = 1000;
                float aspectRatio = graphicsDevice.Viewport.Width / graphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Camera (GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
        }


        private void UpdateVectors()
        {
            Matrix transMatrix = Matrix.Transpose(ViewMatrix);
            cRight = transMatrix.Right;
            cForward = transMatrix.Forward;
            cUp = transMatrix.Up;
        }

        public virtual void Update() {
            UpdateVectors();
        }
    }
}
