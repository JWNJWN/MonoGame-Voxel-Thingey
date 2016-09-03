using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using VoxEngine.Helpers;

namespace VoxEngine.Settings
{
    [Serializable]
    public class GameSettings
    {
        private string _playerName = "JWN";
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                if (_playerName != value)
                    _needSave = true;
                _playerName = value;
            }
        }

        public const int MinimumResolutionWidth = 640;

        private int _resolutionWidth = 0;
        public int ResolutionWidth
        {
            get { return _resolutionWidth; }
            set
            {
                if (_resolutionWidth != value)
                    _needSave = true;
                _resolutionWidth = value;
            }
        }

        public const int MinimumResolutionHeight = 480;

        private int _resolutionHeight = 0;
        public int ResolutionHeight
        {
            get { return _resolutionHeight; }
            set
            {
                if (_resolutionHeight != value)
                    _needSave = true;
                _resolutionHeight = value;
            }
        }

        private bool _fullscreen = false;
        public bool Fullscreen
        {
            get { return _fullscreen; }
            set
            {
                if (_fullscreen != value)
                    _needSave = true;
                _fullscreen = value;
            }
        }

        private bool _postScreenEffects = true;
        public bool PostScreenEffects
        {
            get { return _postScreenEffects; }
            set
            {
                if (_postScreenEffects != value)
                    _needSave = true;
                _postScreenEffects = value;
            }
        }

        private bool _reflections = true;
        public bool Reflections
        {
            get { return _reflections; }
            set
            {
                if (_reflections != value)
                    _needSave = true;
                _reflections = value;
            }
        }

        private bool _refractions = true;
        public bool Refractions
        {
            get { return _refractions; }
            set
            {
                if (_refractions != value)
                    _needSave = true;
                _refractions = value;
            }
        }

        private bool _shadowMapping = true;
        public bool ShadowMapping
        {
            get { return _shadowMapping; }
            set
            {
                if (_shadowMapping != value)
                    _needSave = true;
                _shadowMapping = value;
            }
        }

        private bool _highDetail = true;
        public bool HighDetail
        {
            get { return _highDetail; }
            set
            {
                if (_highDetail != value)
                    _needSave = true;
                _highDetail = value;
            }
        }

        private float _soundVolume = 0.8f;
        public float SoundVolume
        {
            get { return _soundVolume; }
            set
            {
                if (_soundVolume != value)
                    _needSave = true;
                _soundVolume = value;
            }
        }

        private float _musicVolume = 0.6f;
        public float MusicVOlume
        {
            get { return _musicVolume; }
            set
            {
                if (_musicVolume != value)
                    _needSave = true;
                _musicVolume = value;
            }
        }

        private Vector2 _mouseSensitivity = Vector2.One / 2;

        public Vector2 MouseSensitivity
        {
            get { return _mouseSensitivity; }
            set
            {
                if (_mouseSensitivity != value)
                    _needSave = true;
                _mouseSensitivity = value;
            }
        }

        private bool _invertMouse = false;
        public bool InvertMouse
        {
            get { return _invertMouse; }
            set
            {
                if (_invertMouse != value)
                    _needSave = true;
                _invertMouse = value;
            }
        }

        private static bool _needSave = false;

        const string SettingsFilename = "Settings.xml";
        private static GameSettings _defaultInstance = null;
        public static GameSettings Default
        {
            get { return _defaultInstance; }
        }

        private GameSettings() { }

        public static void Initialize()
        {
            _defaultInstance = new GameSettings();
            Load();
        }

        public static void Load()
        {
            _needSave = false;

            FileStream file = FileHelper.LoadGameContentFile(SettingsFilename);

            if(file == null)
            {
                _needSave = true;
                return;
            }

            if(file.Length == 0)
            {
                file.Close();

                file = FileHelper.LoadGameContentFile(SettingsFilename);
                if(file != null)
                {
                    GameSettings loadedGameSettings = (GameSettings)new XmlSerializer(typeof(GameSettings)).Deserialize(file);
                    if (loadedGameSettings != null)
                        _defaultInstance = loadedGameSettings;

                    file.Close();
                }
                _needSave = true;
                Save();
            }else
            {
                GameSettings loadedGameSettings = (GameSettings)new XmlSerializer(typeof(GameSettings)).Deserialize(file);
                if (loadedGameSettings != null)
                    _defaultInstance = loadedGameSettings;

                file.Close();
            }
        }

        public static void Save()
        {
            if (!_needSave)
                return;

            _needSave = false;

            FileStream file = FileHelper.SaveGameContentFile(SettingsFilename);
            new XmlSerializer(typeof(GameSettings)).Serialize(file, _defaultInstance);

            file.Close();
        }

        public static void SetMinimumGraphics()
        {
            GameSettings.Default.ResolutionWidth = GameSettings.MinimumResolutionWidth;
            GameSettings.Default.ResolutionHeight = GameSettings.MinimumResolutionHeight;
            GameSettings.Default.Reflections = false;
            GameSettings.Default.Refractions = false;
            GameSettings.Default.ShadowMapping = false;
            GameSettings.Default.HighDetail = false;
            GameSettings.Default.PostScreenEffects = false;
            GameSettings.Save();
        }
    }
}
