using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.Textures;

namespace VoxEngine.Managers
{
    public class TextureManager : GameComponent
    {
        private static Dictionary<string, IVoxTexture> _textures = new Dictionary<string, IVoxTexture>();

        private static bool _initialized = false;
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static int _texturesLoaded = 0;
        public static int TexturesLoaded
        {
            get { return _texturesLoaded; }
        }

        public TextureManager(Game game) : base(game)
        {

        }

        public static void AddTexture(IVoxTexture newTexture, string textureName)
        {
            if(textureName != null && !_textures.ContainsKey(textureName))
            {
                _textures.Add(textureName, newTexture);
                if (_initialized)
                {
                    ThreadStart threadStarter = delegate
                    {
                        newTexture.LoadContent();
                        _texturesLoaded++;
                    };
                    Thread loadingThread = new Thread(threadStarter);
                    loadingThread.Start();
                }
            }
        }

        public static void RemoveTexture(string textureName)
        {
            if(textureName != null && _textures.ContainsKey(textureName))
            {
                if(_initialized)
                {
                    ThreadStart threadStarter = delegate
                    {
                        _textures[textureName].UnloadContent();
                        _textures.Remove(textureName);
                        _texturesLoaded--;
                    };
                    Thread loadingThread = new Thread(threadStarter);
                    loadingThread.Start();
                }
            }
        }

        public static IVoxTexture GetTexture(string textureName)
        {
            if (textureName != null && _textures.ContainsKey(textureName))
                return _textures[textureName];
            return _textures["missing"];
        }

        public override void Initialize()
        {
            base.Initialize();

            VoxTexture missingTex = new VoxTexture("missing");
            AddTexture(missingTex, "missing");

            VoxTexture blankTex = new VoxTexture("blank");
            AddTexture(blankTex, "blank");

            ThreadStart threadStarter = delegate
            {
                foreach(IVoxTexture texture in _textures.Values)
                {
                    if(!texture.ReadyToRender)
                    {
                        texture.LoadContent();
                        _texturesLoaded++;
                    }
                }
            };

            Thread loadingThread = new Thread(threadStarter);
            loadingThread.Start();

            while (loadingThread.IsAlive) ;
            _initialized = true;
        }
    }
}
