using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.SceneObject;
using VoxEngine.SceneObject.StandardObjects;
using VoxEngine.Shaders;
using VoxEngine.Managers;
using VoxEngine.VertexDeclarations;
using VoxEngine.Types;
using VoxEngine.Helpers;

using System.Diagnostics;

namespace VoxEngine.Voxels
{
    public class Chunk : VoxSceneObject, IVoxLoadable
    {
        private List<GreedyVertexPositionTexture> _vertices = new List<GreedyVertexPositionTexture>();
        public List<GreedyVertexPositionTexture> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        private List<int> _indices = new List<int>();
        public List<int> Indices
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
        int _indexCount;

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

        private int _voxelCount;
        public int VoxelCount
        {
            get { return _voxelCount; }
            set { _voxelCount = value; }
        }

        private bool UpdateBuffers = false;

        private bool _editing = false;
        public bool Editing
        {
            get { return _editing; }
            set { _editing = value; }
        }

        public Chunk(byte size, Vector3 position)
        {
            _dirty = false;
            _size = size;
            _voxels = new ushort[_size * _size * _size];
            Position = position * _size;
        }

        public void LoadContent()
        {
        }

        public void UnloadContent()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        public void Update(object obj)
        {
            GameTime gameTime = (GameTime)obj;
            if (_dirty && !_editing)
            {
                //GreedyMesh();
                SceneChunkManager._chunkManagerMeshList.Add(GreedyMesh);
                _dirty = false;
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (UpdateBuffers && _vertices.Count > 0)
            {
                _vertexBuffer = new VertexBuffer(EngineManager.Device, GreedyVertexPositionTexture.VertexDeclaration, _vertices.Count, BufferUsage.None);
                _indexBuffer = new IndexBuffer(EngineManager.Device, IndexElementSize.ThirtyTwoBits, _indices.Count, BufferUsage.None);
                _vertexBuffer.SetData(_vertices.ToArray());
                _indexBuffer.SetData(_indices.ToArray());
                _indexCount = _indices.Count;
                UpdateBuffers = false;
                ReadyToRender = true;
            }

            if (ReadyToRender && _voxelCount > 0)
            {
                EngineManager.Device.DepthStencilState = DepthStencilState.Default;

                Effect effect = ShaderManager.GetShader("chunk").BaseEffect;
                effect.Parameters["ModelTexture"].SetValue(TextureManager.GetTexture("chunk").BaseTexture);

                effect.Parameters["TextureTotalSize"].SetValue(64f);
                effect.Parameters["TextureVoxelSize"].SetValue(16f);

                effect.Parameters["Gamma"].SetValue(1.8f);

                effect.Parameters["World"].SetValue(World);
                effect.Parameters["View"].SetValue(CameraManager.ActiveCamera.View);
                effect.Parameters["Projection"].SetValue(CameraManager.ActiveCamera.Projection);

                effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(0.5f, -0.7f, 0.6f));

                effect.Parameters["DiffuseIntensity"].SetValue(0.25f);

                effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["AmbientIntensity"].SetValue(0.1f);

                EngineManager.Device.SetVertexBuffer(_vertexBuffer);
                EngineManager.Device.Indices = _indexBuffer;

                effect.CurrentTechnique.Passes[0].Apply();
                EngineManager.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indexCount / 3);
            }
        }

        public void DrawDebug(GameTime gameTime)
        {
        }

        public ushort GetVoxel(int x, int y, int z)
        {
            if (InRange(x, y, z))
            {
                x = (int)MathsHelper.mod(x, _size);
                y = (int)MathsHelper.mod(y, _size);
                z = (int)MathsHelper.mod(z, _size);

                return _voxels[x + y * _size + z * _size * _size];
            }
            else
            {
                return SceneChunkManager.GetVoxel(x, y, z);
            }
        }

        public void SetVoxel(int x, int y, int z, ushort voxel)
        {
            if (InRange(x, y, z))
            {
                x = (int)MathsHelper.mod(x, _size);
                y = (int)MathsHelper.mod(y, _size);
                z = (int)MathsHelper.mod(z, _size);

                if (_voxels[x + y * _size + z * _size * _size] == 0 && voxel != 0)
                    _voxelCount++;
                else if (_voxels[x + y * _size + z * _size * _size] != 0 && voxel == 0)
                    _voxelCount--;

                _voxels[x + y * _size + z * _size * _size] = voxel;
                _dirty = true;
            }
            else
            {
                SceneChunkManager.SetVoxel(x, y, z, voxel);
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
            UpdateBuffers = false;
            _vertices.Clear();
            _indices.Clear();
            if (_voxelCount > 0)
            {
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

                                    vox = GetVoxel(x[0] + (int)Position.X, x[1] + (int)Position.Y, x[2] + (int)Position.Z);
                                    vox1 = GetVoxel(x[0] + q[0] + (int)Position.X, x[1] + q[1] + (int)Position.Y, x[2] + q[2] + (int)Position.Z);
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


                                        Byte3[] vPos = {
                                            new Byte3((byte)x[0], (byte)x[1], (byte)x[2]),
                                            new Byte3((byte)(x[0] + du[0]), (byte)(x[1] + du[1]), (byte)(x[2] + du[2])),
                                            new Byte3((byte)(x[0] + du[0] + dv[0]), (byte)(x[1] + du[1] + dv[1]), (byte)(x[2] + du[2] + dv[2])),
                                            new Byte3((byte)(x[0] + dv[0]), (byte)(x[1] + dv[1]), (byte)(x[2] + dv[2]))
                                        };

                                        DrawFace(vPos[0], vPos[1], vPos[2], vPos[3], mask[n], back);


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
            }
            UpdateBuffers = true;
        }

        void DrawFace(Byte3 v1, Byte3 v2, Byte3 v3, Byte3 v4, ushort voxel, bool back)
        {
            int index = _vertices.Count;

            Vector3 a = v3.ToVector3() - v1.ToVector3();
            Byte2 tile = Byte2.One;

            byte absX = (byte)Math.Abs(a.X);
            byte absY = (byte)Math.Abs(a.Y);
            byte absZ = (byte)Math.Abs(a.Z);

            bool x = absX > 0;
            bool y = absY > 0;
            bool z = absZ > 0;

            if (x && y)
            {
                tile = new Byte2(absX, absY);
                if (back)
                {
                    _vertices.Add(new GreedyVertexPositionTexture(v3, new Byte2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset[2]));
                    _vertices.Add(new GreedyVertexPositionTexture(v4, new Byte2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset[2]));
                    _vertices.Add(new GreedyVertexPositionTexture(v1, new Byte2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset[2]));
                    _vertices.Add(new GreedyVertexPositionTexture(v2, new Byte2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset[2]));
                }
                else
                {
                    _vertices.Add(new GreedyVertexPositionTexture(v4, new Byte2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset[3]));
                    _vertices.Add(new GreedyVertexPositionTexture(v3, new Byte2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset[3]));
                    _vertices.Add(new GreedyVertexPositionTexture(v2, new Byte2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset[3]));
                    _vertices.Add(new GreedyVertexPositionTexture(v1, new Byte2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset[3]));
                }
            }
            else if (z && y)
            {
                tile = new Byte2(absZ, absY);
                if (back)
                {
                    _vertices.Add(new GreedyVertexPositionTexture(v2, new Byte2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset[4]));
                    _vertices.Add(new GreedyVertexPositionTexture(v3, new Byte2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset[4]));
                    _vertices.Add(new GreedyVertexPositionTexture(v4, new Byte2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset[4]));
                    _vertices.Add(new GreedyVertexPositionTexture(v1, new Byte2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset[4]));
                }
                else
                {
                    _vertices.Add(new GreedyVertexPositionTexture(v3, new Byte2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset[5]));
                    _vertices.Add(new GreedyVertexPositionTexture(v2, new Byte2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset[5]));
                    _vertices.Add(new GreedyVertexPositionTexture(v1, new Byte2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset[5]));
                    _vertices.Add(new GreedyVertexPositionTexture(v4, new Byte2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset[5]));
                }
            }
            else
            {
                tile = new Byte2(absX, absZ);
                if (back)
                {
                    _vertices.Add(new GreedyVertexPositionTexture(v2, new Byte2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset[1]));
                    _vertices.Add(new GreedyVertexPositionTexture(v3, new Byte2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset[1]));
                    _vertices.Add(new GreedyVertexPositionTexture(v4, new Byte2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset[1]));
                    _vertices.Add(new GreedyVertexPositionTexture(v1, new Byte2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset[1]));
                }
                else
                {
                    _vertices.Add(new GreedyVertexPositionTexture(v3, new Byte2(0, 0) * tile, VoxelManager.Voxels[voxel].Offset[0]));
                    _vertices.Add(new GreedyVertexPositionTexture(v2, new Byte2(1, 0) * tile, VoxelManager.Voxels[voxel].Offset[0]));
                    _vertices.Add(new GreedyVertexPositionTexture(v1, new Byte2(1, 1) * tile, VoxelManager.Voxels[voxel].Offset[0]));
                    _vertices.Add(new GreedyVertexPositionTexture(v4, new Byte2(0, 1) * tile, VoxelManager.Voxels[voxel].Offset[0]));
                }
            }

            _indices.Add((index));
            _indices.Add((index + 1));
            _indices.Add((index + 2));

            _indices.Add((index));
            _indices.Add((index + 2));
            _indices.Add((index + 3));
        }
    }
}