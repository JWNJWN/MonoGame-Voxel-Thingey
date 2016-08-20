using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.GeometricPrimitives;

namespace Voxel.Engine.Render
{
    public class RenderDescription
    {
        public Model model;
        public GeometricPrimitive geoPrim;

        public Matrix worldTransform = Matrix.Identity;

        public RenderDescription() { }
    }
}
