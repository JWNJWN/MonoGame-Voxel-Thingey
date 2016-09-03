using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.GameComponents;
using Microsoft.Xna.Framework;

namespace VoxEngine.Interfaces
{
    public interface IVoxCullable : IVoxSceneObject
    {
        bool DrawBoundingBox
        {
            get;
            set;
        }

        bool Culled
        {
            get;
            set;
        }

        bool BoundingBoxCreated
        {
            get;
        }

        BoundingBox BoundingBox
        {
            get;
        }

        BoundingBox GetBoundingBoxTransformed();
    }
}
