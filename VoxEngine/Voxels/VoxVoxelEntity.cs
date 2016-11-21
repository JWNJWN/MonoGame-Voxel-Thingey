using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.Voxels
{
    public struct VoxVoxelEntity
    {
        public VoxVoxelEntity(EventHandler<Vector3> updateEvent)
        {
            _updateEvent = updateEvent;
        }

        private EventHandler<Vector3> _updateEvent;
        public EventHandler<Vector3> UpdateEvent
        {
            get { return _updateEvent; }
            set { _updateEvent = value; }
        }
    }
}
