using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoxEngine.Interfaces
{
    public interface IVoxShader
    {
        Effect BaseEffect
        {
            get;
        }
        bool ReadyToRender
        {
            get;
        }

        void Initialize(GraphicsDevice graphicsDevice);
    }
}
