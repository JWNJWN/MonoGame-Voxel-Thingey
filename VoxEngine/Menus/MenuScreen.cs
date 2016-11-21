using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Managers;
using VoxEngine.GameComponents;
using VoxEngine.GUI;

namespace VoxEngine.Menus
{
    public abstract class MenuScreen : GameScreen
    {
        protected GUIPanel _rootPanel;

        string _menuTitle;

        public MenuScreen(string menuTitle)
        {
            _menuTitle = menuTitle;
            _rootPanel = new GUIPanel();
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (EngineManager.Input.MenuCancel)
                ExitScreen();

            _rootPanel.HandleInput(gameTime);
        }

        protected virtual void OnCancel(object sender, EventArgs e)
        {
            ExitScreen();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            _rootPanel.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 position = new Vector2(100, 150);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X -= transitionOffset * 512;

            ScreenManager.SpriteBatch.Begin();

            _rootPanel.Draw(gameTime);

            Vector2 titlePosition = new Vector2(426, 80);
            Vector2 titleOrigin = ScreenManager.Font.MeasureString(_menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
        }
    }
}
