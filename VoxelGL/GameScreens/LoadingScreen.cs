using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.Managers;
using VoxEngine.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelGL.GameScreens
{
    class LoadingScreen : GameScreen
    {
        bool _loadingIsSlow;
        bool _otherScreensAreGone;

        GameScreen[] _screensToLoad;

        private LoadingScreen(bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
        }

        public static void Load(bool loadingIsSlow, params GameScreen[] screensToLoad)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            LoadingScreen loadingScreen = new LoadingScreen(loadingIsSlow, screensToLoad);

            ScreenManager.AddScreen(loadingScreen);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if(_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach(GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                        ScreenManager.AddScreen(screen);
                }
                EngineManager.Game.ResetElapsedTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) && (ScreenManager.GetScreens().Length == 1))
                _otherScreensAreGone = true;

            if (_loadingIsSlow)
            {
                const string message = "Loading...";

                Viewport viewport = EngineManager.Device.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = ScreenManager.Font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = new Color(255, 255, 255, TransitionAlpha);

                ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message, textPosition, color);
                ScreenManager.SpriteBatch.End();
            }
        }
    }
}
