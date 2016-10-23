using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.SceneObject;
using VoxEngine.Shaders;
using VoxEngine.Managers;
using VoxEngine.VertexDeclarations;

namespace VoxEngine.Voxels
{
    public class Chunk : VoxSceneObject, IVoxMesh, IVoxUpdateable, IVoxLoadable
    {
        private List<GreedyVertexPositionTexture> _vertices = new List<GreedyVertexPositionTexture>();
        public List<GreedyVertexPositionTexture> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        private List<uint> _indices = new List<uint>();
        public List<uint> Indices
        {
            get { return _indices; }
            set { _indices = value; }
        }

        private VertexBuffer _vertexBuffer;
        public VertexBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
            set { _vertexBuffer = value; }
        }

        private IndexBuffer _indexBuffer;
        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
            set { _indexBuffer = value; }
        }

        private byte _size;
        public byte Size
        {
            get { return _size; }
        }

        private ushort[] _voxels;
        private bool _dirty;
        public bool Dirty
        {
            set { _dirty = value; }
        }

        public Chunk(byte size, Vector3 position)
        {
            _dirty = true;
            _size = size;
            _voxels = new ushort[_size * _size * _size];
            Position = position;
        }

        public void LoadContent()
        {

        }

        public void UnloadContent()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            if (_dirty)
                GreedyMesh();
        }

        public override void Draw(GameTime gameTime)
        {
            if (ReadyToRender && !_dirty)
            {
                EngineManager.Device.DepthStencilState = DepthStencilState.Default;

                Effect effect = ShaderManager.GetShader("chunk").BaseEffect;
                effect.Parameters["ModelTexture"].SetValue(TextureManager.GetTexture("chunk").BaseTexture);

                effect.Parameters["TextureTotalSize"].SetValue(64f);
                effect.Parameters["TextureVoxelSize"].SetValue(16f);

                effect.Parameters["World"].SetValue(World);
                effect.Parameters["View"].SetValue(CameraManager.ActiveCamera.View);
                effect.Parameters["Projection"].SetValue(CameraManager.ActiveCamera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(World)));

                effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(0.5f, -0.7f, 0.6f));

                effect.Parameters["DiffuseIntensity"].SetValue(0.25f);

                effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["AmbientIntensity"].SetValue(0.1f);

                EngineManager.Device.SetVertexBuffer(_vertexBuffer);
                EngineManager.Device.Indices = _indexBuffer;
                
                effect.CurrentTechnique.Passes[0].Apply();
                EngineManager.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Count, 0, _indices.Count / 3);
            }
        }

        public ushort GetVoxel(int x, int y, int z)
        {
            if(InRelativeRange(x,y,z))
                return _voxels[x + y * _size + z * _size * _size];
            return 0;
        }

        public void SetVoxel(int x, int y, int z, ushort voxel)
        {
            if (InRange(x, y, z))
            {
                x -= (int)Position.X;
                y -= (int)Position.Y;
                z -= (int)Position.Z;

                _voxels[x + y * _size + z * _size * _size] = voxel;
                _dirty = true;
            }

        }

        private bool InRange(int x, int y, int z)
        {
            bool isX = x >= Position.X && x < Position.X + _size;
            bool isY = y >= Position.Y && y < Position.Y + _size;
            bool isZ = z >= Position.Z && z < Position.Z + _size;
            return isX && isY && isZ;
        }

        private bool InRelativeRange(int x, int y, int z)
        {
            bool isX = x >= 0 && x < _size;
            bool isY = y >= 0 && y < _size;
            bool isZ = z >= 0 && z < _size;
            return isX && isY && isZ;
        }

        private void GreedyMesh()
        {
            _vertices.Clear();
            _indices.Clear();
            ReadyToRender = false;
            for (bool back = true, b = false; b != back; back = back && b, b = !b)
            {
                for (int d = 0; d < 3; d++)
                {
                    int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                    int[] x = new int[3];
                    int[] q = new int[3];
                    ushort[] mask = new ushort[_size * _size];

                    q[d] = 1;

                    for (x[d] = -1; x[d] < _size;)
                    {
                        int n = 0;
                        for (x[v] = 0; x[v] < _size; x[v]++)
                        {
                            for (x[u] = 0; x[u] < _size; x[u]++)
                            {
                                ushort vox = 0, vox1 = 0;
                                
                                vox = GetVoxel(x[0], x[1], x[2]);
                                vox1 = GetVoxel(x[0] + q[0], x[1] + q[1], x[2] + q[2]);
                                mask[n++] = (VoxelManager.Voxels[vox] != null && VoxelManager.Voxels[vox1] != null && VoxelManager.Voxels[vox].Transparent == VoxelManager.Voxels[vox1].Transparent) ? (ushort)0 : back ? vox1 : vox;
                            }
                        }
                        x[d]++;

                        n = 0;

                        for (j = 0; j < _size; j++)
                        {
                            for (i = 0; i < _size;)
                            {
                                if (mask[n] != 0)
                                {
                                    for (w = 1; i + w < _size && mask[n + w] != 0 && mask[n + w] == mask[n]; w++) { }

                                    bool done = false;
                                    for (h = 1; j + h < _size; h++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            if (mask[n + k + h * _size] == 0 || mask[n + k + h * _size] != mask[n])
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
                                            mask[n + k + l * _size] = 0;
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
            if (_vertices.Count > 0)
            {
                _vertexBuffer = new VertexBuffer(EngineManager.Device, GreedyVertexPositionTexture.VertexDeclaration, _vertices.Count, BufferUsage.None);
                _vertexBuffer.SetData(_vertices.ToArray());
                _indexBuffer = new IndexBuffer(EngineManager.Device, IndexElementSize.ThirtyTwoBits, _indices.Count, BufferUsage.None);
                _indexBuffer.SetData(_indices.ToArray());
                ReadyToRender = true;
            }
            _dirty = false;
        }

        void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, ushort voxel)
        {
            int index = _vertices.Count;

            Vector3 a = v3 - v1;
            Vector2 tile = Vector2.One;

            float absX = Math.Abs(a.X);
            float absY = Math.Abs(a.Y);
            float absZ = Math.Abs(a.Z);

            bool x = absX > 0;
            bool y = absY > 0;
            bool z = absZ > 0;

            if (x && y)
                tile = new Vector2(absX, absY);
            if (x && z)
                tile = new Vector2(absZ, absX);
            if (z && y)
                tile = new Vector2(absY, absZ);

            Console.WriteLine(VoxelManager.Voxels[voxel].Offset);

            _vertices.Add(new GreedyVertexPositionTexture(v1, new Vector2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset));
            _vertices.Add(new GreedyVertexPositionTexture(v2, new Vector2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset));
            _vertices.Add(new GreedyVertexPositionTexture(v3, new Vector2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset));
            _vertices.Add(new GreedyVertexPositionTexture(v4, new Vector2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset));
            
            _indices.Add((ushort)(index));
            _indices.Add((ushort)(index + 1));
            _indices.Add((ushort)(index + 2));

            _indices.Add((ushort)(index));
            _indices.Add((ushort)(index + 2));
            _indices.Add((ushort)(index + 3));
        }
    }
}