using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.Interfaces
{
    public interface IVoxDebug
    {
        List<VertexPositionColor> DebugVertices
        {
            get;
            set;
        }

        List<short> DebugIndices
        {
            get;
            set;
        }

        void DrawDebug(GameTime gameTime);
    }
}
