using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxEngine.Interfaces
{
    public interface IVoxVoxel
    {
        ushort ID
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

        bool ReadyToRender
        {
            get;
        }

        void LoadContent();
        void UnloadContent();
    }
}
