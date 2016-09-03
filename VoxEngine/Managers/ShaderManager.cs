using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.Shaders;

namespace VoxEngine.Managers
{
    public class ShaderManager : GameComponent
    {
        private static Dictionary<string, IVoxShader> _shaders = new Dictionary<string, IVoxShader>();

        private static int _shadersLoaded = 0;
        public static int ShadersLoaded
        {
            get { return _shadersLoaded; }
        }

        private static bool _initialized = false;

        public static bool Initialized
        {
            get { return _initialized; }
        }

        public ShaderManager(Game game) : base(game) { }

        public static void AddShader(IVoxShader newShader, string shaderLabel)
        {
            if(shaderLabel != null && !_shaders.ContainsKey(shaderLabel))
            {
                _shaders.Add(shaderLabel, newShader);
                if(_initialized)
                {
                    ThreadStart threadStarter = delegate
                    {
                        newShader.Initialize(EngineManager.Device);
                        _shadersLoaded++;
                    };
                    Thread loadingThread = new Thread(threadStarter);
                    loadingThread.Start();
                }
            }
        }

        public static IVoxShader GetShader(String shaderLabel)
        {
            if(shaderLabel != null && _shaders.ContainsKey(shaderLabel))
                    return _shaders[shaderLabel];
            return null;
        }

        public override void Initialize()
        {
            base.Initialize();

            AddShader(new basicEffect(), "basic");
            AddShader(new PrimitiveEffect(), "primitive");

            ThreadStart threadStarter = delegate
            {
                foreach (IVoxShader shader in _shaders.Values)
                {
                    if (!shader.ReadyToRender)
                    {
                        shader.Initialize(EngineManager.Device);
                        _shadersLoaded++;
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
