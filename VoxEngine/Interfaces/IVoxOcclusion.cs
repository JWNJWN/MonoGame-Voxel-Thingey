using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.Interfaces
{
    public interface IVoxOcclusion : IVoxCullable, IVoxSceneObject
    {
        string OcclusionModelName
        {
            get;
            set;
        }

        OcclusionQuery Query
        {
            get;
        }

        bool Occluded
        {
            get;
            set;
        }

        void DrawCulling(GameTime gameTime);
    }
}
