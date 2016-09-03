using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.GameComponents
{
    public class Camera
    {
        private Vector3 _position = Vector3.Zero;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private float _fieldOfView = MathHelper.Pi / 2.0f;
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; }
        }

        private float _nearPlane = 0.1f;
        public float NearPlane
        {
            get { return _nearPlane; }
            set { _nearPlane = value; }
        }

        private float _farPlane = 3500.0f;
        public float FarPlane
        {
            get { return _farPlane; }
            set { _farPlane = value; }
        }

        public float ViewableFieldOfView
        {
            get { return FieldOfView / 1.125f; }
        }

        private Matrix _view;
        public Matrix View
        {
            get { return _view; }
            set { _view = value; }
        }

        private Matrix _reflectedView;
        public Matrix ReflectedView
        {
            get { return _reflectedView; }
            set { _reflectedView = value; }
        }

        private Matrix _projection;
        public Matrix Projection
        {
            get { return _projection; }
            set { _projection = value; }
        }

        private BoundingFrustum _frustum;
        public BoundingFrustum Frustum
        {
            get { return _frustum; }
            set { _frustum = value; }
        }

        private BoundingFrustum _reflectedFrustum;
        public BoundingFrustum ReflectedFrustum
        {
            get { return _reflectedFrustum; }
            set { _reflectedFrustum = value; }
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void SetPosition(Vector3 newPosition) { }
        public virtual void Translate(Vector3 move) { }
        public virtual void RotateX(float angle) { }
        public virtual void RotateY(float angle) { }
        public virtual void RotateZ(float angle) { }
        public virtual void Reset() { }
    }
}
