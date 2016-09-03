using VoxEngine.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.SceneObject.StandardObjects
{
    public class Quad : VoxPrimitive
    {
        public Quad(Color color) : base(color) { }

        public override void LoadContent()
        {
            Vertices.Add(new VertexPositionColor(new Vector3(0, 0, 0), Color));
            Vertices.Add(new VertexPositionColor(new Vector3(1, 0, 0), Color));
            Vertices.Add(new VertexPositionColor(new Vector3(1, 1, 0), Color));
            Vertices.Add(new VertexPositionColor(new Vector3(0, 1, 0), Color));
            
            Indices.Add(0);
            Indices.Add(1);
            Indices.Add(2);
            Indices.Add(0);
            Indices.Add(2);
            Indices.Add(3);

            PrimitiveType = PrimitiveType.TriangleList;

            base.LoadContent();
        }
    }
}
