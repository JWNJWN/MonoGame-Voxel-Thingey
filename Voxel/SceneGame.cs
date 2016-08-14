using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Voxel.Engine.Managers;

namespace Voxel
{
    public class SceneGame : Game
    {

        private GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return this.graphics; }
            set { this.graphics = value; }
        }

        private Dictionary<string, BaseManager> managers;
        public Dictionary<string, BaseManager> Managers
        {
            get { return this.managers; }
        }

        public SceneGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

            this.managers = new Dictionary<string, BaseManager>();

            Window.Title = "Voxel Main";
        }

        protected override void Initialize()
        {
            SceneManager sceneManager = new SceneManager(this);
            RenderManager renderManager = new RenderManager(this);

            base.Initialize();
        }

        public void AddManager(BaseManager manager)
        {
            BaseManager checkManager = null;
            if (managers.TryGetValue(manager.Name, out checkManager))
                throw new Exception("Manager type " + manager.Name + " already exists within the game engine.");
            managers.Add(manager.Name, manager);
        }

        public BaseManager GetManager(string managerName)
        {
            BaseManager manager = null;
            if (!managers.TryGetValue(managerName, out manager))
                throw new Exception("Manager type " + managerName + " wasn't found within the game engine.");
            return manager;
        }

        protected override void LoadContent()
        {
            foreach (KeyValuePair<string, BaseManager> pair in managers)
                (pair.Value).LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (KeyValuePair<string, BaseManager> pair in managers)
                (pair.Value).UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if(this.IsActive)
            {
                foreach (KeyValuePair<string, BaseManager> pair in managers)
                    (pair.Value).Update(gameTime);

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            foreach (KeyValuePair<string, BaseManager> pair in managers)
                (pair.Value).Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
