using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.Types;
using Microsoft.Xna.Framework;
using VoxEngine.Interfaces;

namespace VoxEngine.Voxels
{
    public struct VoxVoxel : IVoxVoxel
    {
        //OFFSETS STORED AS: TOP BOTTOM LEFT RIGHT FRONT BACK
        public VoxVoxel(bool transparent, byte hardness, byte viscocity, Byte2[] offset, bool infrequentUpdates = false, EventHandler<Vector3> update = null)
        {
            _transparent = transparent;
            _hardness = hardness;
            _viscocity = viscocity;
            _offset = offset;
            _infrequentUpdates = infrequentUpdates;
            _update = update;
        }

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

        private Byte2[] _offset;
        public Byte2[] Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private bool _infrequentUpdates;
        public bool InfrequentUpdates
        {
            get { return _infrequentUpdates; }
            set { _infrequentUpdates = value; }
        }

        private EventHandler<Vector3> _update;
        public EventHandler<Vector3> Update
        {
            get { return _update; }
            set { _update = value; }
        }
    }
}
