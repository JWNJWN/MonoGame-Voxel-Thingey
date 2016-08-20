using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Voxel.Utility
{
    public class MeshData
    {
        List<Vector3> vertices;
        List<int> triangles;

        public MeshData()
        {
            vertices = new List<Vector3>(2 ^ 16);
            triangles = new List<int>(vertices.Count*3);
        }

        public void AddVertex(Vector3 vertex)
        {
            vertices.Add(vertex);
        }

        public void AddTriangle(int triangle)
        {
            triangles.Add(triangle);
        }
    }
}
