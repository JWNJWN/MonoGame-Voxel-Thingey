using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.Models;
using VoxEngine.Managers;
using VoxEngine.SceneObject;
using VoxEngine.Interfaces;
using VoxEngine.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelGL.GameObjects.Ship
{
    public class ShipMain : VoxSceneObject, IVoxLoadable, IVoxUpdateable, IVoxSimplePhysics, IVoxOcclusion, IVoxAcceptInput
    {
        private string _occlusionModelName;
        public string OcclusionModelName
        {
            get { return _occlusionModelName; }
            set { _occlusionModelName = value; }
        }

        private OcclusionQuery _query = new OcclusionQuery(EngineManager.Device);
        public OcclusionQuery Query
        {
            get { return _query; }
        }

        private bool _occluded = false;
        public bool Occluded
        {
            get { return _occluded; }
            set { _occluded = value; }
        }

        public VertexPositionColor[] points = new VertexPositionColor[8];
        public int[] index = new int[24];

        private bool _culled = false;
        public bool Culled
        {
            get { return _culled; }
            set { _culled = value; }
        }

        private bool _drawBoundingBox = false;
        public bool DrawBoundingBox
        {
            get { return _drawBoundingBox; }
            set { _drawBoundingBox = value; }
        }

        public Vector3 Direction;

        private Vector3 _up = Vector3.Up;
        public Vector3 Up
        {
            get { return _up; }
        }

        private Vector3 _right;
        public Vector3 Right
        {
            get { return _right; }
        }

        private float _rotationRate = 1.0f;
        public float RotationRate
        {
            get { return _rotationRate; }
            set { _rotationRate = value; }
        }

        private float _mass = 1.0f;
        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        private float _thrustForce = 24000.0f;
        public float ThrustForce
        {
            get { return _thrustForce; }
            set { _thrustForce = value; }
        }

        private float _dragFactor = 0.97f;
        public float DragFactor
        {
            get { return _dragFactor; }
            set { _dragFactor = value; }
        }

        private Vector3 _velocity;
        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private Matrix _world;
        public override Matrix World
        {
            get { return _world; }
        }

        private bool _boundingBoxCreated;
        public bool BoundingBoxCreated
        {
            get { return _boundingBoxCreated; }
        }

        public BoundingBox _boundingBox;
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
        }

        public ShipMain()
        {
            Reset();
        }
        public ShipMain(Vector3 newPosition)
        {
            Position = newPosition;
            Reset();
        }

        public BoundingBox GetBoundingBoxTransformed()
        {
            Vector3 min, max;
            min = _boundingBox.Min;
            max = _boundingBox.Max;

            min = Vector3.Transform(_boundingBox.Min, Matrix.CreateTranslation(Position));
            max = Vector3.Transform(_boundingBox.Max, Matrix.CreateTranslation(Position));

            return new BoundingBox(min, max);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            IVoxModel model = ModelManager.GetModel(ModelName);
            if (model != null && model.ReadyToRender && !ReadyToRender)
            {
                Matrix[] transforms = new Matrix[model.BaseModel.Bones.Count];
                model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                _boundingBox = new BoundingBox();

                foreach (ModelMesh mesh in model.BaseModel.Meshes)
                    if (!BoundingBoxCreated)
                        _boundingBox = BoundingBox.CreateMerged(_boundingBox, BoundingBox.CreateFromSphere(mesh.BoundingSphere));

                _boundingBoxCreated = true;

                Vector3 min, max;
                min = Vector3.Transform(BoundingBox.Min, Matrix.CreateTranslation(Position));
                max = Vector3.Transform(BoundingBox.Max, Matrix.CreateTranslation(Position));

                _boundingBox = new BoundingBox(min, max);

                points[0].Position = new Vector3(_boundingBox.Min.X, _boundingBox.Min.Y, _boundingBox.Min.Z);
                points[1].Position = new Vector3(_boundingBox.Max.X, _boundingBox.Min.Y, _boundingBox.Min.Z);
                points[2].Position = new Vector3(_boundingBox.Max.X, _boundingBox.Min.Y, _boundingBox.Max.Z);
                points[3].Position = new Vector3(_boundingBox.Min.X, _boundingBox.Min.Y, _boundingBox.Max.Z);

                points[4].Position = new Vector3(_boundingBox.Min.X, _boundingBox.Max.Y, _boundingBox.Min.Z);
                points[5].Position = new Vector3(_boundingBox.Max.X, _boundingBox.Max.Y, _boundingBox.Min.Z);
                points[6].Position = new Vector3(_boundingBox.Max.X, _boundingBox.Max.Y, _boundingBox.Max.Z);
                points[7].Position = new Vector3(_boundingBox.Min.X, _boundingBox.Max.Y, _boundingBox.Max.Z);

                ReadyToRender = true;
            }
        }

        public void LoadContent()
        {

            VoxModel model = new VoxModel("Ship");

            ModelManager.AddModel(model, "ship");
            this.ModelName = "ship";
            this.OcclusionModelName = "ship";

            index[0] = 0;
            index[1] = 1;
            index[2] = 1;
            index[3] = 2;
            index[4] = 2;
            index[5] = 3;
            index[6] = 3;
            index[7] = 0;

            index[8] = 4;
            index[9] = 5;
            index[10] = 5;
            index[11] = 6;
            index[12] = 6;
            index[13] = 7;
            index[14] = 7;
            index[15] = 4;

            index[16] = 0;
            index[17] = 4;
            index[18] = 1;
            index[19] = 5;
            index[20] = 2;
            index[21] = 6;
            index[22] = 3;
            index[23] = 7;
        }

        public void UnloadContent() { }

        public void Reset()
        {
            Position = new Vector3(0, 0, 0);
            Direction = Vector3.Forward;
            _up = Vector3.Up;
            _right = Vector3.Right;
            Velocity = Vector3.Zero;
        }

        public void HandleInput(GameTime gameTime, Input input)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ReadyToRender)
            {
                Vector2 rotationAmount = -input.CurrentGamePadState.ThumbSticks.Left;
                if (input.CurrentKeyboardState.IsKeyDown(Keys.W))
                    rotationAmount.Y = -1.0f;
                if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
                    rotationAmount.Y = 1.0f;
                if (input.CurrentKeyboardState.IsKeyDown(Keys.D))
                    rotationAmount.X = -1.0f;
                if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
                    rotationAmount.X = 1.0f;
                // Determine thrust amount from input

                float thrustAmount = 0;
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Space))
                    thrustAmount = 1.0f;

                // Scale rotation amount to radians per second
                rotationAmount = rotationAmount * RotationRate * elapsed;

                // Correct the X axis steering when the ship is upside down
                if (Up.Y < 0)
                    rotationAmount.X = -rotationAmount.X;

                // Create rotation matrix from rotation amount
                Matrix rotationMatrix =
                    Matrix.CreateFromAxisAngle(Right, rotationAmount.Y) *
                    Matrix.CreateRotationY(rotationAmount.X);

                // Rotate orientation vectors
                Direction = Vector3.TransformNormal(Direction, rotationMatrix);
                _up = Vector3.TransformNormal(Up, rotationMatrix);

                // Re-normalize orientation vectors
                // Without this, the matrix transformations may introduce small rounding
                // errors which add up over time and could destabilize the ship.
                Direction.Normalize();
                Up.Normalize();

                // Re-calculate Right
                _right = Vector3.Cross(Direction, Up);

                // The same instability may cause the 3 orientation vectors may
                // also diverge. Either the Up or Direction vector needs to be
                // re-computed with a cross product to ensure orthagonality
                _up = Vector3.Cross(Right, Direction);

                // Calculate force from thrust amount
                Vector3 force = Direction * thrustAmount * ThrustForce;

                // Apply acceleration
                Vector3 acceleration = force / Mass;
                Velocity += acceleration * elapsed;

                // Apply psuedo drag
                Velocity *= DragFactor;

                // Apply velocity
                Position += Velocity * elapsed;

                // Reconstruct the ship's world matrix
                _world = Matrix.Identity;
                _world.Forward = Direction;
                _world.Up = Up;
                _world.Right = Right;
                _world.Translation = Position;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            if (_drawBoundingBox)
            {
                IVoxShader shader = ShaderManager.GetShader("basic");

                if (shader != null && shader.ReadyToRender)
                {
                    BasicEffect effect = shader.BaseEffect as BasicEffect;
                    effect.DiffuseColor = Color.Red.ToVector3();
                    effect.View = CameraManager.ActiveCamera.View;
                    effect.Projection = CameraManager.ActiveCamera.Projection;
                    effect.World = World;

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        EngineManager.Device.DrawUserIndexedPrimitives(PrimitiveType.LineList, points, 0, 8, index, 0, 12, VertexPositionColor.VertexDeclaration);
                    }
                }
            }
        }

        public override void DrawCulling(GameTime gameTime)
        {
            Occluded = false;
            if (ReadyToRender && !Culled)
            {
                _query.Begin();
                IVoxModel model = ModelManager.GetModel(_occlusionModelName);
                if (model != null && model.ReadyToRender)
                {
                    Matrix[] transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mesh in model.BaseModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;
                            effect.World = World;
                            effect.View = CameraManager.ActiveCamera.View;
                            effect.Projection = CameraManager.ActiveCamera.Projection;
                        }
                        mesh.Draw();
                    }
                }
                _query.End();

                while (!_query.IsComplete) { }

                if (_query.IsComplete && _query.PixelCount == 0)
                {
                    Occluded = true;
                }
            }
        }
    }
}
