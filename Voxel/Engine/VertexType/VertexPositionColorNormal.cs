using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Graphics
{
    public struct VertexPositionColorNormal :  IVertexType
    {

        Vector3 vertexPosition;
        Color vertexColor;
        Vector3 vertexNormal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal)
        {
            vertexPosition = position;
            vertexColor = color;
            vertexNormal = normal;
        }

        public Vector3 Position
        {
            get { return vertexPosition; }
            set { vertexPosition = value; }
        }

        public Color Color
        {
            get { return vertexColor; }
            set { vertexColor = Color; }
        }

        public Vector3 Normal
        {
            get { return vertexNormal; }
            set { vertexNormal = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
