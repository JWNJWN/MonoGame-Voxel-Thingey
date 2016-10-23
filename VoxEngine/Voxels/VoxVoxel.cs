using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.Interfaces;

namespace VoxEngine.Voxels
{
    public struct VoxVoxel : IVoxVoxel
    {
        private byte _hardness;
        public byte Hardness
        {
            get { return _hardness; }
            set { _hardness = value; }
        }

        private byte _viscocity;
        public byte Viscocity
        {
            get { return _viscocity; }
            set { _viscocity = value; }
        }

        private bool _transparent;
        public bool Transparent
        {
            get { return _transparent; }
            set { _transparent = value; }
        }

        private Vector2 _offset;
        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
    }
}
