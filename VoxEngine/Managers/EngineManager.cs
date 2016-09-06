using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxEngine.Managers
{
    public class EngineManager : VoxEngine
    {
        private static Game _game;
        public static Game Game
        {
            get { return _game; }
            set { _game = value; }
        }
        
        public EngineManager() : base("Engine")
        {
            IsMouseVisible = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
