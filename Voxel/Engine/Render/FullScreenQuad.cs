using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel.Engine.Render
{
    class FullscreenQuad
    {
        //Vertex Buffer
        VertexBuffer vb;
        //Index Buffer
        IndexBuffer ib;
        //Constructor
        public FullscreenQuad(GraphicsDevice GraphicsDevice)
        {
            //Vertices
            VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };
            //Make Vertex Buffer
            vb = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration,
            vertices.Length, BufferUsage.None);
            vb.SetData<VertexPositionTexture>(vertices);
            //Indices
            ushort[] indices = { 0, 1, 2, 2, 3, 0 };
            //Make Index Buffer
            ib = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits,
           indices.Length, BufferUsage.None);
            ib.SetData<ushort>(indices);
        }
        //Draw and Set Buffers
        public void Draw(GraphicsDevice GraphicsDevice)
        {
            //Set Vertex Buffer
            GraphicsDevice.SetVertexBuffer(vb);
            //Set Index Buffer
            GraphicsDevice.Indices = ib;
            //Draw Quad
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
        //Set Buffers Onto GPU
        public void ReadyBuffers(GraphicsDevice GraphicsDevice)
        {
            //Set Vertex Buffer
            GraphicsDevice.SetVertexBuffer(vb);
            //Set Index Buffer
            GraphicsDevice.Indices = ib;
        }
        public void JustDraw(GraphicsDevice GraphicsDevice)
        {
            //Draw Quad
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
    }
}
