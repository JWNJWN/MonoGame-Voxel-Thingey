using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;

namespace Voxel.Engine.Entities.Components
{
    public class FlyControlComponent : BaseComponent
    {
        float speed;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        public FlyControlComponent(BaseEntity parentEntity, float movementSpeed) : base(parentEntity)
        {
            speed = movementSpeed;
            Initialize();
        }

        protected override string GetName()
        {
            return "FlyControl";
        }

        protected override void Initialize()
        {
            currentKeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.W))
                Parent.position += Parent.rotation.Forward * speed * delta;
            if (currentKeyboardState.IsKeyDown(Keys.S))
                Parent.position += Parent.rotation.Backward * speed * delta;
            if (currentKeyboardState.IsKeyDown(Keys.A))
                Parent.position += Parent.rotation.Left * speed * delta;
            if (currentKeyboardState.IsKeyDown(Keys.D))
                Parent.position += Parent.rotation.Right * speed * delta;
            
        }
    }
}
