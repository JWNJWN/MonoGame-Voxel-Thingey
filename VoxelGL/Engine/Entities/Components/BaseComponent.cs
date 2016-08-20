using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voxel.Engine.Render;

namespace Voxel.Engine.Entities.Components
{
    abstract public class BaseComponent
    {
        private BaseEntity parent;
        public BaseEntity Parent
        {
            get { return parent; }
        }

        public BaseComponent(BaseEntity parentEntity)
        {
            this.parent = parentEntity;
        }

        public string Name
        {
            get { return this.GetName(); }
        }

        protected abstract string GetName();

        protected virtual void Initialize()
        {
            this.parent.AddComponent(this);
        }

        protected float delta;

        public virtual void Update(GameTime gameTime) { delta = (float)gameTime.ElapsedGameTime.TotalSeconds; }
        public virtual void Draw(GameTime gameTime, List<RenderDescription> renderDescriptions) { }
    }
}
