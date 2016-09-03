using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;

namespace VoxEngine.Shaders
{
    public class basicEffect : IVoxShader
    {
        private BasicEffect _baseEffect;
        public Effect BaseEffect { get { return _baseEffect; } }

        private bool _readyToRender = false;
        public bool ReadyToRender { get { return _readyToRender; } }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _baseEffect = new BasicEffect(graphicsDevice);
            _readyToRender = true;
        }
    }
}
