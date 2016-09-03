using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.GameComponents;
using VoxEngine.Managers;

namespace VoxelGL.GameScreens
{
    abstract class MenuScreen : GameScreen
    {
        List<MenuEntry> _menuEntries = new List<MenuEntry>();
        protected IList<MenuEntry> MenuEntries
        {
            get { return _menuEntries; }
        }

        int _selectedEntry = 0;
        string _menuTitle;
        
        public MenuScreen(string menuTitle)
        {
            _menuTitle = menuTitle;
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            if(input.MenuUp)
            {
                _selectedEntry--;
                if (_selectedEntry < 0)
                    _selectedEntry = _menuEntries.Count - 1;
            }

            if(input.MenuDown)
            {
                _selectedEntry++;
                if (_selectedEntry >= _menuEntries.Count)
                    _selectedEntry = 0;
            }

            if (input.MenuSelect)
                OnSelectEntry(_selectedEntry);
            else if (input.MenuCancel)
                OnCancel();
        }

        protected virtual void OnSelectEntry(int entryIndex)
        {
            _menuEntries[_selectedEntry].OnSelectEntry();
        }

        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for(int i=0;i<_menuEntries.Count;i++)
            {
                bool isSelected = IsActive & (i == _selectedEntry);
                _menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 position = new Vector2(100, 150);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            ScreenManager.SpriteBatch.Begin();

            for (int i = 0; i < _menuEntries.Count; i++) 
            {
                MenuEntry menuEntry = _menuEntries[i];

                bool isSelected = IsActive && (i == _selectedEntry);
                menuEntry.Draw(this, position, isSelected, gameTime);
                position.Y += menuEntry.GetHeight(this);
            }

            Vector2 titlePosition = new Vector2(426, 80);
            Vector2 titleOrigin = ScreenManager.Font.MeasureString(_menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
        }
    }
}
