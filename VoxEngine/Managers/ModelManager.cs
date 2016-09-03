using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.Interfaces;

namespace VoxEngine.Managers
{
    public class ModelManager : GameComponent
    {
        private static Dictionary<string, IVoxModel> _models = new Dictionary<string, IVoxModel>();

        private static bool _initialized = false;

        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static int _modelsLoaded = 0;
        public static int ModelsLoaded
        {
            get { return _modelsLoaded; }
        }

        public ModelManager(Game game) : base(game)
        {

        }

        public static void AddModel(IVoxModel newModel, string modelName)
        {
            if(modelName != null && !_models.ContainsKey(modelName))
            {
                _models.Add(modelName, newModel);
                if(_initialized)
                {
                    ThreadStart threadStarter = delegate
                    {
                        newModel.LoadContent();
                        _modelsLoaded++;
                    };

                    Thread loadingThread = new Thread(threadStarter);
                    loadingThread.Start();
                }
            }
        }

        public static IVoxModel GetModel(string modelName)
        {
            if (modelName != null && _models.ContainsKey(modelName))
                return _models[modelName];
            return null;
        }

        public override void Initialize()
        {
            base.Initialize();

            ThreadStart threadStarter = delegate
            {
                foreach (IVoxModel model in _models.Values)
                {
                    if (!model.ReadyToRender)
                    {
                        model.LoadContent();
                        _modelsLoaded++;
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
