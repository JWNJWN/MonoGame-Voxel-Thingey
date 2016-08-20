using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Voxel.Engine.Entities.Components
{
    public class VoxelContainerComponent : BaseComponent
    {
        private byte[] voxels;
        public byte[] Voxels
        {
            get { return voxels; }
            set { voxels = value; }
        }
        public int containerSize;

        public bool dirty = false;

        public VoxelContainerComponent(BaseEntity parentEntity, int ContainerSize) : base(parentEntity)
        {
            containerSize = ContainerSize;
            Initialize();
        }

        protected override void Initialize()
        {
            voxels = new byte[containerSize * containerSize * containerSize];

            base.Initialize();
        }

        public byte GetVoxel(int x, int y, int z)
        {
            return Voxels[x + y * containerSize + z * containerSize * containerSize];
        }

        public void SetVoxel(int x, int y, int z, byte voxel)
        {
            Voxels[x + y * containerSize + z * containerSize * containerSize] = voxel;
            dirty = true;
        }

        protected override string GetName()
        {
            return "VoxelContainer";
        }
    }
}
