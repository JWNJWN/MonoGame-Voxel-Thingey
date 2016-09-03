using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.Interfaces
{
    public interface IVoxTexture
    {
        string FileName
        {
            get;
            set;
        }

        Texture BaseTexture
        {
            get;
        }

        bool ReadyToRender
        {
            get;
        }

        void LoadContent();
        void UnloadContent();
    }
}
