using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Voxel.Engine.World;

namespace Voxel.Engine.Managers
{
    public class ChunkManager : BaseManager
    {
        private Dictionary<Vector3, Chunk> chunks;
        public Dictionary<Vector3, Chunk> Chunks
        {
            get { return chunks; }
        }

        public ChunkManager(SceneGame game) : base(game)
        {
            this.chunks = new Dictionary<Vector3, Chunk>();

            Initialize();
        }

        protected override void Initialize()
        {
            ///AddChunk(new Chunk(this, new Vector3(0, 0, 0)));
            base.Initialize();
        }

        protected override string GetName()
        {
            return "Chunk";
        }

        public override void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<Vector3, Chunk> pair in chunks)
                (pair.Value).Update(gameTime);
        }

        public void AddChunk(Chunk chunk)
        {
            Chunk checkEntity = null;
            if (chunks.TryGetValue(chunk.position, out checkEntity))
                throw new Exception("A chunk with position " + chunk.position + " already exists.");
            chunks.Add(chunk.position/32, chunk);
        }

        public void RemoveChunk(Chunk chunk)
        {
            Chunk checkEntity = null;
            if (!chunks.TryGetValue(chunk.position, out checkEntity))
                throw new Exception("No chunk with position " + chunk.position + " exists in the scene to be removed.");
            chunks.Remove(chunk.position/32);
        }

        public Chunk GetChunk(Vector3 chunkPosition)
        {
            Chunk entity = null;
            if (!chunks.TryGetValue(chunkPosition, out entity)) { }
                //Console.WriteLine(new Exception("No chunk at position " + chunkPosition.ToString() + " exists in the scene."));
            return entity;
        }
    }
}
