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

        public EngineManager(string unitTestName) : base(unitTestName) { }
        public EngineManager() : base("Engine") { }

        protected override void Draw(GameTime gameTime)
        {
            //Device.Clear(BackgroundColor);

            base.Draw(gameTime);

            //this.Window.Title = FpsCounter.FPS.ToString();
        }
    }
}
