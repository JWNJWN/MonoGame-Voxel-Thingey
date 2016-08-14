using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Voxel.Engine.Camera
{
    public class PlayerCamera : Camera
    {
        private MouseState previousState;
        private float cSensitivity = 0.008f;

        public PlayerCamera(GraphicsDevice graphicsDevice, float sensitivity) : base(graphicsDevice)
        {
            cSensitivity = sensitivity;
        }
        public PlayerCamera(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }

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

            cAngles.X += (Mouse.GetState().X - previousState.X) * cSensitivity;
            cAngles.Y -= (graphicsDevice.Viewport.Height / 2 - Mouse.GetState().Y) * cSensitivity;

            //Cap to Pi radians of freedom
            cAngles.Y = MathHelper.Clamp(cAngles.Y, -MathHelper.PiOver2 + MathHelper.ToRadians(1f), MathHelper.PiOver2 - MathHelper.ToRadians(1f));

            //Reset at full turn
            cAngles.X = cAngles.X % MathHelper.TwoPi;

            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            previousState = Mouse.GetState();
        }
    }
}
