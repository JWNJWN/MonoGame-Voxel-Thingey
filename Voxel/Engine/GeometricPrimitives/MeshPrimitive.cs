using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel.Engine.GeometricPrimitives
{
    public class MeshPrimitive : GeometricPrimitive
    {
        public MeshPrimitive(GraphicsDevice graphicsDevice, List<VertexPositionColor> vertList, List<int> indList)
        {
            foreach (VertexPositionColor vert in vertList)
                AddVertex(vert.Position, Vector3.Zero, vert.Color);

            foreach (int ind in indList)
                AddIndex(ind);

            InitializePrimitive(graphicsDevice);
        }
    }
}
