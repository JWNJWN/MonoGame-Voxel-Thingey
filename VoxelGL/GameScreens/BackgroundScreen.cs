using System;
using System.Collections.Generic;
using System.Text;
using VoxEngine.GameComponents;
using VoxEngine.Managers;
using VoxEngine.Textures;
using VoxEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelGL.GameScreens
{
    public class BackgroundScreen : GameScreen
    {
        const string texture = "background";

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            TextureManager.AddTexture(new VoxTexture("background"), texture);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            TextureManager.RemoveTexture(texture);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = EngineManager.Device.Viewport;

            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture).BaseTexture as Texture2D,
                                            fullscreen,
                                            new Color(fade, fade, fade));
            ScreenManager.SpriteBatch.End();
        }
    }
}