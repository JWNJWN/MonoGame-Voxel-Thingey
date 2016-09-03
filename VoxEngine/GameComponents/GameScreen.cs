using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoxEngine.Managers;

namespace VoxEngine.GameComponents
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public abstract class GameScreen
    {
        private bool _isPopup = false;
        public bool IsPopup
        {
            get { return _isPopup; }
            set { _isPopup = value; }
        }

        private TimeSpan _transitionOnTime = TimeSpan.Zero;
        public TimeSpan TransitionOnTime
        {
            get { return _transitionOnTime; }
            set { _transitionOnTime = value; }
        }

        private TimeSpan _transitionOffTime = TimeSpan.Zero;
        public TimeSpan TransitionOffTime
        {
            get { return _transitionOffTime; }
            set { _transitionOffTime = value; }
        }

        private float _transitionPosition = 1;
        public float TransitionPosition
        {
            get { return _transitionPosition; }
            set { _transitionPosition = value; }
        }

        public byte TransitionAlpha
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        private ScreenState _screenState = ScreenState.TransitionOn;
        public ScreenState ScreenState
        {
            get { return _screenState; }
            set { _screenState = value; }
        }

        private bool _isExiting = false;
        public bool IsExiting
        {
            get { return _isExiting; }
            set { _isExiting = value; }
        }
        public bool IsActive
        {
            get
            {
                return !_otherScreenHasFocus &&
                       (_screenState == ScreenState.TransitionOn ||
                        _screenState == ScreenState.Active);
            }
        }

        private bool _otherScreenHasFocus;

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _otherScreenHasFocus = otherScreenHasFocus;

            if(_isExiting)
            {
                _screenState = ScreenState.TransitionOff;
                if(!UpdateTransition(gameTime, _transitionOffTime, 1))
                {
                    ScreenManager.RemoveScreen(this);
                    _isExiting = false;
                }
            }else if(coveredByOtherScreen)
            {
                if (UpdateTransition(gameTime, _transitionOffTime, 1))
                    _screenState = ScreenState.TransitionOff;
                else
                    _screenState = ScreenState.Hidden;
            }else
            {
                if (UpdateTransition(gameTime, _transitionOnTime, -1))
                    _screenState = ScreenState.TransitionOn;
                else
                    _screenState = ScreenState.Active;
            }
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;
            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            _transitionPosition += transitionDelta * direction;

            if((_transitionPosition <= 0) || (_transitionPosition >= 1))
            {
                _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }

            return true;
        }

        public virtual void HandleInput(GameTime gameTime, Input input) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void PostUIDraw(GameTime gameTime) { }

        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
                ScreenManager.RemoveScreen(this);
            else
                _isExiting = true;
        }
    }
}
