using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.Types;

namespace VoxEngine.Interfaces
{
    public interface IVoxVoxel
    {
        byte Viscocity
        {
            get;
            set;
        }

        bool Transparent
        {
            get;
            set;
        }

        byte Hardness
        {
            get;
            set;
        }

        Byte2[] Offset
        {
            get;
            set;
        }

        bool InfrequentUpdates
        {
            get;
            set;
        }

        EventHandler<Vector3> Update
        {
            get;
            set;
        }
    }
}
