using System;
using VoxEngine.Menus;
using VoxEngine.GUI;
using VoxEngine.Managers;
using Microsoft.Xna.Framework;

namespace VoxelGL.GameScreens
{
    class PauseMenuScreen : MenuScreen 
    {
        public PauseMenuScreen() : base("Paused")
        {
            IsPopup = true;
            /*MenuEntry resumeGameMenuEntry = new MenuEntry(0, 0, 10, 10, "Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry(0, 20, 10, 10, "Quit Game");

            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;*/

            GUIButton resumeButton = new GUIButton(_rootPanel, 0.25f, 0.3f, 0.5f, 0.1f, "resumeButton", "Resume", OnCancel);
            GUIButton quitButton = new GUIButton(_rootPanel, 0.25f, 0.5f, 0.5f, 0.1f, "quitButton", "Quit", QuitGameMenuEntrySelected);
        }

        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";
            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);
            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmQuitMessageBox);
        }

        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(false, new BackgroundScreen(), new MainMenuScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }
    }
}
