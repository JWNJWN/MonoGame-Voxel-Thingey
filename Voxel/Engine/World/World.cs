using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Voxel.Engine.World.Voxel;

namespace Voxel.Engine.World
{
    public class World
    {

        Dictionary<Vector3i, Chunk> wChunks;

        public World()
        {
            wChunks = new Dictionary<Vector3i, Chunk>();
        }

        public void CreateChunk(Vector3i pos)
        {
            /*if (!wChunks.ContainsKey(pos))
                wChunks.Add(pos, new Chunk(pos.ToVector3()));*/
        }

        public Chunk[] GetChunks()
        {
            return wChunks.Values.ToArray();
        }

        public Chunk GetChunk(Vector3i cPos)
        {
            return wChunks[cPos];
        }

        public byte GetVoxelByte(Vector3i vPos)
        {
            return wChunks[vPos / 32].GetVoxelByte(vPos % 32);
        }

        public Vox GetVoxel(Vector3i vPos)
        {
            return wChunks[vPos / 32].GetVoxel(vPos % 32);
        }
    }
}
