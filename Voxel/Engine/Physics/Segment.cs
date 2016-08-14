using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Voxel.Engine.Physics
{
    public struct Segment : Collider
    {
        public Vector3 Position, Delta, Padding;

        public Segment(Vector3 Position, Vector3 Delta, Vector3 Padding)
        {
            this.Position = Position;
            this.Delta = Delta;
            this.Padding = Padding;
        }
    }
}
