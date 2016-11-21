using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.Voxels;
using VoxEngine.Types;
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

        //FOR UNIQUE SIDES ORDER IS: TOP BOTTOM LEFT RIGHT FORWARD BACK

        public override void Initialize()
        {
            base.Initialize();

            VoxVoxel air = new VoxVoxel(true, 0, 0, new Byte2[6] { Byte2.Zero, Byte2.Zero, Byte2.Zero, Byte2.Zero, Byte2.Zero, Byte2.Zero });
            AddVoxel(air, 0);

            VoxVoxel stone = new VoxVoxel(false, 255, 128, new Byte2[6] { Byte2.Zero, Byte2.Zero, Byte2.Zero, Byte2.Zero, Byte2.Zero, Byte2.Zero });
            AddVoxel(stone, 1);

            VoxVoxel dirt = new VoxVoxel(false, 255, 64, new Byte2[6] { new Byte2(1, 0), new Byte2(1, 0), new Byte2(1, 0), new Byte2(1, 0), new Byte2(1, 0), new Byte2(1, 0) });
            AddVoxel(dirt, 2);

            VoxVoxel grass = new VoxVoxel(false, 255, 64, new Byte2[6] { new Byte2(2, 0), new Byte2(1, 0), new Byte2(3, 0), new Byte2(3, 0), new Byte2(3, 0), new Byte2(3, 0) }, true, (object sender, Vector3 voxPosition) =>
            {
                Random rand = new Random();
                Vector3 spreadRadius = new Vector3(2, 2, 2);
                List<Vector3> potentialGrowSpots = new List<Vector3>();
                for (int x = (int)(voxPosition.X - spreadRadius.X); x < voxPosition.X + spreadRadius.X; x++)
                {
                    for (int y = (int)(voxPosition.Y - spreadRadius.Y); y < voxPosition.Y + spreadRadius.Y; y++)
                    {
                        for (int z = (int)(voxPosition.Z - spreadRadius.Z); z < voxPosition.Z + spreadRadius.Z; z++)
                        {
                            if (SceneChunkManager.GetVoxel(x, y, z) == 2 && SceneChunkManager.GetVoxel(x, y + 1, z) == 0)
                                potentialGrowSpots.Add(new Vector3(x, y, z));
                        }
                    }
                }

                foreach (var pos in potentialGrowSpots)
                    if (rand.Next(0, 100) > 98)
                        SceneChunkManager.SetVoxel((int)pos.X, (int)pos.Y, (int)pos.Z, 3);

            });
            AddVoxel(grass, 3);

            _initialized = true;
        }
    }
}
