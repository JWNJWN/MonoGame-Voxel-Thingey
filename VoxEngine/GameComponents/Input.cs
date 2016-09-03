using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxEngine.GameComponents
{
    public class Input : GameComponent
    {

        public KeyboardState CurrentKeyboardState;
        public GamePadState CurrentGamePadState;
        public MouseState CurrentMouseState;

        public KeyboardState LastKeyboardState;
        public GamePadState LastGamePadState;
        public MouseState LastMouseState;

        private Point _lastMouseLocation;

        private Vector2 _mouseMoved;
        public Vector2 MouseMoved
        {
            get { return _mouseMoved; }
        }

        public Input(Game game) :base(game)
        {
            Enabled = true;
        }

        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePadState = CurrentGamePadState;
            LastMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentMouseState = Mouse.GetState();

            _mouseMoved = new Vector2(LastMouseState.X - CurrentMouseState.X, LastMouseState.Y - CurrentMouseState.Y);
            _lastMouseLocation = new Point(CurrentMouseState.X, CurrentMouseState.Y);
        }

        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
                       (CurrentGamePadState.DPad.Up == ButtonState.Pressed &&
                        LastGamePadState.DPad.Up == ButtonState.Released) ||
                       (CurrentGamePadState.ThumbSticks.Left.Y > 0 &&
                        LastGamePadState.ThumbSticks.Left.Y <= 0);
            }
        }
        
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
                       (CurrentGamePadState.DPad.Down == ButtonState.Pressed &&
                        LastGamePadState.DPad.Down == ButtonState.Released) ||
                       (CurrentGamePadState.ThumbSticks.Left.Y < 0 &&
                        LastGamePadState.ThumbSticks.Left.Y >= 0);
            }
        }
        
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       (CurrentGamePadState.Buttons.A == ButtonState.Pressed &&
                        LastGamePadState.Buttons.A == ButtonState.Released) ||
                       (CurrentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Start == ButtonState.Released);
            }
        }
        
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePadState.Buttons.B == ButtonState.Pressed &&
                        LastGamePadState.Buttons.B == ButtonState.Released) ||
                       (CurrentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Back == ButtonState.Released);
            }
        }
        
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Back == ButtonState.Released) ||
                       (CurrentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        LastGamePadState.Buttons.Start == ButtonState.Released);
            }
        }
        
        bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) &&
                    LastKeyboardState.IsKeyUp(key));
        }
    }
}
