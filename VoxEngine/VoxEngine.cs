using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using VoxEngine.Settings;
using VoxEngine.Managers;
using VoxEngine.GameComponents;

namespace VoxEngine
{
    public partial class VoxEngine : Game
    {
        protected static int width, height;

        public static int Width
        {
            get { return width; }
        }
        public static int Height
        {
            get { return height; }
        }

        private static float aspectRatio = 1.0f;
        public static float AspectRatio
        {
            get { return aspectRatio; }
        }

        private static Color _backgroundColor = Color.LightBlue;
        public static Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        public static PlatformID CurrentPlatform = Environment.OSVersion.Platform;

        private static string _windowTitle = "";
        public static string WindowTitle
        {
            get { return _windowTitle; }
        }

        public bool _isAppActive = false;

        public bool IsAppActive
        {
            get { return _isAppActive; }
            set { _isAppActive = value; }
        }

        protected static GraphicsDeviceManager _graphicsDeviceManager = null;
        public static GraphicsDevice Device
        {
            get { return _graphicsDeviceManager.GraphicsDevice; }
        }

        protected static ContentManager _contentManager = null;
        public static ContentManager ContentManager
        {
            get { return _contentManager; }
        }

        private static bool _checkedGraphicsOptions = false;
        private static bool _applyDeviceChanges = false;

        private static FpsCounter _fpsCounter = null;
        public static FpsCounter FpsCounter
        {
            get { return _fpsCounter; }
        }

        private static ShaderManager _shaderManager = null;
        private static TextureManager _textureManager = null;
        private static ScreenManager _screenManager = null;
        private static SceneGraphManager _sceneGraphManager = null;
        private static SceneChunkManager _sceneChunkManager = null;
        private static CameraManager _cameraManager = null;
        private static ModelManager _modelManager = null;
        private static VoxelManager _voxelManager = null;

        private static Input _input = null;
        public static Input Input
        {
            get { return _input; }
        }

        protected VoxEngine(string windowTitle)
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            
            GameSettings.Initialize();
            ApplyResolutionChange();
            _graphicsDeviceManager.ApplyChanges();

            _windowTitle = windowTitle;

#if DEBUG
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
#endif
            this.IsFixedTimeStep = false;

            _contentManager = new ContentManager(this.Services);

            _textureManager = new TextureManager(this);
            Components.Add(_textureManager);

            _fpsCounter = new FpsCounter(this);
            Components.Add(_fpsCounter);

            _shaderManager = new ShaderManager(this);
            Components.Add(_shaderManager);
            
            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            _sceneGraphManager = new SceneGraphManager(this);
            Components.Add(_sceneGraphManager);

            _sceneChunkManager = new SceneChunkManager(this);
            Components.Add(_sceneChunkManager);

            _cameraManager = new CameraManager(this);
            Components.Add(_cameraManager);

            _modelManager = new ModelManager(this);
            Components.Add(_modelManager);

            _voxelManager = new VoxelManager(this);
            Components.Add(_voxelManager);
            
            _input = new Input(this);
            Components.Add(_input);
        }

        protected VoxEngine() : this("Game")
        {

        }

        public static void CheckOptionsAndPSVersion()
        {
            if (Device == null)
                throw new InvalidOperationException("Graphics Device is not created yet!");

            _checkedGraphicsOptions = true;
        }

        public static void ApplyResolutionChange()
        {
            int resolutionWidth = GameSettings.Default.ResolutionWidth;
            int resolutionHeight = GameSettings.Default.ResolutionHeight;

            if(resolutionWidth <= 0 || resolutionHeight <= 0)
            {
                resolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            _graphicsDeviceManager.PreferredBackBufferWidth = resolutionWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = resolutionHeight;
            _graphicsDeviceManager.IsFullScreen = GameSettings.Default.Fullscreen;
            _applyDeviceChanges = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphicsDeviceManager.DeviceReset += new EventHandler<EventArgs>(GraphicsDeviceManager_DeviceReset);
            GraphicsDeviceManager_DeviceReset(null, EventArgs.Empty);
        }

        void GraphicsDeviceManager_DeviceReset(object sender, EventArgs e)
        {
            width = _graphicsDeviceManager.GraphicsDevice.Viewport.Width;
            height = _graphicsDeviceManager.GraphicsDevice.Viewport.Height;
            aspectRatio = (float)width / (float)height;
            CameraManager.SetAllCamerasProjectionMatrix(aspectRatio);
        }

        protected override void LoadContent()
        {
            GameSettings.Load();
            //base.LoadContent();
        }

        protected override void UnloadContent()
        {
            GameSettings.Save();
            //base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Device.Clear(BackgroundColor);
            
            base.Draw(gameTime);

            if(_applyDeviceChanges)
            {
                _graphicsDeviceManager.ApplyChanges();
                _applyDeviceChanges = false;
            }
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            IsAppActive = true;
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            IsAppActive = false;
        }
    }
}
