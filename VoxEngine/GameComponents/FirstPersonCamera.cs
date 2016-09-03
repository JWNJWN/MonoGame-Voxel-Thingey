using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.GameComponents
{
    public class FirstPersonCamera : Camera
    {
        private Vector3 _cameraReference = new Vector3(0, 0, 10);
        public Vector3 CameraReference
        {
            get { return _cameraReference; }
        }

        private float _yaw = 0.0f;
        private float _pitch = 0.0f;

        public override void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public void SetCameraReference(Vector3 newReference)
        {
            _cameraReference = newReference;
        }

        public override void Translate(Vector3 move)
        {
            Matrix forwardMovement = Matrix.CreateRotationY(_yaw);
            Vector3 v = new Vector3(0, 0, 0);
            v = Vector3.Transform(move, forwardMovement);

            Position += v;
        }

        public override void RotateY(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            if (_yaw >= MathHelper.Pi * 2)
                _yaw = MathHelper.ToRadians(0.0f);
            else if (_yaw <= -MathHelper.Pi * 2)
                _yaw = MathHelper.ToRadians(0.0f);
            _yaw += angle;
        }

        public override void RotateX(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            _pitch += angle;
            if (_pitch >= MathHelper.ToRadians(75))
                _pitch = MathHelper.ToRadians(75);
            else if (_pitch <= MathHelper.ToRadians(-75))
                _pitch = MathHelper.ToRadians(-75);
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 cameraPosition = Position;
            Matrix rotationMatrix = Matrix.CreateRotationY(_yaw);
            Matrix pitchMatrix = Matrix.Multiply(Matrix.CreateRotationX(_pitch), rotationMatrix);
            Vector3 transformedReference = Vector3.Transform(_cameraReference, pitchMatrix);
            Vector3 cameraLookat = cameraPosition + transformedReference;

            View = Matrix.CreateLookAt(cameraPosition, cameraLookat, Vector3.Up);

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            ReflectedFrustum = new BoundingFrustum(Matrix.Multiply(ReflectedView, Projection));
        }
    }
}
