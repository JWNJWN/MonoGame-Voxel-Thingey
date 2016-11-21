using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VoxEngine.Types;

namespace VoxEngine.VertexDeclarations
{
    public struct GreedyVertexPositionTexture : IVertexType
    {
        Byte4 vertexPosition;
        //2 Byte2 First-Tile Second-Offset
        Byte4 vertexTextureInfo;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.Position, 0),
            new VertexElement(4, VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 0)
        );

        public GreedyVertexPositionTexture(Byte3 position, Byte2 uV, Byte2 offset)
        {
            vertexPosition = new Byte4(position, 1);
            vertexTextureInfo = new Byte4(uV, offset);
        }

        public Byte4 Position
        {
            get { return vertexPosition; }
            set { vertexPosition = value; }
        }

        public Byte4 Texture
        {
            get { return vertexTextureInfo; }
            set { vertexTextureInfo = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
