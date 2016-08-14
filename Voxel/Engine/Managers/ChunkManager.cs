using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voxel.Engine.Entities;

namespace Voxel.Engine.Managers
{
    public class ChunkManager : BaseManager
    {
        private Dictionary<Vector3i, ChunkEntity> chunks;
        public Dictionary<Vector3i, ChunkEntity> Chunks
        {
            get { return chunks; }
        }

        protected override string GetName()
        {
            return "Chunk";
        }

        public ChunkManager(SceneGame game) : base(game)
        {
            this.chunks = new Dictionary<Vector3i, ChunkEntity>();

            Initialize();
        }

    }
}
