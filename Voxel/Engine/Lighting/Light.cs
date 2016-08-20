using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Voxel.Engine
{
    public enum LightType
    {
        Directional,
        Spot,
        Point
    };

    public struct Light
    {
        public Vector3 position;
        public Matrix rotation;

        public Color color;
        public float intensity;
        public LightType type;

        public Light(Vector3 position, Matrix rotation, Color color, float intensity, LightType type) {
            this.position = position;
            this.rotation = rotation;

            this.color = color;
            this.intensity = intensity;
            this.type = type;
        }
    }
}
