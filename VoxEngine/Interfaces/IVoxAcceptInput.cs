using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.GameComponents;

namespace VoxEngine.Interfaces
{
    public interface IVoxAcceptInput : IVoxSceneObject
    {
        void HandleInput(GameTime gameTime, Input input);
    }
}
