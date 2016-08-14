using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.Physics;

namespace Voxel.Engine.Object
{
    public class Primitives
    {

        public static void DrawLine(GraphicsDevice graphicsDevice, Vector3 startPosition, Vector3 endPosition, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[2];
            int[] indices = new int[2] { 0, 1 };

            verts[0].Position = startPosition;
            verts[0].Color = color;

            verts[1].Position = endPosition;
            verts[1].Color = color;



            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, verts, 0, 2, indices, 0, 1);
        }

        public static void DrawPlane(GraphicsDevice graphicsDevice, Vector3 planePosition, Vector3 planeDimensions, bool planeVertical, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[4];
            int[] indices = new int[5] { 0, 1, 2, 3, 0 };

            if (planeVertical)
            {
                verts[0] = new VertexPositionColor(planePosition, color);
                verts[1] = new VertexPositionColor(planePosition + new Vector3(0, planeDimensions.Y, 0), color);
                verts[2] = new VertexPositionColor(planePosition + new Vector3(planeDimensions.X, planeDimensions.Y, planeDimensions.Z), color);
                verts[3] = new VertexPositionColor(planePosition + new Vector3(planeDimensions.X, 0, planeDimensions.Z), color);
            }
            else
            {
                verts[0] = new VertexPositionColor(planePosition, color);
                verts[1] = new VertexPositionColor(planePosition + new Vector3(0, 0, planeDimensions.Z), color);
                verts[2] = new VertexPositionColor(planePosition + new Vector3(planeDimensions.X, 0, planeDimensions.Z), color);
                verts[3] = new VertexPositionColor(planePosition + new Vector3(planeDimensions.X, 0, 0), color);
            }

            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, verts, 0, 4, indices, 0, 4);
        }

        public static void DrawCube(GraphicsDevice graphicsDevice, Vector3 cubePosition, Vector3 cubeDimensions, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[8] {
                new VertexPositionColor(cubePosition, color),
                new VertexPositionColor(cubePosition + new Vector3(0, 0, cubeDimensions.Z), color),
                new VertexPositionColor(cubePosition + new Vector3(cubeDimensions.X, 0, cubeDimensions.Z), color),
                new VertexPositionColor(cubePosition + new Vector3(cubeDimensions.X, 0, 0), color),
                new VertexPositionColor(cubePosition + new Vector3(0, cubeDimensions.Y, 0), color),
                new VertexPositionColor(cubePosition + new Vector3(0, cubeDimensions.Y, cubeDimensions.Z), color),
                new VertexPositionColor(cubePosition + new Vector3(cubeDimensions.X, cubeDimensions.Y, cubeDimensions.Z), color),
                new VertexPositionColor(cubePosition + new Vector3(cubeDimensions.X, cubeDimensions.Y, 0), color)};
            int[] indices = new int[24] { 0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 4, 5, 5, 6, 6, 7, 7, 4, 1, 5, 2, 6, 3, 7};

            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, verts, 0, 8, indices, 0, 12);
        }

        public static void DrawBoundingBox(GraphicsDevice graphicsDevice, BoundingBox boundingBox, Color color)
        {
            DrawCube(graphicsDevice, boundingBox.Min, boundingBox.Max - boundingBox.Min, color);
        }

        public static void DrawAABB(GraphicsDevice graphicsDevice, AABB box, Color color)
        {
            DrawCube(graphicsDevice, box.Min, box.Max-box.Min, color);
        }
    }
}
