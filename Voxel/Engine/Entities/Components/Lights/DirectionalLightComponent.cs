using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Voxel.Engine.Entities.Components.Lights
{
    public class DirectionalLightComponent : LightComponent
    {
        public DirectionalLightComponent(BaseEntity parentEntity, Vector3 offset, Color color, float intensity) : base(parentEntity)
        {
            this.offset = offset;

            this.color = color;
            this.intensity = intensity;

            type = LightType.Directional;
        }
    }
}
