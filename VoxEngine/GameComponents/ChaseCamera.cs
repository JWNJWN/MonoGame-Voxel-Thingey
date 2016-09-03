using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxEngine.GameComponents
{
    public class ChaseCamera : Camera
    {
        private bool _springEnabled = true;
        public bool SpringEnabled
        {
            get { return _springEnabled; }
            set { _springEnabled = value; }
        }

        private Vector3 _velocity;
        public Vector3 Velocity
        {
            get { return _velocity; }
        }

        private float _stiffness = 1800.0f;
        public float Stiffness
        {
            get { return _stiffness; }
            set { _stiffness = value; }
        }

        private float _damping = 600.0f;
        public float Damping
        {
            get { return _damping; }
            set { _damping = value; }
        }

        private float _mass = 50.0f;
        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        private Vector3 _chasePosition;
        public Vector3 ChasePosition
        {
            get { return _chasePosition; }
            set { _chasePosition = value; }
        }

        private Vector3 _chaseDirection;
        public Vector3 ChaseDirection
        {
            get { return _chaseDirection; }
            set { _chaseDirection = value; }
        }

        private Vector3 _up = Vector3.Up;
        public Vector3 Up
        {
            get { return _up; }
            set { _up = value; }
        }

        private Vector3 _desiredPositionOffset = new Vector3(0, 2.0f, 2.0f);
        public Vector3 DesiredPositionOffset
        {
            get { return _desiredPositionOffset; }
            set { _desiredPositionOffset = value; }
        }

        private Vector3 _desiredPosition;
        public Vector3 DesiredPosition
        {
            get { return _desiredPosition; }
            set { _desiredPosition = value; }
        }

        private Vector3 _lookAtOffset = new Vector3(0, 2.8f, 0);
        public Vector3 LookAtOffset
        {
            get { return _lookAtOffset; }
            set { _lookAtOffset = value; }
        }

        private Vector3 _lookAt;
        public Vector3 LookAt
        {
            get { return _lookAt; }
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePosition();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 stretch = Position - _desiredPosition;
            Vector3 force = -_stiffness * stretch - _damping * _velocity;

            Vector3 acceleration = force / _mass;
            _velocity += acceleration * elapsed;

            Position += _velocity * elapsed;

            UpdateMatrices();
        }

        private void UpdatePosition()
        {
            // Construct a matrix to transform from object space to worldspace
            Matrix transform = Matrix.Identity;
            transform.Forward = ChaseDirection;
            transform.Up = Up;
            transform.Right = Vector3.Cross(Up, ChaseDirection);

            // Calculate desired camera properties in world space
            _desiredPosition = ChasePosition +
                Vector3.TransformNormal(DesiredPositionOffset, transform);
            _lookAt = ChasePosition +
                Vector3.TransformNormal(LookAtOffset, transform);
        }

        private void UpdateMatrices()
        {
            View = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ReflectedFrustum = new BoundingFrustum(Matrix.Multiply(ReflectedView, Projection));
        }

        public override void Reset()
        {
            UpdatePosition();

            _velocity = Vector3.Zero;

            Position = _desiredPosition;

            UpdateMatrices();
        }
    }
}
