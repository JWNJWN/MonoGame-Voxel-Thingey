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
        private static ShipMain shipMain = new ShipMain();

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            
            CameraManager.SetActiveCamera(CameraManager.CameraNumber._default);
            CameraManager.SetAllCamerasProjectionMatrix((float)EngineManager.Device.Viewport.Width / EngineManager.Device.Viewport.Height);
        }

        public override void LoadContent()
        {
            Cube cube = new Cube(Color.White);
            SceneGraphManager.AddObject(cube);

            SceneChunkManager.AddChunk(new Vector3(0, 0, 0));

            SceneGraphManager.LoadContent();

            EngineManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            SceneGraphManager.UnloadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            delta = gameTime.ElapsedGameTime.TotalSeconds;

            FirstPersonCamera camera = (FirstPersonCamera)CameraManager.ActiveCamera;
        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            if (input.PauseGame)
                ScreenManager.AddScreen(new PauseMenuScreen());
            else
            {
                SceneGraphManager.HandleInput(gameTime, input);

                if (!input.LastKeyboardState.IsKeyDown(Keys.C) && input.CurrentKeyboardState.IsKeyDown(Keys.C))
                    SceneChunkManager.AddChunk(CameraManager.ActiveCamera.Position);

                if (input.CurrentKeyboardState.IsKeyDown(Keys.Q))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 30f * (float)delta, 0.0f));
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Z))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, -30f * (float)delta, 0.0f));
                if (input.CurrentKeyboardState.IsKeyDown(Keys.W))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 0, 10f * (float)delta));
                if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
                    CameraManager.ActiveCamera.Translate(new Vector3(0, 0, -10f * (float)delta));
                if (input.CurrentKeyboardState.IsKeyDown(Keys.D))
                    CameraManager.ActiveCamera.Translate(new Vector3(-10f * (float)delta, 0, 0));
                if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
                    CameraManager.ActiveCamera.Translate(new Vector3(10f * (float)delta, 0, 0));

                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    SceneChunkManager.DirtyChunks();
                }

                if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    CameraManager.ActiveCamera.RotateX(-input.MouseMoved.Y);
                    CameraManager.ActiveCamera.RotateY(input.MouseMoved.X);
                }
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

            SceneGraphManager.Draw(gameTime);
        }

        public override void PostUIDraw(GameTime gameTime)
        {
            base.PostUIDraw(gameTime);

            string message = "FPS: " + EngineManager.FpsCounter.FPS.ToString() +
                             " Culled: " + SceneGraphManager.culled.ToString() +
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
