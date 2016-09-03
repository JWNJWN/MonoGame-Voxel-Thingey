using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.Interfaces
{
    public interface IVoxUpdateable : IVoxSceneObject
    {
        void Update(GameTime gameTime);
    }
}
