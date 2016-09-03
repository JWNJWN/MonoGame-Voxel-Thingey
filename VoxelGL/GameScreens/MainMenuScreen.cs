using System;
using System.Collections.Generic;
using System.Text;
using VoxEngine.Managers;

namespace VoxelGL.GameScreens
{
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base("Main Menu")
        {
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(true, new GameplayScreen());
        }

        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            //OptionsMenu
        }

        protected override void OnCancel()
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