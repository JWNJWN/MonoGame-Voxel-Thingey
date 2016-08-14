using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel.Engine
{
    public class Scene
    {

        public virtual void Init() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Draw() { }
        public virtual void End() { }
    }
}


