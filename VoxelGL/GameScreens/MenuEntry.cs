using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Managers;

namespace VoxelGL.GameScreens
{
    class MenuEntry
    {
        string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        float selectionFade;
        public event EventHandler<EventArgs> Selected;
        protected internal virtual void OnSelectEntry()
        {
            Selected?.Invoke(this, EventArgs.Empty);
        }

        public MenuEntry(string text)
        {
            this.text = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalDays * 4;
            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade + fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Yellow : Color.White;

            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * selectionFade;

            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);
            Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 2);

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return ScreenManager.Font.LineSpacing;
        }
    }
}
