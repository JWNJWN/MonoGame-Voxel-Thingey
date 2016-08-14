using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using Voxel.Engine;
using Voxel.Engine.UI;
using Voxel.Engine.Camera;
using Voxel.Engine.Object;
using Voxel.Engine.World;
using Voxel.Engine.World.Voxel;
using Voxel.Engine.Physics;
using Voxel.Engine.Entities;
using Voxel.Engine.Entities.Components;

namespace Voxel
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D tex;

        BasicEffect groundPlaneEffect;
        BasicEffect debugEffect;

        Effect voxelEffect;

        Matrix worldMatrix;
        Camera camera;

        VoxelIndexer voxelIndex;

        DrawHandler drawHandler;

        Chunk chunk;

        DynamicVertexBuffer vertBuffer;
        DynamicIndexBuffer triBuffer;

        RasterizerState defaultRasterState;
        RasterizerState debugRasterState;

        public Player player;

        Camera MainCamera;

        bool debug = true;

        public float physicsRate = 120;

        List<Entity> entities = new List<Entity>();

        World world;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Voxel Test";
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            Components.Add(new FrameRateCounter(this));
        }

        int groundPlaneSize = 1024;
        VertexPositionTexture[] groundPlaneVertices = new VertexPositionTexture[6];

        int blocksPerGrid = 32;
        
        void InitGroundPlane()
        {
            groundPlaneVertices[0] = new VertexPositionTexture(new Vector3(-groundPlaneSize, 0, -groundPlaneSize), new Vector2(0, 0));
            groundPlaneVertices[1] = new VertexPositionTexture(new Vector3(groundPlaneSize, 0, -groundPlaneSize), new Vector2(groundPlaneSize * (1f / blocksPerGrid), 0));
            groundPlaneVertices[2] = new VertexPositionTexture(new Vector3(-groundPlaneSize, 0, groundPlaneSize), new Vector2(0, groundPlaneSize * (1f / blocksPerGrid)));
            groundPlaneVertices[3] = new VertexPositionTexture(new Vector3(-groundPlaneSize, 0, groundPlaneSize), new Vector2(0, groundPlaneSize * (1f / blocksPerGrid)));
            groundPlaneVertices[4] = new VertexPositionTexture(new Vector3(groundPlaneSize, 0, -groundPlaneSize), new Vector2(groundPlaneSize * (1f / blocksPerGrid), 0));
            groundPlaneVertices[5] = new VertexPositionTexture(new Vector3(groundPlaneSize, 0, groundPlaneSize), new Vector2(groundPlaneSize * (1f / blocksPerGrid), groundPlaneSize * (1f / blocksPerGrid)));
        }

        protected override void Initialize()
        {
            worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);


            InitGroundPlane();

            groundPlaneEffect = new BasicEffect(graphics.GraphicsDevice);
            debugEffect = new BasicEffect(graphics.GraphicsDevice);
            camera = new FlyCam(graphics.GraphicsDevice);

            voxelIndex = new VoxelIndexer();

            world = new World();

            for(int x = 0; x<5;x++)
            {
                for(int z=0;z<5;z++)
                {
                    world.CreateChunk(new Vector3i(x*32, 0, z*32));
                }
            }

            player = new Player(graphics.GraphicsDevice, chunk);
            player.Position = new Vector3(16, 32, 16);
            player.pBoundingBox.Min = player.Position;
            player.game = this;

            entities.Add(player);

            defaultRasterState = new RasterizerState();
            debugRasterState = new RasterizerState();
            debugRasterState.FillMode = FillMode.WireFrame;
            
            vertBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColorNormal), (int)Math.Pow(2,20), BufferUsage.WriteOnly);

            triBuffer = new DynamicIndexBuffer(graphics.GraphicsDevice, IndexElementSize.ThirtyTwoBits, (int)Math.Pow(2, 20)*3, BufferUsage.WriteOnly);

            graphics.ApplyChanges();

            MainCamera = camera;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tex = Content.Load<Texture2D>("Grid");
            spriteFont = Content.Load<SpriteFont>("TestFont");
            voxelEffect = Content.Load<Effect>("Effects/VoxelEffect");

            drawHandler = new DrawHandler(graphics.GraphicsDevice, spriteBatch);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        public static float GetDeltaTime()
        {
            return dt;
        }

        static float dt;
        double oldTime;

        protected override void Update(GameTime gameTime)
        {
            dt = (float)(gameTime.TotalGameTime.TotalSeconds - oldTime);
            oldTime = gameTime.TotalGameTime.TotalSeconds;

            float t = 0;
            t += (1f / gameTime.ElapsedGameTime.Milliseconds);
            if (t >= 1 / physicsRate)
            {
                t = 0;
                //PhysicsUpdate(gameTime);
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (Entity e in entities)
                if(IsActive)
                    //e.Update();
            if(IsActive)
                MainCamera.Update();

            base.Update(gameTime);

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        protected void PhysicsUpdate(GameTime gameTime)
        {
            foreach (PhysicsObject p in entities)
                p.PhysicsUpdate();
        }

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawGroundPlane();
            SetupBuffers();
            DrawChunk();

            if(debug)
                DrawDebug();
            
            //Draw Sprites
            spriteBatch.Begin();

            drawHandler.DrawOutlinedString(spriteFont, "C YAW: " + MainCamera.cAngles.X, Color.Black, Color.White, 1f, new Vector2(0, spriteFont.LineSpacing));
            drawHandler.DrawOutlinedString(spriteFont, "C Pitch: " + MainCamera.cAngles.Y, Color.Black, Color.White, 1f, new Vector2(0, spriteFont.LineSpacing) * 2);
            drawHandler.DrawOutlinedString(spriteFont, "C Pos: " + MainCamera.cPosition.ToString(), Color.Black, Color.White, 1f, new Vector2(0, spriteFont.LineSpacing) * 3);
            
            drawHandler.DrawOutlinedString(spriteFont, "Vert Count: " + vertBuffer.VertexCount, Color.Black, Color.White, 1f, new Vector2(0, spriteFont.LineSpacing) * 5);
            drawHandler.DrawOutlinedString(spriteFont, "Tri Count: " + triBuffer.IndexCount/3, Color.Black, Color.White, 1f, new Vector2(0, spriteFont.LineSpacing) * 6);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        void SetupBuffers()
        {
            Chunk[] chunks = world.GetChunks();
            List<VertexPositionColorNormal> vertList = new List<VertexPositionColorNormal>();
            List<int> triList = new List<int>();
            
            for (int i = 0; i < chunks.Length; i++) 
            {
                int prevTriCount = vertList.Count;
                vertList.AddRange(chunks[i].cVerts);
                for (int j = 0; j < chunks[i].cTris.Count; j++)
                {
                    triList.Add(chunks[i].cTris[j] + prevTriCount);
                }
            }
            
            vertBuffer.SetData(vertList.ToArray());
            
            triBuffer.SetData(triList.ToArray());
        }

        void DrawChunk()
        {
            voxelEffect.Parameters["World"].SetValue(worldMatrix);
            voxelEffect.Parameters["View"].SetValue(MainCamera.ViewMatrix);
            voxelEffect.Parameters["Projection"].SetValue(MainCamera.ProjectionMatrix);
            voxelEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));

            voxelEffect.Parameters["LightDirection"].SetValue(new Vector3(0.8f, -1, 0.6f));
            voxelEffect.Parameters["LightColor"].SetValue(Color.White.ToVector4());
            voxelEffect.Parameters["LightAmbient"].SetValue(Color.Gray.ToVector4());

            voxelEffect.Parameters["Gamma"].SetValue(2.2f);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            GraphicsDevice.RasterizerState = defaultRasterState;

            GraphicsDevice.SetVertexBuffer(vertBuffer);
            GraphicsDevice.Indices = triBuffer;

            foreach (var pass in voxelEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, triBuffer.IndexCount/3);
            }
        }

        void DrawGroundPlane()
        {
            groundPlaneEffect.World = worldMatrix;
            groundPlaneEffect.View = MainCamera.ViewMatrix;
            groundPlaneEffect.Projection = MainCamera.ProjectionMatrix;

            groundPlaneEffect.Texture = tex;
            groundPlaneEffect.TextureEnabled = true;

            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            foreach (var pass in groundPlaneEffect.CurrentTechnique.Passes)
            {
                pass.Apply();


                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, groundPlaneVertices, 0, 2);
            }
        }

        void DrawDebug()
        {
            debugEffect.World = worldMatrix;
            debugEffect.View = MainCamera.ViewMatrix;
            debugEffect.Projection = MainCamera.ProjectionMatrix;

            debugEffect.VertexColorEnabled = true;

            //GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = debugRasterState;

            foreach (var pass in debugEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //Primitives.DrawPlane(graphics.GraphicsDevice, Vector3.Zero, new Vector3(10, 50, 10), true, Color.Red);
                Primitives.DrawAABB(graphics.GraphicsDevice, player.pBoundingBox, Color.Green);
                /*foreach(AABB box in chunk.boundingBoxes)
                    Primitives.DrawAABB(graphics.GraphicsDevice, box, Color.Red);*/
            }
        }
    }
}
