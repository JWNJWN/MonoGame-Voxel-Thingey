using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

        Vector2 Offset
        {
            get;
            set;
        }
    }
}
