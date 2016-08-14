namespace Voxel.Engine.Entities.Components
{
    public class CameraComponent : BaseComponent
    {
        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
        }

        protected override string GetName()
        {
            return "Camera";
        }

        public CameraComponent(BaseEntity ParentEntity) :
            base(ParentEntity)
        {
            Initialize();
        }

        protected override void Initialize()
        {
            aspectRatio = this.Parent.Manager.Game.GraphicsDevice.Viewport.AspectRatio;

            base.Initialize();
        }
    }
}
