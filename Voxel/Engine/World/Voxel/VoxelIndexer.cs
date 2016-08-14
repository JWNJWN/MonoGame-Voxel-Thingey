using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Voxel.Engine.World.Voxel
{
    public class VoxelIndexer
    {
        public static Vox[] voxelIndex;

        public VoxelIndexer()
        {
            voxelIndex = new Vox[256];

            InitVoxelType();
        }

        void InitVoxelType()
        {
            byte i = 1;
            /* Grass */ voxelIndex[i++] = new Vox(Color.LawnGreen, 1);
            /* Dirt */ voxelIndex[i++] = new Vox(Color.SaddleBrown, 1);
            /* Stone */ voxelIndex[i++] = new Vox(Color.Gray, 1);
        }
    }
}
