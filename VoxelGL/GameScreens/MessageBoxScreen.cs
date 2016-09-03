using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Textures;
using VoxEngine.Managers;
using VoxEngine.GameComponents;

namespace VoxelGL.GameScreens
{
    class MessageBoxScreen : GameScreen
    {
        private const string texture = "gradient";
        private string _message;

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        public MessageBoxScreen(string message) : this(message, true)
        {

        }

        public MessageBoxScreen(string message, bool includeUsageText)
        {
            const string usageText = "\nSpace, Enter = ok" +
                                     "\nEsc = cancel";

            if (includeUsageText)
                _message = message + usageText;
            else
                _message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void LoadContent()
        {
            TextureManager.AddTexture(new VoxTexture("gradient"), texture);
        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            if(input.MenuSelect)
            {
                Accepted?.Invoke(this, EventArgs.Empty);
                ExitScreen();
            }else if (input.MenuCancel)
            {
                Cancelled?.Invoke(this, EventArgs.Empty);
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = EngineManager.Device.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = ScreenManager.Font.MeasureString(_message);
            Vector2 textPostion = (viewportSize - textSize) / 2;

            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPostion.X - hPad, (int)textPostion.Y - vPad,
                                                         (int)textSize.X + hPad * 2, (int)textSize.Y + vPad * 2);

            Color color = new Color(255, 255, 255, TransitionAlpha);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture).BaseTexture as Texture2D,
                                           backgroundRectangle, color);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _message, textPostion, color);
            ScreenManager.SpriteBatch.End();
        }
    }
}
