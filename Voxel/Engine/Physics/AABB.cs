using Microsoft.Xna.Framework;

namespace Voxel.Engine.Physics
{
    public struct AABB : Collider
    {
        private Vector3 Position, Size;

        public Vector3 Min
        {
            get
            {
                return Position;
            }
            set
            {
                Position = value;
            }
        }

        public Vector3 Max
        {
            get
            {
                return Position + Size;
            }
            set
            {
                Size = value - Position;
            }
        }

        public Vector3 Center
        {
            get
            {
                return Position + Size / 2;
            }
        }

        public Vector3 LengthFromCenter
        {
            get
            {
                return Size / 2;
            }
        }

        public AABB(Vector3 Position, Vector3 Size)
        {
            this.Position = Position;
            this.Size = Size;
        }

        public AABB(Vector3[] Positions)
        {
            Position = Positions[0];
            Size = Vector3.Zero;
            foreach (Vector3 pos in Positions)
            {
                Vector3 TempMinMax = Min;
                if (pos.X < TempMinMax.X) TempMinMax.X = pos.X;
                if (pos.Y < TempMinMax.Y) TempMinMax.Y = pos.Y;
                if (pos.Z < TempMinMax.Z) TempMinMax.Z = pos.Z;
                Min = TempMinMax;

                TempMinMax = Max;
                if (pos.X > TempMinMax.X) TempMinMax.X = pos.X;
                if (pos.Y > TempMinMax.Y) TempMinMax.Y = pos.Y;
                if (pos.Z > TempMinMax.Z) TempMinMax.Z = pos.Z;
                Max = TempMinMax;
            }
        }
    }
}
