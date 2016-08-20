using Microsoft.Xna.Framework;

namespace Voxel.Engine.World.Voxel
{
    public struct Vox
    {
        private Color vColor;
        private byte vStrength;

        public Vox(Color color, byte strength)
        {
            vColor = color;
            vStrength = strength;
        }

        public Color color
        {
            get { return vColor; }
            set { vColor = value; }
        }

        public byte strength
        {
            get { return vStrength; }
            set { vStrength = value; }
        }
    }
}
