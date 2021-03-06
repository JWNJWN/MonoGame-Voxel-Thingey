﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxEngine.GameComponents
{
    public class FpsCounter : DrawableGameComponent
    {
        private float updateInterval = 1.0f;
        private float timeSinceLastUpdate = 0.0f;
        private float frameCount = 0;
        private float fps = 0;

        public float FPS
        {
            get { return fps; }
        }

        public FpsCounter(Game game) : base(game)
        {
            Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(timeSinceLastUpdate >= updateInterval)
            {
                timeSinceLastUpdate -= updateInterval;
                fps = frameCount;
                frameCount = 0;

                Updated?.Invoke(this, new EventArgs());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCount++;
        }

        public event EventHandler<EventArgs> Updated;
    }
}
