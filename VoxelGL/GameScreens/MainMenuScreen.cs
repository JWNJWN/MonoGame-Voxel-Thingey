using System;
using VoxEngine.Managers;
using VoxEngine.Menus;
using VoxEngine.GUI;

namespace VoxelGL.GameScreens
{
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base("Main Menu")
        {
            GUIPanel buttonPanel = new GUIPanel(_rootPanel, 0.3f, 0.4f, 0.4f, 0.5f, "buttonPanel");

            GUIButton playButton = new GUIButton(buttonPanel, 0f, 0f, 1f, 0.1f, "playButton", "Play", (object sender, EventArgs e) =>
            {
                LoadingScreen.Load(true, new GameplayScreen());
            });
        }

        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            //OptionsMenu
        }

        protected override void OnCancel(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen messageBox = new MessageBoxScreen(message);
            messageBox.Accepted += ExitMessageBoxAccepted;
            ScreenManager.AddScreen(messageBox);
        }

        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            EngineManager.Game.Exit();
        }
    }
}