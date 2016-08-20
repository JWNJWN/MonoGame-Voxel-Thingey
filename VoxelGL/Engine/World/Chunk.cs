using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.World.Voxel;
using Voxel.Engine.Entities;
using Voxel.Engine.Managers;
using Voxel.Engine.Render;
using Voxel.Engine.GeometricPrimitives;

using SharpNoise.Modules;

namespace Voxel.Engine.World
{
    public class Chunk
    {
        #region Chunk Entity Variables
        private ChunkManager manager;
        private string name;

        public Vector3 position = Vector3.Zero;
        public Matrix scaleMatrix = Matrix.Identity;

        public ChunkManager Manager
        {
            get { return manager; }
        }
        public string Name
        {
            get { return name; }
        }
        #endregion

        #region Chunk Voxel Container Variables
        private byte[] voxels;
        public byte[] Voxels
        {
            get { return voxels; }
        }

        public int containerSize;

        public bool dirty = false;
        #endregion

        #region Chunk Render Variables
        private RenderDescription description;

        List<VertexPositionColorNormal> vertices;
        List<int> indices;
        #endregion

        ///TODO : Add Perlin to ChunkManager and have it be universal
        Perlin perlin;

        public Chunk(ChunkManager chunkManager, Vector3 Position) : this(chunkManager, Position, 32, 1) { }

        public Chunk(ChunkManager chunkManager, Vector3 Position, int ContainerSize, float scale)
        {
            this.manager = chunkManager;
            this.name = Position.ToString();
            this.containerSize = ContainerSize;
            this.scaleMatrix = Matrix.CreateScale(scale);
            this.position = Position * containerSize;

            voxels = new byte[containerSize * containerSize * containerSize];

            vertices = new List<VertexPositionColorNormal>();
            indices = new List<int>();

            description = new RenderDescription();

            description.worldTransform = this.scaleMatrix * Matrix.CreateTranslation(this.position);

            perlin = new Perlin();
            perlin.Seed = 1414;
            perlin.OctaveCount = 1;
            perlin.Lacunarity = 1;
            perlin.Persistence = 1;
            perlin.Frequency = 0.008;
            perlin.Quality = SharpNoise.NoiseQuality.Standard;

            Generate();
        }

        private void Generate()
        {
            for (int x = (int)position.X; x < containerSize + position.X; x++)
            {
                for (int z = (int)position.Z; z < containerSize + position.Z; z++)
                {
                    double pValue = (perlin.GetValue((x + 0.5) * scaleMatrix.Scale.X, 0, (z + 0.5) * scaleMatrix.Scale.Z) + 1) / 2;

                    int height = (int)(pValue * containerSize);

                    for (int y = 0; y < containerSize; y++)
                    {
                        if (y == height)
                            SetVoxel(x, y, z, 1);
                        if (y < height)
                            SetVoxel(x, y, z, 2);
                    }
                }
            }
        }

        public byte GetVoxel(int x, int y, int z)
        {
            if(InRange(x,y,z)) {
                x = (int)mod(x, containerSize);
                y = (int)mod(y, containerSize);
                z = (int)mod(z, containerSize);

                return Voxels[x + y * containerSize + z * containerSize * containerSize];
            }else
            {
                Chunk tempChunk = manager.GetChunk(x, y, z);
                if (tempChunk == null)
                    return 0;
                return tempChunk.GetVoxel(x, y, z);
            }
        }

        public void SetVoxel(int x, int y, int z, byte voxel)
        {
            if(InRange(x,y,z))
            {
                x = (int)mod(x, containerSize);
                y = (int)mod(y, containerSize);
                z = (int)mod(z, containerSize);

                Voxels[x + y * containerSize + z * containerSize * containerSize] = voxel;
                dirty = true;
            }else
            {
                manager.GetChunk(x, y, z).SetVoxel(x, y, z, voxel);
            }
        }

        float mod(float val, int mod)
        {
            return ((val % mod) + mod) % mod;
        }

        private bool InRange(int x, int y, int z)
        {
            bool isX = x >= position.X && x < position.X + containerSize;
            bool isY = y >= position.Y && y < position.Y + containerSize;
            bool isZ = z >= position.Z && z < position.Z + containerSize;
            return isX && isY && isZ;
        }

        private void GreedyMesh()
        {
            vertices.Clear();
            indices.Clear();

            int[] cPos = new int[3] { (int)position.X, (int)position.Y, (int)position.Z };
            for (bool back = true, b = false; b != back; back = back && b, b = !b)
            {
                for (int d = 0; d < 3; d++)
                {
                    int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                    int[] x = new int[3];
                    int[] q = new int[3];
                    byte[] mask = new byte[containerSize * containerSize];

                    q[d] = 1;


                    //Mask Generation
                    for (x[d] = -1; x[d] < containerSize;)
                    {
                        int n = 0;

                        for (x[v] = 0; x[v] < containerSize; x[v]++)
                        {
                            for (x[u] = 0; x[u] < containerSize; x[u]++)
                            {

                                byte vox = 0, vox1 = 0;

                                vox = GetVoxel(x[0] + cPos[0], x[1] + cPos[1], x[2] + cPos[2]);
                                vox1 = GetVoxel(x[0] + q[0] + cPos[0], x[1] + q[1] + cPos[1], x[2] + q[2] + cPos[2]);
                                mask[n++] = (VoxelIndexer.voxelIndex[vox].color.A != 0 && VoxelIndexer.voxelIndex[vox1].color.A != 0 && VoxelIndexer.voxelIndex[vox].color.A == VoxelIndexer.voxelIndex[vox1].color.A) ? (byte)0 : back ? vox1 : vox;
                            }
                        }

                        x[d]++;

                        //Mesh Generation
                        n = 0;

                        for (j = 0; j < containerSize; j++)
                        {
                            for (i = 0; i < containerSize;)
                            {
                                if (mask[n] != 0)
                                {
                                    for (w = 1; i + w < containerSize && mask[n + w] != 0 && mask[n + w] == mask[n]; w++) { }

                                    bool done = false;
                                    for (h = 1; j + h < containerSize; h++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            if (mask[n + k + h * containerSize] == 0 || mask[n + k + h * containerSize] != mask[n])
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

                                    Vector3 a = vPos[1] - vPos[0];
                                    Vector3 a1 = vPos[3] - vPos[0];

                                    Vector3 normal = new Vector3((a.Y * a1.Z) - (a.Z * a1.Y), (a.Z * a1.X) - (a.X * a1.Z), (a.X * a1.Y) - (a.Y * a1.X));
                                    normal.Normalize();

                                    if (back)
                                        DrawFace(vPos[0], vPos[1], vPos[2], vPos[3], mask[n], -normal);
                                    else
                                        DrawFace(vPos[3], vPos[2], vPos[1], vPos[0], mask[n], normal);


                                    for (l = 0; l < h; l++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            mask[n + k + l * containerSize] = 0;
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
            description.geoPrim = new MeshPrimitive(Manager.Game.GraphicsDevice, vertices, indices);
        }

        void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, byte voxel, Vector3 normal)
        {
            int index = vertices.Count;

            vertices.Add(new VertexPositionColorNormal(v1, VoxelIndexer.voxelIndex[voxel].color, normal));
            vertices.Add(new VertexPositionColorNormal(v2, VoxelIndexer.voxelIndex[voxel].color, normal));
            vertices.Add(new VertexPositionColorNormal(v3, VoxelIndexer.voxelIndex[voxel].color, normal));
            vertices.Add(new VertexPositionColorNormal(v4, VoxelIndexer.voxelIndex[voxel].color, normal));

            indices.Add(index);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index);
            indices.Add(index + 2);
            indices.Add(index + 3);
        }

        public void Update(GameTime gameTime)
        {
            if (dirty)
                GreedyMesh();

            description.worldTransform = this.scaleMatrix * Matrix.CreateTranslation(this.position);
        }

        public void Draw(GameTime gameTime, List<RenderDescription> renderDescriptions)
        {
            renderDescriptions.Add(description);
            dirty = false;
        }
    }
}
