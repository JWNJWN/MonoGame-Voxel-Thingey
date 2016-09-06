using System;
using System.Diagnostics;
using VoxEngine.Managers;
using VoxelGL.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelGL
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            StartUnitTests();
#else
            StartGame();
#endif
        }

        private static void StartUnitTests()
        {
            StartGame();
        }

        private static void StartGame()
        {
            try
            {
                using (EngineManager game = new EngineManager())
                {
                    EngineManager.Game = game;
                    game.RunOneFrame();
                    SetupScene();
                    game.Run();
                }
            }catch(NoSuitableGraphicsDeviceException)
            {
                Debug.WriteLine("Pixel and vertex shaders 2.0 or greater are required.");
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void SetupScene()
        {
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }
    }
}
