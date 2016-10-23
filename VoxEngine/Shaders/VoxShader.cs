using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.Interfaces;
using VoxEngine.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.Shaders
{
    public class VoxShader : IVoxShader
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private Effect _baseEffect;
        public Effect BaseEffect
        {
            get { return _baseEffect; }
        }

        private bool _readyToRender = false;
        public bool ReadyToRender
        {
            get { return _readyToRender; }
        }

        public VoxShader() { }
        public VoxShader(string fileName)
        {
            _fileName = fileName;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _baseEffect = EngineManager.Game.Content.Load<Effect>("Content/Effects/" + _fileName);
            _readyToRender = true;
        }
    }
}
