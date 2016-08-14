using Microsoft.Xna.Framework;

using Voxel.Engine.Managers;
using Voxel.Engine.Entities.Components;

namespace Voxel.Engine.Entities
{
    public class ChunkEntity : BaseEntity
    {
        public ChunkEntity(SceneManager sceneManager, Vector3 position) : base(sceneManager, "Chunk " + position.ToString())
        {
            this.position = position;
            VoxelContainerComponent voxContatinerComponent = new VoxelContainerComponent(this, 32);
            VoxelGeneratorComponent voxGeneratorComponent = new VoxelGeneratorComponent(this);
            VoxelRenderComponent voxRenderComponent = new VoxelRenderComponent(this);
        }
    }
}
