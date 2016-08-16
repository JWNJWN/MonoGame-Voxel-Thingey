using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.Render;
using Voxel.Engine.World.Voxel;
using Voxel.Engine.GeometricPrimitives;

namespace Voxel.Engine.Entities.Components
{
    public class VoxelRenderComponent : BaseComponent
    {
        private RenderDescription description;

        private Matrix scaleMatrix = Matrix.Identity;

        VoxelContainerComponent voxContainer = null;

        List<VertexPositionColorNormal> vertices;
        List<int> indices;

        public Matrix Scale
        {
            get { return scaleMatrix; }
            set { scaleMatrix = value; }
        }

        protected override string GetName()
        {
            return "VoxelRender";
        }

        public VoxelRenderComponent(BaseEntity parentEntity) : base(parentEntity)
        {
            Initialize();

            GreedyMesh();
        }

        protected override void Initialize()
        {
            voxContainer = Parent.GetComponent("VoxelContainer") as VoxelContainerComponent;
            if (voxContainer == null)
                throw new Exception("Entity Contains " + GetName() + " but does not contain VoxelContainer.");
            
            vertices = new List<VertexPositionColorNormal>();
            indices = new List<int>();

            description = new RenderDescription();

            description.worldTransform = this.scaleMatrix * this.Parent.rotation * Matrix.CreateTranslation(this.Parent.position);

            base.Initialize();
        }

        void GreedyMesh()
        {
            vertices.Clear();
            indices.Clear();

            for (bool back = true, b = false; b != back; back = back && b, b = !b)
            {
                for (int d = 0; d < 3; d++)
                {
                    int i, j, k, l, w, h, u = (d + 1) % 3, v = (d + 2) % 3;

                    int[] x = new int[3];
                    int[] q = new int[3];
                    byte[] mask = new byte[voxContainer.containerSize * voxContainer.containerSize];

                    q[d] = 1;


                    //Mask Generation
                    for (x[d] = -1; x[d] < voxContainer.containerSize;)
                    {
                        int n = 0;

                        for (x[v] = 0; x[v] < voxContainer.containerSize; x[v]++)
                        {
                            for (x[u] = 0; x[u] < voxContainer.containerSize; x[u]++)
                            {

                                byte vox = 0, vox1 = 0;

                                if (x[d] >= 0) vox = voxContainer.GetVoxel(x[0], x[1], x[2]);
                                if (x[d] < voxContainer.containerSize - 1) vox1 = voxContainer.GetVoxel(x[0] + q[0], x[1] + q[1], x[2] + q[2]);
                                mask[n++] = (VoxelIndexer.voxelIndex[vox].color.A != 0 && VoxelIndexer.voxelIndex[vox1].color.A != 0 && VoxelIndexer.voxelIndex[vox].color.A == VoxelIndexer.voxelIndex[vox1].color.A) ? (byte)0 : back ? vox1 : vox;
                            }
                        }

                        x[d]++;

                        //Mesh Generation
                        n = 0;

                        for (j = 0; j < voxContainer.containerSize; j++)
                        {
                            for (i = 0; i < voxContainer.containerSize;)
                            {
                                if (mask[n] != 0)
                                {
                                    for (w = 1; i + w < voxContainer.containerSize && mask[n + w] != 0 && mask[n + w] == mask[n]; w++) { }

                                    bool done = false;
                                    for (h = 1; j + h < voxContainer.containerSize; h++)
                                    {
                                        for (k = 0; k < w; k++)
                                        {
                                            if (mask[n + k + h * voxContainer.containerSize] == 0 || mask[n + k + h * voxContainer.containerSize] != mask[n])
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
                                            mask[n + k + l * voxContainer.containerSize] = 0;
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
            voxContainer.dirty = false;
            description.geoPrim = new MeshPrimitive(this.Parent.Manager.Game.GraphicsDevice, vertices, indices);
        }

        void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, byte voxel, Vector3 normal)
        {
            //Console.WriteLine(normal.ToString());

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

        public override void Update(GameTime gameTime)
        {
            if(voxContainer.dirty)
                GreedyMesh();

            description.worldTransform = this.scaleMatrix * this.Parent.rotation * Matrix.CreateTranslation(this.Parent.position*scaleMatrix.Scale);
        }

        public override void Draw(GameTime gameTime, List<RenderDescription> renderDescriptions)
        {
            ///TODO: Add Octree culling
            renderDescriptions.Add(description);
        }
    }
}
