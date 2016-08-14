using Microsoft.Xna.Framework;

namespace Voxel.Engine.Managers
{
    abstract public class BaseManager
    {
        private SceneGame game;
        public SceneGame Game
        {
            get { return this.game; }
        }

        public string Name
        {
            get { return this.GetName(); }
        }

        public BaseManager(SceneGame game)
        {
            this.game = game;
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        protected virtual void Initialize()
        {
            this.game.AddManager(this);
        }

        protected abstract string GetName();
    }
}
