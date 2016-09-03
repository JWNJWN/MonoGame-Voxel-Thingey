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
    public class PrimitiveEffect : IVoxShader
    {
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

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _baseEffect = EngineManager.Game.Content.Load<Effect>("Content/Effects/Primitive");
            _readyToRender = true;
        }
    }
}
