using Microsoft.Xna.Framework;

using Voxel.Engine.Managers;
using Voxel.Engine.Entities.Components;

namespace Voxel.Engine.Entities
{
    public class ChunkEntity : BaseEntity
    {
        public ChunkEntity(SceneManager sceneManager, Vector3 position) : base(sceneManager, "Chunk " + position.ToString())
        {
            this.position = position*32;
            VoxelContainerComponent voxContainerComponent = new VoxelContainerComponent(this, 32);
            VoxelRenderComponent voxRenderComponent = new VoxelRenderComponent(this);
            VoxelGeneratorComponent voxGeneratorComponent = new VoxelGeneratorComponent(this);
        }
    }
}
