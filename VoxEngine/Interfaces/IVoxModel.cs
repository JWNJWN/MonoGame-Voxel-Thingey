using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace VoxEngine.Interfaces
{
    public interface IVoxModel
    {
        string FileName
        {
            get;
            set;
        }

        Model BaseModel
        {
            get;
        }

        bool ReadyToRender
        {
            get;
        }

        void LoadContent();
    }
}
