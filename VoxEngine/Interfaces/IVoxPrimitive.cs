using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VoxEngine.Textures;

namespace VoxEngine.Interfaces
{
    public interface IVoxPrimitive : IVoxMesh
    {
        List<VertexPositionColor> Vertices
        {
            get;
            set;
        }
    }
}
