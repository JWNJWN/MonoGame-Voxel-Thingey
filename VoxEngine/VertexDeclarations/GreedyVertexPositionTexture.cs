using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoxEngine.VertexDeclarations
{
    public struct GreedyVertexPositionTexture : IVertexType
    {
        Vector3 vertexPosition;
        Vector2 vertexTexture;
        Vector2 vertexOffset;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 3 + sizeof(float) * 2, VertexElementFormat.Vector2, VertexElementUsage.Normal, 0)
        );

        public GreedyVertexPositionTexture(Vector3 position, Vector2 uV, Vector2 offset)
        {
            vertexPosition = position;
            vertexTexture = uV;
            vertexOffset = offset;
        }

        public Vector3 Position
        {
            get { return vertexPosition; }
            set { vertexPosition = value; }
        }

        public Vector2 Texture
        {
            get { return vertexTexture; }
            set { vertexTexture = value; }
        }

        public Vector2 Offset
        {
            get { return vertexOffset; }
            set { vertexOffset = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
