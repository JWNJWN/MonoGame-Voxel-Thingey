using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel.Engine.UI
{
    public class DrawHandler
    {
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;

        public DrawHandler(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
        }

        //public DrawText(int x, int y, string )

        public void DrawRect(int x, int y, int width, int height, Color color)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), color);
        }

        public void DrawHollowRect(int x, int y, int width, int height, int stroke, Color color)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, new Rectangle(x, y, width, stroke), color);
            spriteBatch.Draw(pixel, new Rectangle(x+width-stroke, y, stroke, height), color);
            spriteBatch.Draw(pixel, new Rectangle(x, height, width, stroke), color);
            spriteBatch.Draw(pixel, new Rectangle(x, y, stroke, height), color);
        }

        public void DrawString(SpriteFont font, string text, Color color, float scale, Vector2 position)
        {
            Vector2 origin = Vector2.Zero;
            spriteBatch.DrawString(font, text, position, color, 0, origin, scale, SpriteEffects.None, 0f);
        }

        public void DrawOutlinedString(SpriteFont font, string text, Color backColor, Color frontColor, float scale, Vector2 position)
        {
            Vector2 origin = Vector2.Zero;

            spriteBatch.DrawString(font, text, position + new Vector2(1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(-1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(-1 * scale, -1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(1 * scale, -1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);          

            spriteBatch.DrawString(font, text, position, frontColor, 0, origin, scale, SpriteEffects.None, 0f);
        }

        public void DrawShadowedString(SpriteFont font, string text, Color backColor, Color frontColor, float scale, Vector2 position)
        {
            Vector2 origin = Vector2.Zero;

            spriteBatch.DrawString(font, text, position + new Vector2(1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(-1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);

            spriteBatch.DrawString(font, text, position, frontColor, 0, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
