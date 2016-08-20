using Voxel.Engine.Managers;
using Voxel.Engine.Entities.Components;

using Microsoft.Xna.Framework;

namespace Voxel.Engine.Entities
{
    public class PlayerEntity : BaseEntity
    {
        public PlayerEntity(SceneManager sceneManager, string entityName, Vector3 position) : base(sceneManager, entityName)
        {
            CameraComponent camComp = new CameraComponent(this);
            FlyControlComponent flyComp = new FlyControlComponent(this, 30, 1.5f);
            MouseLookComponent mouseLookComp = new MouseLookComponent(this, 0.5f);
            ChunkLoaderComponent chunkLoadComp = new ChunkLoaderComponent(this, 16);
            VoxelRayComponent voxRayComp = new VoxelRayComponent(this);

            this.position = position;
        }
    }
}
