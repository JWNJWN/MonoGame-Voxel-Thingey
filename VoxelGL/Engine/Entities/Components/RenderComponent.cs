using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.Render;
using Voxel.Engine.GeometricPrimitives;

namespace Voxel.Engine.Entities.Components
{
    public class RenderComponent : BaseComponent
    {
        private RenderDescription description;

        private Matrix scaleMatrix = Matrix.Identity;
        public Matrix Scale
        {
            get { return scaleMatrix; }
            set { scaleMatrix = value; }
        }
        
        protected override string GetName()
        {
            return "Render";
        }

        public RenderComponent(BaseEntity parentEntity, string modelName) : base(parentEntity)
        {
            Initialize();

            LoadModel(modelName);
        }

        public RenderComponent(BaseEntity parentEntity, GeometricPrimitiveType primitiveType) : base(parentEntity)
        {
            Initialize();

            LoadPrimitive(primitiveType, Color.WhiteSmoke);
        }

        public RenderComponent(BaseEntity parentEntity, GeometricPrimitiveType primitiveType, Color color) : base(parentEntity)
        {
            Initialize();

            LoadPrimitive(primitiveType, color);
        }

        protected override void Initialize()
        {
            description = new RenderDescription();

            description.worldTransform = this.scaleMatrix * this.Parent.rotation * Matrix.CreateTranslation(this.Parent.position);

            base.Initialize();
        }

        private void LoadModel(string modelName)
        {
            description.model = this.Parent.Manager.Content.Load<Model>(modelName);
        }

        private void LoadPrimitive(GeometricPrimitiveType primitiveType, Color color)
        {
            switch (primitiveType)
            {
                case GeometricPrimitiveType.Cube:
                    description.geoPrim = new CubePrimitive(this.Parent.Manager.Game.GraphicsDevice, color);
                    break;
                case GeometricPrimitiveType.Sphere:
                    description.geoPrim = new SpherePrimitive(this.Parent.Manager.Game.GraphicsDevice, color);
                    break;
                case GeometricPrimitiveType.Cylinder:
                    description.geoPrim = new CylinderPrimitive(this.Parent.Manager.Game.GraphicsDevice, color);
                    break;
                case GeometricPrimitiveType.Torus:
                    description.geoPrim = new TorusPrimitive(this.Parent.Manager.Game.GraphicsDevice, color);
                    break;
                case GeometricPrimitiveType.Teapot:
                    description.geoPrim = new TeapotPrimitive(this.Parent.Manager.Game.GraphicsDevice);
                    break;
                default:
                    throw new Exception("LoadPrimitive does not handle this type of GeometricPrimitive. Was a new primitive type made and not handled here?");
            }
        }

        public override void Update(GameTime gameTime)
        {
            description.worldTransform = this.scaleMatrix * this.Parent.rotation * Matrix.CreateTranslation(this.Parent.position);
        }

        public override void Draw(GameTime gameTime, List<RenderDescription> renderDescriptions)
        {
            // Could do frustum culling here if we wanted to be more efficient.
            // We could go a step further and gather the entities within sections of an octree that are
            // currently within the view frustum, then skip the frustum check here.

            renderDescriptions.Add(description);
        }
    }
}
