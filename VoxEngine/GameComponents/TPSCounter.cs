using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxEngine.GameComponents
{
    public class TpsCounter : GameComponent
    {
        private float updateInterval = 1.0f;
        private float timeSinceLastUpdate = 0.0f;
        private float tickCount = 0;
        private float tps = 0;

        public float TPS
        {
            get { return tps; }
        }

        public TpsCounter(Game game) : base(game)
        {
            Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            tickCount++;
            timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeSinceLastUpdate >= updateInterval)
            {
                tps = tickCount / timeSinceLastUpdate; //mean fps over updateIntrval
                tickCount = 0;
                timeSinceLastUpdate -= updateInterval;

                Updated?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> Updated;
    }
}
