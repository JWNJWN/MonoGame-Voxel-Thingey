using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.GameComponents;
using VoxEngine.Interfaces;
using VoxEngine.Managers;

namespace VoxEngine.SceneObject.SceneGraph
{
    public class SceneObjectNode : Node, IComparable
    {
        private VoxSceneObject _sceneObject;
        public VoxSceneObject SceneObject
        {
            get { return _sceneObject; }
            set { _sceneObject = value; }
        }

        public SceneObjectNode(VoxSceneObject newObject)
        {
            _sceneObject = newObject;
        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            if (SceneObject is IVoxAcceptInput)
                ((IVoxAcceptInput)SceneObject).HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime)
        {
            if (SceneObject is IVoxUpdateable)
                ((IVoxUpdateable)SceneObject).Update(gameTime);
        }

        public override void UnloadContent()
        {
            if (SceneObject is IVoxLoadable)
                ((IVoxLoadable)SceneObject).UnloadContent();
        }

        public override void LoadContent()
        {
            if (SceneObject is IVoxLoadable)
                ((IVoxLoadable)SceneObject).LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (SceneObject is IVoxCullable && ((IVoxCullable)SceneObject).Culled)
                SceneGraphManager.culled++;
            else if (SceneObject is IVoxOcclusion && ((IVoxOcclusion)SceneObject).Occluded)
                SceneGraphManager.occluded++;
            else
                SceneObject.Draw(gameTime);
        }

        public override void DrawCulling(GameTime gameTime)
        {
            if(SceneObject is IVoxCullable)
            {
                ((IVoxCullable)SceneObject).Culled = false;
                if (CameraManager.ActiveCamera.Frustum.Contains(((IVoxCullable)SceneObject).GetBoundingBoxTransformed()) == ContainmentType.Disjoint)
                    ((IVoxCullable)SceneObject).Culled = true;
                else
                    SceneObject.DrawCulling(gameTime);
            }
        }
    }
}
