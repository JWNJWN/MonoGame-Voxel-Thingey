using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.Interfaces
{
    public interface IVoxSimplePhysics : IVoxSceneObject
    {
        Vector3 Up
        {
            get;
        }
        Vector3 Right
        {
            get;
        }
        float RotationRate
        {
            get;
            set;
        }
        float Mass
        {
            get;
            set;
        }
        float ThrustForce
        {
            get;
            set;
        }
        float DragFactor
        {
            get;
            set;
        }
        Vector3 Velocity
        {
            get;
            set;
        }
        void Reset();
    }
}
