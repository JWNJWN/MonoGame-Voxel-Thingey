using System;
using VoxEngine.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.SceneObject.StandardObjects
{
    public class Cylinder : VoxPrimitive
    {
        public Cylinder(float height, float diameter, Color color, uint tessellation = 3)
        {
            height /= 2;

            float radius = diameter / 2;
            
            for (uint i = 0; i < tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i, tessellation);

                AddVertex(normal * radius + Vector3.Up * height, color);
                AddVertex(normal * radius + Vector3.Down * height, color);

                AddIndex(i * 2);
                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 2) % (tessellation * 2));

                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 3) % (tessellation * 2));
                AddIndex((i * 2 + 2) % (tessellation * 2));
            }
            
            CreateCap(tessellation, height, radius, Vector3.Up, color);
            CreateCap(tessellation, height, radius, Vector3.Down, color);
        }

        void CreateCap(uint tessellation, float height, float radius, Vector3 normal, Color color)
        {
            for (uint i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                }
                else
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                }
            }
            
            for (uint i = 0; i < tessellation; i++)
            {
                Vector3 position = GetCircleVector(i, tessellation) * radius +
                                   normal * height;

                AddVertex(position, color);
            }
        }

        static Vector3 GetCircleVector(uint i, uint tessellation)
        {
            float angle = i * MathHelper.TwoPi / tessellation;

            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            return new Vector3(dx, 0, dz);
        }
    }
}
