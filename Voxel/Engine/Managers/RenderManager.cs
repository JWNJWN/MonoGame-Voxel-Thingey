using System;
using System.Diagnostics;
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
        private Effect effectDeferredShading;
        private Effect effectDirectionalLight;
        private Effect effectComposite;
        private string currentCameraEntityName = EngineCommon.RootEntityName;

        private RenderTargetBinding[] renderTargets;
        private RenderTarget2D lightMap;

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
            effectDeferredShading = Game.Content.Load<Effect>("Effects/DeferredShading");
            effectDirectionalLight = Game.Content.Load<Effect>("Effects/DirectionalLight");
            effectComposite = Game.Content.Load<Effect>("Effects/Composite");
            effect.CurrentTechnique = effect.Techniques["VoxelEffect"];
            //effect.EnableDefaultLighting();
        }

        public override void LoadContent()
        {
            LoadBasicEffect();

            renderTargets = new RenderTargetBinding[4];

            renderTargets[0] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, false,
                                                       SurfaceFormat.Rgba64, DepthFormat.Depth16));

            renderTargets[1] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, false,
                                                       SurfaceFormat.Rgba64, DepthFormat.Depth16));

            renderTargets[2] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, false,
                                                       SurfaceFormat.Rgba64, DepthFormat.Depth16));

            renderTargets[3] = new RenderTargetBinding(new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, false,
                                                       SurfaceFormat.Rgba64, DepthFormat.Depth16));

            lightMap = new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, false,
                                          SurfaceFormat.Rgba64, DepthFormat.Depth16);
            
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        private void SetupGBuffer(GameTime gameTime)
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

            GraphicsDevice.SetRenderTargets(renderTargets);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            this.GraphicsDevice.Clear(Color.Transparent);


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
                    
                    effectDeferredShading.Parameters["World"].SetValue(desc.worldTransform);
                    effectDeferredShading.Parameters["View"].SetValue(Matrix.CreateLookAt(cameraEntity.position,
                              (cameraEntity.position + cameraEntity.rotation.Forward), Vector3.Up));
                    effectDeferredShading.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(
                              MathHelper.ToRadians(EngineCommon.FOV), camComp.AspectRatio, 0.1f, 5000.0f));

                    effectDeferredShading.CurrentTechnique.Passes["GBuffer"].Apply();

                    int primitiveCount = (desc.geoPrim.Indices.Count / 3);

                    this.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
                }
            }
            GraphicsDevice.SetRenderTargets(null);
        }
        
        private void Lighting()
        {
            BlendState LightMapBS = new BlendState();
            LightMapBS.ColorSourceBlend = Blend.One;
            LightMapBS.ColorDestinationBlend = Blend.One;
            LightMapBS.ColorBlendFunction = BlendFunction.Add;
            LightMapBS.AlphaSourceBlend = Blend.One;
            LightMapBS.AlphaDestinationBlend = Blend.One;
            LightMapBS.AlphaBlendFunction = BlendFunction.Add;

            SceneManager sceneMgr = this.Game.GetManager("Scene") as SceneManager;
            if (sceneMgr == null)
                throw new Exception("Scene Manager not registered properly to the game engine.");
            
            BaseEntity cameraEntity = sceneMgr.GetEntity(currentCameraEntityName);
            if (cameraEntity == null)
                throw new Exception("A camera entity must always exist if we are trying to render a scene");

            CameraComponent camComp = cameraEntity.GetComponent("Camera") as CameraComponent;
            if (camComp == null)
                throw new Exception("An entity was designated as a camera but never given a camera component");

            GraphicsDevice.SetRenderTarget(lightMap);
            GraphicsDevice.Clear(Color.Transparent);

            GraphicsDevice.BlendState = LightMapBS;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            effectDirectionalLight.Parameters["VPI"].SetValue(Matrix.Invert(Matrix.CreateLookAt(cameraEntity.position,
                              (cameraEntity.position + cameraEntity.rotation.Forward), Vector3.Up) * Matrix.CreatePerspectiveFieldOfView(
                              MathHelper.ToRadians(EngineCommon.FOV), camComp.AspectRatio, 0.1f, 5000.0f)));
            effectDirectionalLight.Parameters["VI"].SetValue(Matrix.Invert(Matrix.CreateLookAt(cameraEntity.position,
                              (cameraEntity.position + cameraEntity.rotation.Forward), Vector3.Up)));
            effectDirectionalLight.Parameters["CameraPosition"].SetValue(cameraEntity.position);
            effectDirectionalLight.Parameters["GBufferTextureSize"].SetValue(new Vector2(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight));

            Vector3 lDIr = new Vector3(-0.7f, -1, -0.5f);
            lDIr.Normalize();

            effectDirectionalLight.Parameters["LightDirection"].SetValue(lDIr);
            effectDirectionalLight.Parameters["LightColor"].SetValue(Color.White.ToVector3());
            effectDirectionalLight.Parameters["LightIntensity"].SetValue(1f);

            //effectDirectionalLight.Parameters["texColor"].SetValue(renderTargets[0].RenderTarget);
            effectDirectionalLight.Parameters["texNormal"].SetValue(renderTargets[1].RenderTarget);
            effectDirectionalLight.Parameters["texDepth"].SetValue(renderTargets[2].RenderTarget);


            effectDirectionalLight.CurrentTechnique.Passes[0].Apply();

            RenderQuad(Vector2.One * -1, Vector2.One);

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        VertexPositionTexture[] verts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,0))
                        };

        short[] ib = new short[] { 0, 1, 2, 2, 3, 0 };

        public void RenderQuad(Vector2 v1, Vector2 v2)
        {

            verts[0].Position.X = v2.X;
            verts[0].Position.Y = v1.Y;

            verts[1].Position.X = v1.X;
            verts[1].Position.Y = v1.Y;

            verts[2].Position.X = v1.X;
            verts[2].Position.Y = v2.Y;

            verts[3].Position.X = v2.X;
            verts[3].Position.Y = v2.Y;

            GraphicsDevice.DrawUserIndexedPrimitives
                (PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }

        private void MakeFinal()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            effectComposite.Parameters["texColor"].SetValue(renderTargets[0].RenderTarget);
            effectComposite.Parameters["texLightMap"].SetValue(lightMap);

            effectComposite.Parameters["GBufferTextureSize"].SetValue(new Vector2(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight));

            effectComposite.CurrentTechnique.Passes[0].Apply();

            RenderQuad(Vector2.One * -1, Vector2.One);
        }


        public override void Draw(GameTime gameTime)
        {
            SetupGBuffer(gameTime);

            Lighting();
            MakeFinal();

            /*spriteBatch.Begin(SpriteSortMode.Immediate);

            float numberColumns = 3;
            float numberRows = (float)Math.Ceiling(renderTargets.Length / numberColumns);

            int width = (int)Math.Ceiling(Graphics.PreferredBackBufferWidth / numberColumns);
            int height = (int)Math.Ceiling(Graphics.PreferredBackBufferHeight / numberRows);

            for (int i = 0; i < renderTargets.Length; i++)
                spriteBatch.Draw((Texture2D)renderTargets[i].RenderTarget, new Rectangle((int)(i % numberColumns) * width, (int)(i / numberColumns % numberRows) * height, width, height), Color.White);

            spriteBatch.Draw(lightMap, new Rectangle(width*2, height, width, height), Color.White);
            
            spriteBatch.End();*/
        }
    }
}
