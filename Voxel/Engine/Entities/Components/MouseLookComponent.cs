using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Voxel.Engine.Entities.Components
{
    public class MouseLookComponent : BaseComponent
    {
        float sensitivity;

        MouseState currentMouseState;
        MouseState originalMouseState;

        float xRot, yRot;

        public MouseLookComponent(BaseEntity parentEntity, float mouseSensitivity) : base(parentEntity)
        {
            sensitivity = mouseSensitivity;
            Initialize();
        }

        protected override void Initialize()
        {
            Mouse.SetPosition(Parent.Manager.Game.GraphicsDevice.Viewport.Width / 2, Parent.Manager.Game.GraphicsDevice.Viewport.Height / 2);

            originalMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            currentMouseState = Mouse.GetState();
            if(currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;

                yRot -= xDifference * sensitivity * delta;
                xRot -= yDifference * sensitivity * delta;

                yRot %= MathHelper.TwoPi;
                xRot = MathHelper.Clamp(xRot, -MathHelper.PiOver2+0.001f, MathHelper.PiOver2-0.0001f);

                Mouse.SetPosition(Parent.Manager.Game.GraphicsDevice.Viewport.Width / 2, Parent.Manager.Game.GraphicsDevice.Viewport.Height / 2);

                Parent.rotation = Matrix.CreateRotationX(xRot) * Matrix.CreateRotationY(yRot);
            }
        }

        protected override string GetName()
        {
            return "MouseLook";
        }
    }
}
