using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

namespace Voxel.Engine.GeometricPrimitives
{
    public class MeshPrimitive : GeometricPrimitive
    {
        public MeshPrimitive(GraphicsDevice graphicsDevice, List<VertexPositionColorNormal> vertList, List<int> indList)
        {
            foreach (VertexPositionColorNormal vert in vertList)
                AddVertex(vert.Position, vert.Normal, vert.Color);

            foreach (int ind in indList)
                AddIndex(ind);

            InitializePrimitive(graphicsDevice);
        }
    }
}
