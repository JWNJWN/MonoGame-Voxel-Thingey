using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VoxEngine.Managers;
using VoxEngine.GameComponents;

namespace VoxEngine.GUI
{
    public class GUIButton : GUIObject
    {
        private string _text;
        private EventHandler _pressedEvent;

        public GUIButton(GUIPanel parent, float x, float y, float width, float height, string buttonName, string text, EventHandler pressedEvent)
            : base(parent, x, y, width, height)
        {
            parent.Children.Add(buttonName, this);
            _text = text;
            _pressedEvent = pressedEvent;
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Draw((Texture2D)TextureManager.GetTexture("blank").BaseTexture, Bounds, Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _text, Bounds.Center.ToVector2() - ScreenManager.Font.MeasureString(_text) / 2, Color.White);
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (Bounds.Contains(EngineManager.Input.CurrentMouseState.Position) && EngineManager.Input.CurrentMouseState.LeftButton == ButtonState.Pressed)
                _pressedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
