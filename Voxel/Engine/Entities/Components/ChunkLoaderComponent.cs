using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Voxel.Engine.Managers;

namespace Voxel.Engine.Entities.Components
{
    public class ChunkLoaderComponent : BaseComponent
    {
        float chunkLoadDistance;

        ChunkManager chunkMgr;

        public ChunkLoaderComponent(BaseEntity parentEntity, int distance) : base(parentEntity)
        {
            this.chunkLoadDistance = distance;
            Initialize();
        }

        protected override string GetName()
        {
            return "ChunkLoader";
        }

        protected override void Initialize()
        {
            chunkMgr = this.Parent.Manager.Game.GetManager("Chunk") as ChunkManager;
            base.Initialize();
        }

        int i = 0;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(chunkMgr == null)
                chunkMgr = this.Parent.Manager.Game.GetManager("Chunk") as ChunkManager;

            if (i % 10 == 0)
            {
                for(int x = (int)(Parent.position.X/32 - chunkLoadDistance); x < (int)(Parent.position.X/32 + chunkLoadDistance); x++)
                {
                    for (int z = (int)(Parent.position.Z / 32 - chunkLoadDistance); z < (int)(Parent.position.Z / 32 + chunkLoadDistance); z++)
                    {
                            if (chunkMgr.GetChunk(new Vector3(x, 0, z)) == null)
                                chunkMgr.AddChunk(new World.Chunk(chunkMgr, new Vector3(x, 0, z)));
                    }
                }
                i = 1;
            }
            else
            {
                i++;
            }
        }
    }
}
