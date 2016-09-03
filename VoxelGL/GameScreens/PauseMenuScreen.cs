using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.Managers;
using Microsoft.Xna.Framework;

namespace VoxelGL.GameScreens
{
    class PauseMenuScreen : MenuScreen 
    {
        public PauseMenuScreen() : base("Paused")
        {
            IsPopup = true;

            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
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
