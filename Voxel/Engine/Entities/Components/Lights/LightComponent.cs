using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Voxel.Engine.Entities.Components.Lights
{
    public class LightComponent : BaseComponent
    {
        public Vector3 offset;

        public Color color;
        public float intensity;

        public LightType type;

        public LightComponent(BaseEntity parentEntity) : base(parentEntity)
        {
        }

        protected override string GetName()
        {
            return "Light";
        }
    }
}
