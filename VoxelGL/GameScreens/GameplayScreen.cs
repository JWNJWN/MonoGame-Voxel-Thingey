using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VoxEngine.GameComponents;
using VoxEngine.SceneObject.StandardObjects;
using VoxEngine.Managers;
using VoxEngine.Menus;
using VoxEngine.Voxels;

using VoxelGL.GameObjects;
using VoxelGL.GameObjects.Ship;


namespace VoxelGL.GameScreens
{
    class GameplayScreen : GameScreen
    {
        private static double delta = 0.0;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            CameraManager.SetActiveCamera(CameraManager.CameraNumber._default);
            CameraManager.SetAllCamerasProjectionMatrix((float)EngineManager.Device.Viewport.Width / EngineManager.Device.Viewport.Height);

            CameraManager.ActiveCamera.SetPosition(new Vector3(16, 35, 16));
        }

        public override void LoadContent()
        {
            SceneChunkManager.AddChunk(new Vector3(0, 0, 0));

            SceneGraphManager.LoadContent();

            EngineManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            SceneGraphManager.UnloadContent();
        }

        int frameCounter = 0;

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            delta = gameTime.ElapsedGameTime.TotalSeconds;

            FirstPersonCamera camera = (FirstPersonCamera)CameraManager.ActiveCamera;

            frameCounter++;

            byte distance = 10;

            if (frameCounter > 60)
            {
                frameCounter = 0;
                for (int x = -distance; x < distance; x++)
                {
                    for (int z = -distance; z < distance; z++)
                    {
                        if (new Vector2(x, z).Length() < distance)
                        {
                            int newX = (int)Math.Floor(CameraManager.ActiveCamera.Position.X / SceneChunkManager.ChunkSize) + x;
                            int newZ = (int)Math.Floor(CameraManager.ActiveCamera.Position.Z / SceneChunkManager.ChunkSize) + z;
                            SceneChunkManager.AddChunkColumn(new Vector2(newX, newZ));
                        }
                    }
                }

                foreach (var chunk in SceneChunkManager.Chunks.Keys)
                    if (Vector2.Distance(new Vector2(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Z), new Vector2(chunk.X, chunk.Z)*SceneChunkManager.ChunkSize) > (distance+1)*SceneChunkManager.ChunkSize)
                        SceneChunkManager.RemoveChunk(chunk);
            }
        }

        bool _drawDebug = false;

        public override void HandleInput(GameTime gameTime)
        {
            if (EngineManager.Input.PauseGame)
                ScreenManager.AddScreen(new PauseMenuScreen());
            else
            {
                SceneGraphManager.HandleInput(gameTime);

                if (!EngineManager.Input.LastKeyboardState.IsKeyDown(Keys.C) && EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.C))
                    SceneChunkManager.AddChunk(new Vector3((int)Math.Floor(CameraManager.ActiveCamera.Position.X / 32), (int)Math.Floor(CameraManager.ActiveCamera.Position.Y / 32), (int)Math.Floor(CameraManager.ActiveCamera.Position.Z / 32)) * 32);

                if (!EngineManager.Input.LastKeyboardState.IsKeyDown(Keys.F3) && EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.F3))
                    _drawDebug = !_drawDebug;


                float speedMult = 1.0f;

                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
                    speedMult = 25f;

                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.Q))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 30f * (float)delta, 0.0f) * speedMult);
                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.Z))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, -30f * (float)delta, 0.0f) * speedMult);
                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.W))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 0, 10f * (float)delta) * speedMult);
                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.S))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 0, -10f * (float)delta) * speedMult);
                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.D))
                    CameraManager.ActiveCamera.Translate(new Vector3(-10f * (float)delta, 0, 0) * speedMult);
                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.A))
                    CameraManager.ActiveCamera.Translate(new Vector3(10f * (float)delta, 0, 0) * speedMult);


                if (EngineManager.Input.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    //SceneChunkManager.DirtyChunks();
                }

                if (EngineManager.Input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    CameraManager.ActiveCamera.RotateX(-EngineManager.Input.MouseMoved.Y);
                    CameraManager.ActiveCamera.RotateY(EngineManager.Input.MouseMoved.X);
                }

                if (EngineManager.Input.CurrentKeyboardState.IsKeyDown(Keys.F11) && !EngineManager.Input.LastKeyboardState.IsKeyDown(Keys.F11))
                    EngineManager.DeviceManager.ToggleFullScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SceneGraphManager.DrawCulling(gameTime);

            EngineManager.Device.Clear(EngineManager.BackgroundColor);

            EngineManager.Device.BlendState = BlendState.Opaque;
            EngineManager.Device.DepthStencilState = DepthStencilState.DepthRead;

            SceneChunkManager.Draw(gameTime);
            if (_drawDebug)
                SceneChunkManager.DrawDebug(gameTime);

            SceneGraphManager.Draw(gameTime);
        }

        public override void PostUIDraw(GameTime gameTime)
        {
            base.PostUIDraw(gameTime);

            string message =
                "FPS: " + EngineManager.FpsCounter.FPS.ToString() +
                "\nCulled: " + SceneGraphManager.culled.ToString() +
                " Occluded: " + SceneGraphManager.occluded.ToString() +
                "\nCamera " + CameraManager.ActiveCamera.Position.ToString();

            Vector2 textPosition = new Vector2(10, 40);

            Color color = new Color(255, 255, 255, TransitionAlpha);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message, textPosition, color);
            ScreenManager.SpriteBatch.End();
        }
    }
}
