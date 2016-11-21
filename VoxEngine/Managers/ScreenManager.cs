using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Textures;
using VoxEngine.Menus;

namespace VoxEngine.Managers
{
    public class ScreenManager : DrawableGameComponent
    {
        private static List<GameScreen> _screens = new List<GameScreen>();
        private static List<GameScreen> _screensToUpdate = new List<GameScreen>();

        public static GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        private static bool _initialized = false;
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static SpriteBatch _spriteBatch;
        public static SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        private static SpriteFont _font;
        public static SpriteFont Font
        {
            get { return _font; }
        }

        bool _traceEnabled = false;
        public bool TraceEnabled
        {
            get { return _traceEnabled; }
            set { _traceEnabled = value; }
        }

        public ScreenManager(Game game) : base(game)
        {
            Enabled = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(EngineManager.Device);
            _font = EngineManager.ContentManager.Load<SpriteFont>("Content/Fonts/menufont");

            foreach (GameScreen screen in _screens)
                screen.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (GameScreen screen in _screens)
                screen.UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            _initialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            EngineManager.Input.Update(gameTime);

            _screensToUpdate.Clear();

            foreach (GameScreen screen in _screens)
                _screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while(_screensToUpdate.Count > 0)
            {
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if(screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    if(!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime);

                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            if (_traceEnabled)
                TraceScreens();
        }

        private void TraceScreens()
        {
            List<string> screenNames = new List<string>();
            foreach (GameScreen screen in _screens)
                screenNames.Add(screen.GetType().Name);

            Trace.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                screen.Draw(gameTime);
            }
            foreach(GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                screen.PostUIDraw(gameTime);
            }
        }

        public static void AddScreen(GameScreen screen)
        {
            _screens.Add(screen);
            if (_initialized)
                screen.LoadContent();
        }

        public static void RemoveScreen(GameScreen screen)
        {
            if (_initialized)
                screen.UnloadContent();

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);
        }

        public static void FadeBackBufferToBlack(int alpha)
        {
            Viewport viewport = EngineManager.Device.Viewport;

            _spriteBatch.Begin();
            _spriteBatch.Draw(TextureManager.GetTexture("blank").BaseTexture as Texture2D,
                                new Rectangle(0,0, viewport.Width, viewport.Height),
                                new Color(0, 0, 0, (byte)alpha));
            _spriteBatch.End();
        }
    }
}
