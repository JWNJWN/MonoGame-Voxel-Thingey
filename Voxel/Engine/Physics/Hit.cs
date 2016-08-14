using Microsoft.Xna.Framework;

namespace Voxel.Engine.Physics
{
    public class Hit
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Delta;
        public float time;
        Collider collider;

        public Hit(Collider collider)
        {
            this.collider = collider;
        }
    }
}
