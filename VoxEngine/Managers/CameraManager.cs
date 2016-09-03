using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.GameComponents;

namespace VoxEngine.Managers
{
    public class CameraManager : GameComponent
    {
        private static Hashtable _cameras = new Hashtable();
        public enum CameraNumber
        {
            _default = 1,
            _dolly = 2,
            _3 = 3,
            _4 = 4,
            _5 = 5,
            _6 = 6,
            _7 = 7,
            _8 = 8,
            _9 = 9,
            _10 = 10
        }

        private static bool _initialized = false;
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static Camera _activeCamera;
        public static Camera ActiveCamera
        {
            get { return _activeCamera; }
        }

        public CameraManager(Game game) : base(game)
        {
            Enabled = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            AddCamera(new FirstPersonCamera(), CameraNumber._default);
            SetActiveCamera(CameraNumber._default);
            AddCamera(new FirstPersonCamera(), CameraNumber._dolly);

            _initialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _activeCamera.Update(gameTime);
        }

        public static void AddCamera(Camera newCamera, CameraNumber cameraNumber)
        {
            if (!_cameras.Contains(cameraNumber))
                _cameras.Add(cameraNumber, newCamera);
        }

        public static void SetAllCamerasProjectionMatrix(float aspectRatio)
        {
            foreach (Camera camera in _cameras.Values)
                camera.Projection = Matrix.CreatePerspectiveFieldOfView(camera.FieldOfView, aspectRatio, camera.NearPlane, camera.FarPlane);
        }

        public static void SetActiveCamera(CameraNumber cameraNumber)
        {
            if (_cameras.ContainsKey(cameraNumber))
                _activeCamera = _cameras[cameraNumber] as Camera;
        }
    }
}
