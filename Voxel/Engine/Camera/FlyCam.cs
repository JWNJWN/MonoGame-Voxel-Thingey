using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

using Voxel.Engine.Physics;

namespace Voxel.Engine.Camera
{
    public class FlyCam : Camera
    {
        private float cSensitivity = 0.008f;
        private float cSpeed = 0.5f;

        private MouseState previousState;

        public FlyCam(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
            cPosition = new Vector3(0, 5, 0);
            previousState = Mouse.GetState();
        }

        public override Matrix ViewMatrix
        {
            get
            { 
                Matrix mYaw = Matrix.CreateFromAxisAngle(Vector3.Up, cAngles.X);
                Matrix mPitch = Matrix.CreateFromAxisAngle(Vector3.Right, cAngles.Y);

                return Matrix.CreateTranslation(-cPosition) * (mYaw * mPitch);
            }
        }

        public override void Update()
        {
            base.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cPosition += cForward * cSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cPosition -= cForward * cSpeed;
            
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cPosition -= cRight * cSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cPosition += cRight * cSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                cPosition += cUp * cSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                cPosition -=  cUp * cSpeed;

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
                cPosition = new Vector3(0, 5, 0);

            cAngles.X += (Mouse.GetState().X - previousState.X) * cSensitivity;
            cAngles.Y -= (graphicsDevice.Viewport.Height / 2 - Mouse.GetState().Y) * cSensitivity;

            //Cap to Pi radians of freedom
            cAngles.Y = MathHelper.Clamp(cAngles.Y, -MathHelper.PiOver2, MathHelper.PiOver2);

            //Reset at full turn
            cAngles.X = cAngles.X % MathHelper.TwoPi;

            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);

            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Segment seg = new Segment(cPosition, cForward, Vector3.One);
                //Hit hit = Collision.Intersect()
            }
        }
    }
}
