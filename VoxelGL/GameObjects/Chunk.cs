using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.SceneObject;
using VoxEngine.Managers;
using VoxEngine.Voxels;

namespace VoxelGL.GameObjects
{
    public class Chunk : VoxSceneObject, IVoxLoadable, IVoxUpdateable, IVoxMesh
    {
        private static List<VertexPositionTexture> _vertices = new List<VertexPositionTexture>();
        public List<VertexPositionTexture> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        private static List<uint> _indices = new List<uint>();
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

        private const byte _size = 32;

        private static ushort[] _voxels;

        private bool _dirty = true;

        public Chunk()
        {
            _voxels = new ushort[_size * _size * _size];
        }

        public void LoadContent()
        {
            _voxels[0] = 0;

            /*_vertexBuffer = new VertexBuffer(EngineManager.Device, VertexPositionColor.VertexDeclaration, _vertices.Count, BufferUsage.None);
            _vertexBuffer.SetData(_vertices.ToArray());
            _indexBuffer = new IndexBuffer(EngineManager.Device, IndexElementSize.ThirtyTwoBits, _indices.Count, BufferUsage.None);
            _indexBuffer.SetData(_indices.ToArray());*/
            ReadyToRender = true;
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
            if(ReadyToRender && !_dirty)
            {
                EngineManager.Device.DepthStencilState = DepthStencilState.Default;

                Effect effect = ShaderManager.GetShader("primitive").BaseEffect;
                effect.Parameters["ModelTexture"].SetValue(TextureManager.GetTexture("dirt").BaseTexture);

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

        private static ushort GetVoxel(int x, int y, int z)
        {
            return _voxels[x + y * _size + z * _size * _size];
        }

        private void GreedyMesh()
        {
            _vertices.Clear();
            _indices.Clear();

            for(bool back = true, b = false; b != back; back = back && b, b = !b)
            {
                for(int d = 0; d < 3; d++)
                {
                    int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                    int[] x = new int[3];
                    int[] q = new int[3];
                    ushort[] mask = new ushort[_size * _size];

                    q[d] = 1;

                    for(x[d] = 0; x[d] < _size;)
                    {
                        int n = 0;
                        for(x[v] = 0; x[v] < _size; x[v]++)
                        {
                            for (x[u] = 0; x[u] < _size; x[u]++) 
                            {
                                ushort vox = 0, vox1 = 0;

                                vox = GetVoxel(x[0], x[1], x[2]);
                                vox1 = GetVoxel(x[0] + q[0], x[1] + q[1], x[2] + q[2]);
                                mask[n++] = (VoxelManager.GetVoxel(vox) != null && VoxelManager.GetVoxel(vox1) != null && VoxelManager.GetVoxel(vox).Transparent == VoxelManager.GetVoxel(vox1).Transparent) ? (ushort)0 : back ? vox1 : vox;
                            }
                        }
                        x[d]++;

                        n = 0;

                        for(j = 0; j < _size; j++)
                        {
                            for(i = 0; i < _size;)
                            {
                                if(mask[n] != 0)
                                {
                                    for (w = 1; i + 1 < _size && mask[n + w] != 0 && mask[n + w] == mask[n]; w++) { }

                                    bool done = false;
                                    for (h = 1; j + h < _size; h++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            if(mask[n+k+h*_size]==0 || mask[n+k+h*_size] != mask[n])
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
            _vertexBuffer = new VertexBuffer(EngineManager.Device, VertexPositionColor.VertexDeclaration, _vertices.Count, BufferUsage.None);
            _vertexBuffer.SetData(_vertices.ToArray());
            _indexBuffer = new IndexBuffer(EngineManager.Device, IndexElementSize.ThirtyTwoBits, _indices.Count, BufferUsage.None);
            _indexBuffer.SetData(_indices.ToArray());
            _dirty = false;
        }

        void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, ushort voxel, Vector3 normal)
        {
            int index = _vertices.Count;

            _vertices.Add(new VertexPositionTexture(v1, new Vector2(0,0)));
            _vertices.Add(new VertexPositionTexture(v2, new Vector2(1, 0)));
            _vertices.Add(new VertexPositionTexture(v3, new Vector2(1, 1)));
            _vertices.Add(new VertexPositionTexture(v4, new Vector2(0, 1)));

            _indices.Add((ushort)(index));
            _indices.Add((ushort)(index + 1));
            _indices.Add((ushort)(index + 2));

            _indices.Add((ushort)(index));
            _indices.Add((ushort)(index + 2));
            _indices.Add((ushort)(index + 3));
        }
    }
}
