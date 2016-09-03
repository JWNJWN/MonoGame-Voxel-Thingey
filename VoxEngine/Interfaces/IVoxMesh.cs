using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.Interfaces
{
    public interface IVoxMesh
    {
        List<IVertexType> Vertices
        {
            get;
            set;
        }
        List<uint> Indices
        {
            get;
            set;
        }

        VertexBuffer VertexBuffer
        {
            get;
            set;
        }

        IndexBuffer IndexBuffer
        {
            get;
            set;
        }
    }
}
