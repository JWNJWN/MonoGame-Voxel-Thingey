using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxEngine.Models;
using VoxEngine.Managers;
using VoxEngine.SceneObject;
using VoxEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelGL.GameObjects.Ship
{
    public class Ship : VoxSceneObject, IVoxLoadable, IVoxUpdateable, IVoxSimplePhysics, IVoxOcclusion, IVoxCullable
    {
        private string _occlusionmodelName;
        public string OcclusionModelName
        {
            get { return _occlusionmodelName; }
            set { _occlusionmodelName = value; }
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

        public Ship() { }
        public Ship(Vector3 newPosition)
        {
            Position = newPosition;
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
            Rotation = Quaternion.Multiply(Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, _rotationRate * elapsed));

            IVoxModel model = ModelManager.GetModel(ModelName);
            if(model != null && model.ReadyToRender && !ReadyToRender)
            {
                Matrix[] transforms = new Matrix[model.BaseModel.Bones.Count];
                _boundingBox = new BoundingBox();

                foreach (ModelMesh mesh in model.BaseModel.Meshes)
                    if (!BoundingBoxCreated)
                        _boundingBox = BoundingBox.CreateMerged(_boundingBox, BoundingBox.CreateFromSphere(mesh.BoundingSphere));

                _boundingBoxCreated = true;

                Vector3 min, max;
                min = BoundingBox.Min;
                max = BoundingBox.Max;

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
            RotationRate = 0.0f;
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
            if(ReadyToRender && !Culled)
            {
                _query.Begin();
                IVoxModel model = ModelManager.GetModel(_occlusionmodelName);
                if(model != null && model.ReadyToRender)
                {
                    Matrix[] transforms = new Matrix[model.BaseModel.Bones.Count];
                    model.BaseModel.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach(ModelMesh mesh in model.BaseModel.Meshes)
                    {
                        foreach(BasicEffect effect in mesh.Effects)
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

                while(!_query.IsComplete) { }

                if(_query.IsComplete && _query.PixelCount == 0)
                {
                    Occluded = true;
                }
            }
        }
    }
}
