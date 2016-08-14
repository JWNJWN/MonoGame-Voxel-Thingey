using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.World.Voxel;
using Voxel.Engine.Physics;
using Voxel.Engine.Entities;
using Voxel.Engine.Managers;

using SharpNoise.Modules;

namespace Voxel.Engine.World
{
    public class Chunk : BaseEntity
    {
        public static int cSize = 32;

        public List<VertexPositionColorNormal> cVerts;
        public List<int> cTris;

        byte[,,] cVoxels;
        
        public List<AABB> boundingBoxes;

        public Chunk(SceneManager sceneManager, string entityName) : base(sceneManager, entityName)
        {
            cVoxels = new byte[cSize, cSize, cSize];

            cVerts = new List<VertexPositionColorNormal>();
            cTris = new List<int>();

            boundingBoxes = new List<AABB>();

            TestFill();
            GreedyMesh();
        }


        /*public Chunk(Vector3 position)
        {
            cPosition = position;

            cVoxels = new byte[cSize, cSize, cSize];

            cVerts = new List<VertexPositionColorNormal>();
            cTris = new List<int>();

            boundingBoxes = new List<AABB>();

            TestFill();
            GreedyMesh();
            GreedyCollision();
        }*/

        public byte GetVoxelByte(int x, int y, int z)
        {
            return cVoxels[x, y, z];
        }
        public byte GetVoxelByte(Vector3i vPos)
        {
            return cVoxels[vPos.x, vPos.y, vPos.z];
        }
        public Vox GetVoxel(int x, int y, int z)
        {
            return VoxelIndexer.voxelIndex[GetVoxelByte(x, y, z)];
        }
        public Vox GetVoxel(Vector3i vPos)
        {
            return VoxelIndexer.voxelIndex[GetVoxelByte(vPos)];
        }
        public Vox GetVoxel(byte vType)
        {
            return VoxelIndexer.voxelIndex[vType];
        }
        public void SetVoxel(int x, int y, int z, byte newVoxel)
        {
            cVoxels[x, y, z] = newVoxel;
        }
        public void SetVoxel(Vector3i vPos, byte newVoxel)
        {
            cVoxels[vPos.x, vPos.y, vPos.z] = newVoxel;
        }

        public bool Contains(int x, int y, int z)
        {
            return x >= 0 && x < cSize && y >= 0 && y < cSize && z >= 0 && z < cSize;
        }
        public bool Contains(Vector3 pos)
        {
            return Contains((int)pos.X, (int)pos.Y, (int)pos.Z);
        }

        Random rng = new Random();

        Perlin perlin = new Perlin();

        void TestFill()
        {
            perlin.Seed = 13123131;
            perlin.OctaveCount = 1;
            perlin.Lacunarity = 1;
            perlin.Persistence = 1;
            perlin.Frequency = 0.05;
            perlin.Quality = SharpNoise.NoiseQuality.Standard;

            for (int x = 0; x < cSize; x++)
            {
                for (int z = 0; z < cSize; z++)
                {
                    double pValue = (perlin.GetValue(x + position.X + 0.5, 0, z + position.Z + 0.5) + 1) / 2;

                    int height = (int)(pValue * cSize);

                    for (int y = 0; y < cSize; y++)
                    {
                        if (y == height)
                            SetVoxel(x, y, z, 1);
                        if (y < height)
                            SetVoxel(x, y, z, 2);
                    }
                }
            }
        }

        void GreedyCollision()
        {
            for (int d = 0; d < 3; d++)
            {
                int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                int[] x = new int[3];
                int[] q = new int[3];

                q[d] = 1;
                for (x[d] = -1; x[d] < cSize;)
                {
                    int n = 0;
                    bool[] mask = new bool[cSize * cSize];
                    bool[] b = new bool[cSize * cSize];
                    for (x[v] = 0; x[v] < cSize; x[v]++)
                    {
                        for (x[u] = 0; x[u] < cSize; x[u]++, n++)
                        {
                            bool vox = false, vox1 = false;

                            if (x[d] >= 0) vox = GetVoxelByte(x[0], x[1], x[2]) != 0;
                            if (x[d] < cSize - 1) vox1 = GetVoxelByte(x[0] + q[0], x[1] + q[1], x[2] + q[2]) != 0;

                            if (vox && vox1)
                            {
                                mask[n] = false;
                                b[n] = false;
                            }
                            else if (vox)
                            {
                                mask[n] = true;
                                b[n] = true;
                            }
                            else if (vox1)
                            {
                                mask[n] = true;
                                b[n] = false;
                            }
                        }
                    }

                    x[d]++;

                    //Mesh Generation
                    n = 0;

                    for (j = 0; j < cSize; j++)
                    {
                        for (i = 0; i < cSize;)
                        {
                            if (mask[n])
                            {
                                for (w = 1; i + w < cSize && mask[n + w] != false && mask[n + w] == mask[n]; w++) { }

                                bool done = false;
                                for (h = 1; j + h < cSize; h++)
                                {
                                    for (k = 0; k < w; k++)
                                    {
                                        if (!mask[n + k + h * cSize] || mask[n + k + h * cSize] != mask[n])
                                        {
                                            done = true;
                                            break;
                                        }
                                    }
                                    if (done) break;
                                }

                                x[u] = i;
                                x[v] = j;

                                int[] du = new int[3];
                                int[] dv = new int[3];

                                du[u] = w;
                                dv[v] = h;

                                Vector3[] vPos = {
                                        new Vector3(x[0], x[1], x[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2])
                                    };


                                Vector3 vect1 = vPos[1] - vPos[0];
                                Vector3 vect2 = vPos[2] - vPos[0];

                                Vector3 normal = Vector3.Cross(vect1, vect2);
                                normal.Normalize();
                                if (b[n])
                                    normal = -normal;
                                vPos[2] += normal;

                                boundingBoxes.Add(new AABB(vPos));

                                for (l = 0; l < h; l++)
                                {
                                    for (k = 0; k < w; k++)
                                    {
                                        mask[n + k + l * cSize] = false;
                                        b[n+k+l*cSize] = false;
                                    }
                                }
                                i += w;
                                n += w;
                            }
                            else
                            {
                                i++;
                                n++;
                            }
                        }

                    }
                }
            }
        }

        void GreedyMesh()
        {
            for (bool back = true, b = false; b != back; back = back && b, b = !b)
            {
                for (int d = 0; d < 3; d++)
                {
                    int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                    int[] x = new int[3];
                    int[] q = new int[3];
                    byte[] mask = new byte[cSize * cSize];

                    q[d] = 1;


                    //Mask Generation
                    for (x[d] = -1; x[d] < cSize;)
                    {
                        int n = 0;

                        for (x[v] = 0; x[v] < cSize; x[v]++)
                        {
                            for (x[u] = 0; x[u] < cSize; x[u]++)
                            {

                                byte vox = 0, vox1 = 0;

                                if (x[d] >= 0) vox = GetVoxelByte(x[0], x[1], x[2]);
                                if (x[d] < cSize - 1) vox1 = GetVoxelByte(x[0] + q[0], x[1] + q[1], x[2] + q[2]);
                                mask[n++] = (GetVoxel(vox).color.A != 0 && GetVoxel(vox1).color.A != 0 && GetVoxel(vox).color.A == GetVoxel(vox1).color.A) ? (byte)0 : back ? vox1 : vox;
                            }
                        }

                        x[d]++;

                        //Mesh Generation
                        n = 0;

                        for (j = 0; j < cSize; j++)
                        {
                            for (i = 0; i < cSize;)
                            {
                                if (mask[n] != 0)
                                {
                                    for (w = 1; i + w < cSize && mask[n + w] != 0 && mask[n + w] == mask[n]; w++) { }

                                    bool done = false;
                                    for (h = 1; j + h < cSize; h++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            if (mask[n + k + h * cSize] == 0 || mask[n + k + h * cSize] != mask[n])
                                            {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done) break;
                                    }

                                    x[u] = i;
                                    x[v] = j;

                                    int[] du = new int[3];
                                    int[] dv = new int[3];

                                    du[u] = w;
                                    dv[v] = h;

                                    Vector3[] vPos = {
                                        new Vector3(x[0], x[1], x[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2])
                                    };

                                    if (back)
                                        DrawFace(vPos[0], vPos[1], vPos[2], vPos[3], mask[n]);
                                    else
                                        DrawFace(vPos[3], vPos[2], vPos[1], vPos[0], mask[n]);


                                    for (l = 0; l < h; l++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            mask[n + k + l * cSize] = 0;
                                        }
                                    }
                                    i += w;
                                    n += w;
                                }
                                else
                                {
                                    i++;
                                    n++;
                                }
                            }

                        }
                    }
                }
            }
        }

        void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, byte voxel)
        {
            Vector3 a = v2 - v1;
            Vector3 b = v3 - v1;

            Vector3 normal = Vector3.Cross(a, b);

            v1 += position;
            v2 += position;
            v3 += position;
            v4 += position;

            int index = cVerts.Count;

            cVerts.Add(new VertexPositionColorNormal(v1, GetVoxel(voxel).color, normal));
            cVerts.Add(new VertexPositionColorNormal(v2, GetVoxel(voxel).color, normal));
            cVerts.Add(new VertexPositionColorNormal(v3, GetVoxel(voxel).color, normal));
            cVerts.Add(new VertexPositionColorNormal(v4, GetVoxel(voxel).color, normal));

            cTris.Add(index);
            cTris.Add(index + 1);
            cTris.Add(index + 2);

            cTris.Add(index);
            cTris.Add(index + 2);
            cTris.Add(index + 3);
        }
    }
}
