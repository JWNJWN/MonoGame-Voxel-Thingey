using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.SceneObject;
using VoxEngine.Managers;

namespace VoxelGL.GameObjects
{
    public class Chunk : VoxSceneObject, IVoxLoadable, IVoxUpdateable, IVoxSimplePhysics
    {

        public Vector3 Up
        {
            get { return Vector3.Up; }
        }
        public Vector3 Right
        {
            get { return Vector3.Right; }
        }

        private float _rotationRate = 0.0f;
        public float RotationRate
        {
            get { return _rotationRate; }
            set { _rotationRate = value; }
        }

        public float Mass
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public float ThrustForce
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public float DragFactor
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Vector3 Velocity
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void LoadContent()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
