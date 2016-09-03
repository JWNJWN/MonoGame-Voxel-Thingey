using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxEngine.Interfaces
{
    public interface IVoxLoadable : IVoxSceneObject
    {
        void LoadContent();
        void UnloadContent();
    }
}
