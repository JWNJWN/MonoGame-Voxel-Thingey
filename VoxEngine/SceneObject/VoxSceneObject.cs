using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.Managers;

namespace VoxEngine.SceneObject
{
    public class VoxSceneObject : IVoxSceneObject
    {
        private bool _readyToRender = false;
        public bool ReadyToRender
        {
            get { return _readyToRender; }
            set { _readyToRender = value; }
        }

        private Material _material;
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        private string _modelName;
        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; }
        }

        private Vector3 _position = Vector3.Zero;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        
        public float Distance
        {
            get
            {
                if (ReadyToRender)
                    return Vector3.Distance(CameraManager.ActiveCamera.Position, Position);
                else
                    return 0.0f;
            }
        }

        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public virtual Matrix World
        {
            get
            {
                return Matrix.CreateScale(this.Scale) *
                    Matrix.CreateFromQuaternion(this.Rotation) *
                    Matrix.CreateTranslation(this.Position);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if(ReadyToRender)
            {
                IVoxModel model = ModelManager.GetModel(_modelName); 

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
            }
        }

        public virtual void DrawCulling(GameTime gameTime)
        {

        }
    }
}
