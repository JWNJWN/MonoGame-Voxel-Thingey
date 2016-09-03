using VoxEngine.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.SceneObject.StandardObjects
{
    public class Cube : VoxPrimitive
    {
        public Cube(Color color) : base(color) { }

        public override void LoadContent()
        {
            Vector3[] normals =
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };
            
            foreach (Vector3 normal in normals)
            {
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2 = Vector3.Cross(normal, side1);
                
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);
                
                AddVertex((normal - side1 - side2) * 0.5f, Color);
                AddVertex((normal - side1 + side2) * 0.5f, Color);
                AddVertex((normal + side1 + side2) *  0.5f, Color);
                AddVertex((normal + side1 - side2) * 0.5f, Color);
            }

            PrimitiveType = PrimitiveType.TriangleList;

            base.LoadContent();
        }
    }
}
