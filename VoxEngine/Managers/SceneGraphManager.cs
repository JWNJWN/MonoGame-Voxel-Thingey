using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.GameComponents;
using VoxEngine.SceneObject;
using VoxEngine.SceneObject.SceneGraph;
using VoxEngine.Interfaces;
using BEPUphysics;

namespace VoxEngine.Managers
{
    public class SceneGraphManager : GameComponent
    {
        public static int occluded = 0;
        public static int culled = 0;

        private static Node _root;
        public static Node Root
        {
            get { return _root; }
        }

        public SceneGraphManager(Game game) : base(game)
        {
            Enabled = true;
            _root = new Node();
        }

        public static void Draw(GameTime gameTime)
        {
            _root.Draw(gameTime);
        }

        public static void DrawCulling(GameTime gameTime)
        {
            _root.Sort();
            culled = 0;
            occluded = 0;
            _root.DrawCulling(gameTime);
        }

        public static void HandleInput(GameTime gameTime, Input input)
        {
            _root.HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _root.Update(gameTime);
        }

        public static void LoadContent()
        {
            _root.LoadContent();
        }

        public static void UnloadContent()
        {
            _root.UnloadContent();
        }

        public static void AddObject(VoxSceneObject newObject)
        {
            SceneObjectNode node = new SceneObjectNode(newObject);
            _root.AddNode(node);
        }
    }
}
