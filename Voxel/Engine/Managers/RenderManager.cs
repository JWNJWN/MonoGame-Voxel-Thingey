using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Voxel.Engine.Entities;
using Voxel.Engine.Entities.Components;
using Voxel.Engine.Render;

using Voxel.Engine.World;

namespace Voxel.Engine.Managers
{
    public class RenderManager : BaseManager
    {
        private SpriteBatch spriteBatch;
        private Effect effect;
        private string currentCameraEntityName = EngineCommon.RootEntityName;

        public RenderManager(SceneGame game) : base(game)
        {
            Initialize();
        }

        protected override string GetName()
        {
            return "Render";
        }

        public GraphicsDeviceManager Graphics
        {
            get { return this.Game.Graphics; }
            set { this.Game.Graphics = value; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return this.Game.GraphicsDevice; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        private void LoadBasicEffect()
        {
            effect = Game.Content.Load<Effect>("Effects/VoxelEffect");
            //effect.EnableDefaultLighting();
        }

        public override void LoadContent()
        {
            LoadBasicEffect();

            spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            SceneManager sceneMgr = this.Game.GetManager("Scene") as SceneManager;
            if (sceneMgr == null)
                throw new Exception("Scene Manager not registered properly to the game engine.");

            ChunkManager chunkMgr = this.Game.GetManager("Chunk") as ChunkManager;
            if (chunkMgr == null)
                throw new Exception("Chunk Manager not registered properly to the game engine.");

            BaseEntity cameraEntity = sceneMgr.GetEntity(currentCameraEntityName);
            if (cameraEntity == null)
                throw new Exception("A camera entity must always exist if we are trying to render a scene");

            CameraComponent camComp = cameraEntity.GetComponent("Camera") as CameraComponent;
            if (camComp == null)
                throw new Exception("An entity was designated as a camera but never given a camera component");

            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            List<RenderDescription> renderDescriptions = new List<RenderDescription>();

            // Get all entities in the scene
            Dictionary<string, BaseEntity> entities = sceneMgr.Entities;

            foreach (KeyValuePair<string, BaseEntity> pair in entities)
            {
                (pair.Value).Draw(gameTime, renderDescriptions);
            }

            Dictionary<Vector3, Chunk> chunks = chunkMgr.Chunks;

            foreach (KeyValuePair<Vector3, Chunk> pair in chunks)
            {
                (pair.Value).Draw(gameTime, renderDescriptions);
            }

            foreach (RenderDescription desc in renderDescriptions)
            {
                if (null != desc.model)
                {
                    // Copy any parent transforms.
                    Matrix[] transforms = new Matrix[desc.model.Bones.Count];
                    desc.model.CopyAbsoluteBoneTransformsTo(transforms);

                    // Draw the model. A model can have multiple meshes, so loop.
                    foreach (ModelMesh mesh in desc.model.Meshes)
                    {
                        // This is where the mesh orientation is set, as well 
                        // as our camera and projection.
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.World = desc.worldTransform;
                            effect.View = Matrix.CreateLookAt(cameraEntity.position,
                                Vector3.Zero, Vector3.Up);
                            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                                MathHelper.ToRadians(EngineCommon.FOV), camComp.AspectRatio, 0.1f, 5000.0f);
                        }
                        // Draw the mesh, using the effects set above.
                        mesh.Draw();
                    }
                }
                else if (null != desc.geoPrim)
                {
                    // Set our vertex declaration, vertex buffer, and index buffer.
                    this.Game.GraphicsDevice.SetVertexBuffer(desc.geoPrim.VertexBuffer);
                    this.Game.GraphicsDevice.Indices = desc.geoPrim.IndexBuffer;

                    foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
                    {
                        effect.Parameters["World"].SetValue(desc.worldTransform);
                        effect.Parameters["View"].SetValue(Matrix.CreateLookAt(cameraEntity.position,
                                  (cameraEntity.position + cameraEntity.rotation.Forward), Vector3.Up));
                        effect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(
                                  MathHelper.ToRadians(EngineCommon.FOV), camComp.AspectRatio, 0.1f, 5000.0f));
                        effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(desc.worldTransform)));

                        effect.Parameters["LightDirection"].SetValue(new Vector3(0.8f, -1, 0.6f));
                        effect.Parameters["LightColor"].SetValue(Color.White.ToVector4());
                        effect.Parameters["LightAmbient"].SetValue(Color.Gray.ToVector4());

                        effect.Parameters["Gamma"].SetValue(2.2f);

                        effectPass.Apply();
                        
                        int primitiveCount = (desc.geoPrim.Indices.Count / 3);

                        this.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
                    }
                }
            }
        }
    }
}
