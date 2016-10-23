using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.Voxels;
using VoxEngine.Interfaces;

namespace VoxEngine.Managers
{
    public class VoxelManager : GameComponent
    {
        private static IVoxVoxel[] _voxels = new IVoxVoxel[65535];
        public static IVoxVoxel[] Voxels
        {
            get { return _voxels; }
        }

        private static bool _initialized = false;
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static ushort _voxelsLoaded = 0;
        public static ushort VoxelsLoaded
        {
            get { return _voxelsLoaded; }
        }

        public VoxelManager(Game game) : base(game) { }

        public static void AddVoxel(IVoxVoxel newVoxel, ushort voxelIndex)
        {
            if (_voxels[voxelIndex] == null)
                _voxels[voxelIndex] = newVoxel;
        }

        public override void Initialize()
        {
            base.Initialize();

            VoxVoxel air = new VoxVoxel();
            air.Transparent = true;
            AddVoxel(air, 0);
            VoxVoxel stone = new VoxVoxel();
            stone.Transparent = false;
            stone.Viscocity = 255;
            stone.Hardness = 128;
            stone.Offset = new Vector2(0, 0);
            AddVoxel(stone, 1);

            VoxVoxel dirt = new VoxVoxel();
            dirt.Transparent = false;
            dirt.Viscocity = 255;
            dirt.Hardness = 64;
            dirt.Offset = new Vector2(1, 0);
            AddVoxel(dirt, 2);

            _initialized = true;
        }
    }
}
